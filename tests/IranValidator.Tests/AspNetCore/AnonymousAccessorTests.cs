using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection;

namespace IranValidator.Tests.AspNetCore;

/// <summary>
/// Reads properties of anonymous types across assembly boundaries.
/// The DLR cannot bind to internal anonymous types created in the library
/// assemblies (RuntimeBinderException), so reflection is used instead.
/// </summary>
internal static class AnonymousAccessor
{
    public static T? Get<T>(object? instance, string propertyName)
        => (T?)instance?.GetType().GetProperty(propertyName)?.GetValue(instance);

    /// <summary>
    /// In .NET 10, ActionContext.ModelState is a get-only auto-property
    /// (<c>{ get; } = default!</c>) that MVC model binding sets at runtime —
    /// there is no public setter, no service/Items lookup, and no setter method
    /// in metadata at all. The only way to populate it in a unit test is to
    /// assign the compiler-generated backing field.
    /// </summary>
    public static void SetModelState(ActionContext context, ModelStateDictionary modelState)
        => typeof(ActionContext)
            .GetField("<ModelState>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(context, modelState);
}
