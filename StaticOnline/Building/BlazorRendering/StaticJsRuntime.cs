using Microsoft.JSInterop;
using System.Diagnostics.CodeAnalysis;

namespace SilentOrbit.StaticOnline.Building.BlazorRendering;

class StaticJsRuntime : IJSRuntime
{
    ValueTask<TValue> IJSRuntime.InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, object?[]? args)
    {
        throw new NotImplementedException();
    }

    ValueTask<TValue> IJSRuntime.InvokeAsync<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.PublicProperties)] TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        throw new NotImplementedException();
    }
}
