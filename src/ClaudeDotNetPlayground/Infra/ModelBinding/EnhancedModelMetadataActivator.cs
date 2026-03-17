using System.Reflection;
using System.Reflection.Emit;

namespace ClaudeDotNetPlayground.Infra.ModelBinding;

/// <summary>
/// Workaround para .NET 10 com PublishAot=true: ModelMetadata.IsEnhancedModelMetadataSupported
/// é um static readonly bool iniciado como false. Com MVC não-AOT rodando em runtime JIT,
/// os providers (SimpleTypeModelBinderProvider, TryParseModelBinderProvider) verificam esse
/// flag antes de acessar IsConvertibleType e IsParseableType, lançando NotSupportedException.
/// Este activator usa DynamicMethod com skipVisibility=true para emitir Stsfld diretamente
/// no backing field readonly, contornando a restrição de initonly sem FieldAccessException.
/// </summary>
internal static class EnhancedModelMetadataActivator
{
    internal static void Activate(ILogger logger)
    {
        var modelMetadataType = typeof(Microsoft.AspNetCore.Mvc.ModelBinding.ModelMetadata);

        var backingField = modelMetadataType.GetField(
            "<IsEnhancedModelMetadataSupported>k__BackingField",
            BindingFlags.NonPublic | BindingFlags.Static);

        if (backingField is not null)
        {
            try
            {
                var dm = new DynamicMethod(
                    "SetEnhancedModelMetadataSupported",
                    typeof(void),
                    Type.EmptyTypes,
                    modelMetadataType,
                    skipVisibility: true);

                var il = dm.GetILGenerator();
                il.Emit(OpCodes.Ldc_I4_1);
                il.Emit(OpCodes.Stsfld, backingField);
                il.Emit(OpCodes.Ret);

                dm.Invoke(null, null);

                logger.LogInformation(
                    "[EnhancedModelMetadataActivator][Activate] IsEnhancedModelMetadataSupported definido via DynamicMethod em {Type}",
                    modelMetadataType.FullName);
                return;
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    "[EnhancedModelMetadataActivator][Activate] DynamicMethod falhou: {Message}",
                    ex.Message);
            }
        }

        logger.LogWarning(
            "[EnhancedModelMetadataActivator][Activate] IsEnhancedModelMetadataSupported não pôde ser ativado — model binding pode falhar");
    }
}
