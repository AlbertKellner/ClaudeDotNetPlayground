namespace Starter.Template.AOT.Api.Infra.Correlation;

internal static class GuidV7
{
    internal static Guid Create() => Guid.CreateVersion7();

    internal static bool IsVersion7(Guid guid)
    {
        Span<byte> bytes = stackalloc byte[16];
        guid.TryWriteBytes(bytes, bigEndian: true, out _);
        return (bytes[6] >> 4) == 7 && (bytes[8] & 0xC0) == 0x80;
    }
}
