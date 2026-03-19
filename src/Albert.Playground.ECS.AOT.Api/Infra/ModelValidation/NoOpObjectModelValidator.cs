using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Albert.Playground.ECS.AOT.Api.Infra.ModelValidation;

/// <summary>
/// No-op IObjectModelValidator used when PublishAot=true causes DefaultObjectValidator
/// to fail with NotSupportedException (IsConvertibleType requires IsEnhancedModelMetadataSupported).
/// MVC does not support Native AOT in .NET 10 (IL2026). This validator bypasses the crash
/// while preserving business validation in use cases.
/// </summary>
internal sealed class NoOpObjectModelValidator : IObjectModelValidator
{
    public void Validate(
        ActionContext actionContext,
        ValidationStateDictionary? validationState,
        string prefix,
        object? model)
    {
    }
}
