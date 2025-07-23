namespace SilentOrbit.StaticOnline.Config.Data;

public enum BuildStage
{
    Added = 0,
    PreScan = 10,
    FinalBuild = 20,
    /// <summary>
    /// Page with URL having query or fragment is not rendered and considered done.
    /// </summary>
    Skipped = 21,
    Fail = 30,
    Draft = 31,
}
