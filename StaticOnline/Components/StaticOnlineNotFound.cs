using Microsoft.AspNetCore.Components;

namespace SilentOrbit.StaticOnline.Components;

public class StaticOnlineNotFound : ComponentBase
{
    [Inject]
    public SitePage Page { get; set; } = null!;

    protected override void OnParametersSet()
    {
        //Debug.Fail(Page.URL);
        Page.NotFound = true;
        base.OnParametersSet();
    }

}
