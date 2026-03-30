using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Starter.Template.AOT.Api.Infra.ModelBinding;

/// <summary>
/// Substitui providers que acessam IsParseableType, IsEnhancedModelMetadataSupported ou
/// outras propriedades de ModelMetadata que lançam NotSupportedException quando
/// PublishAot=true + .NET 10 (IsEnhancedModelMetadataSupported = false).
/// Retorna null para que o próximo provider na cadeia seja tentado.
/// </summary>
internal sealed class NullModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context) => null;
}
