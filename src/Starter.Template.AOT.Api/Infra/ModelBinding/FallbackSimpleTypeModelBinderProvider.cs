using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace Starter.Template.AOT.Api.Infra.ModelBinding;

/// <summary>
/// Substitui SimpleTypeModelBinderProvider para contornar a NotSupportedException causada por
/// IsConvertibleType quando PublishAot=true + .NET 10 (IsEnhancedModelMetadataSupported = false).
/// Usa TypeDescriptor.GetConverter diretamente em vez de ModelMetadata.IsConvertibleType.
/// </summary>
internal sealed class FallbackSimpleTypeModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        var type = context.Metadata.ModelType;
        var converter = TypeDescriptor.GetConverter(type);

        if (converter.CanConvertFrom(typeof(string)))
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new SimpleTypeModelBinder(type, loggerFactory);
        }

        return null;
    }
}
