namespace SilentOrbit.StaticOnline.Config;

public enum BuildStage
{
    Added = 0,
    PreScan = 10,
    PreScanDone = 11,
    FinalBuild = 20,
    FinalBuildDone = 21,
    Fail = 30,
}
