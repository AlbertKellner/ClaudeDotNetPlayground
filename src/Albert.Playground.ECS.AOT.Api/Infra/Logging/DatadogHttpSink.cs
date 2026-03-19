using System.Text.Json;
using System.Threading.Channels;
using Serilog.Core;
using Serilog.Events;

namespace Albert.Playground.ECS.AOT.Api.Infra.Logging;

internal sealed class DatadogHttpSink : ILogEventSink, IAsyncDisposable
{
    private readonly HttpClient _httpClient;
    private readonly Channel<DatadogLogEntry> _channel;
    private readonly Task _processTask;
    private readonly CancellationTokenSource _cts = new();
    private readonly string _service;
    private readonly string _host;
    private readonly string _env;

    public DatadogHttpSink(string apiKey, string service, string host, string env)
    {
        _service = service;
        _host = host;
        _env = env;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://http-intake.logs.datadoghq.com"),
            DefaultRequestHeaders =
            {
                { "DD-API-KEY", apiKey }
            }
        };

        _channel = Channel.CreateBounded<DatadogLogEntry>(new BoundedChannelOptions(1000)
        {
            FullMode = BoundedChannelFullMode.DropOldest,
            SingleReader = true
        });

        _processTask = Task.Run(ProcessBatchAsync);
    }

    public void Emit(LogEvent logEvent)
    {
        var entry = new DatadogLogEntry
        {
            Message = logEvent.RenderMessage(),
            Timestamp = logEvent.Timestamp.ToUnixTimeMilliseconds(),
            Level = logEvent.Level.ToString().ToLowerInvariant(),
            Service = _service,
            Host = _host,
            DdTags = $"env:{_env}"
        };

        _channel.Writer.TryWrite(entry);
    }

    private async Task ProcessBatchAsync()
    {
        var batch = new List<DatadogLogEntry>(50);

        while (!_cts.Token.IsCancellationRequested)
        {
            try
            {
                batch.Clear();

                if (await _channel.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (batch.Count < 50 && _channel.Reader.TryRead(out var entry))
                    {
                        batch.Add(entry);
                    }

                    if (batch.Count > 0)
                    {
                        await SendBatchAsync(batch);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch
            {
                await Task.Delay(5000, _cts.Token);
            }
        }

        while (_channel.Reader.TryRead(out var remaining))
        {
            batch.Add(remaining);
        }

        if (batch.Count > 0)
        {
            await SendBatchAsync(batch);
        }
    }

    private async Task SendBatchAsync(List<DatadogLogEntry> batch)
    {
        var json = JsonSerializer.Serialize(batch, DatadogLogJsonContext.Default.ListDatadogLogEntry);

        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

        await _httpClient.PostAsync("/api/v2/logs", content);
    }

    public async ValueTask DisposeAsync()
    {
        _channel.Writer.Complete();

        await _cts.CancelAsync();

        try
        {
            await _processTask.WaitAsync(TimeSpan.FromSeconds(5));
        }
        catch (TimeoutException) { }
        catch (OperationCanceledException) { }

        _cts.Dispose();
        _httpClient.Dispose();
    }
}
