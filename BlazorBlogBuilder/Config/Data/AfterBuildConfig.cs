namespace SilentOrbit.BlazorBlogBuilder.Config;

public enum AfterBuildConfig
{
    /// <summary>
    /// Shut down immediately after the site is generated.
    /// Set to false when you want to explote the site during development.
    /// Set to true when running in automatic builds such as GitHub Action.
    /// </summary>
    Exit = 0,

    /// <summary>
    /// Keep the WebServer running for live testing.
    /// </summary>
    KeepRunning = 1,

    /// <summary>
    /// Launch the browser with the running website.
    /// </summary>
    LaunchBrowser = 2,
}
