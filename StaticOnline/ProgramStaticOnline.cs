using Microsoft.AspNetCore.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;
using SilentOrbit.StaticOnline.Config;
using SilentOrbit.StaticOnline.Tools;
using System.Buffers.Text;
using System.Reflection;

namespace SilentOrbit.StaticOnline;

public static class ProgramStaticOnline
{
    /// <summary>
    /// Return true if the program should exit.
    /// Return false if the program should continue with running the live website.
    /// </summary>
    public static bool BuildMain<TSiteConfig>(string[] args, out int exitCode)
        where TSiteConfig : SiteConfig, new()
    {
        PrintArgs(args);

        try
        {
            if (args.Length == 0 || args[0] != "build")
            {
                exitCode = 0;
                return false;
            }

            var target = ParseTargetDir(args);
            var config = new TSiteConfig();
            Build(target, config);

            exitCode = 0;
            return true;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(new StackTrace(ex));

            Console.WriteLine($@"Usage: {ExeName} build <output path>");

            exitCode = -1;
            return true;
        }
    }

    internal static void Main(string[] args)
    {
        PrintArgs(args);

        try
        {
            if (args.Length != 2)
                throw new ArgumentException($"Expected 2 arguments, got {args.Length}");
            var asmPath =
                Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    args[0].Trim()));
            Console.WriteLine(@$"Loading ""{args[0]}""");
            Console.WriteLine(@$"Loading ""{asmPath}""");
            var asm = Assembly.LoadFile(asmPath);
            var types = asm.GetTypes();
            var configTypes = types.Where(t => t.IsAssignableTo(typeof(SiteConfig))).ToArray();
            if (configTypes.Length != 1)
                throw new Exception($"Expected one subclass of {nameof(SiteConfig)}<App>, found {configTypes.Length}.");

            var config = (SiteConfig)Activator.CreateInstance(configTypes[0])!;
            var target = ParseTargetDir(args);
            Build(target, config);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(new StackTrace(ex));
            Console.WriteLine(@"Usage: StaticOnline.exe ../path/to/Blog.dll C:\Path\To\GeneratedOutput");
            throw;
        }
    }

    static DirPath ParseTargetDir(string[] args)
    {
        if (args.Length < 2)
            throw new ArgumentException("Expected 2:nd argument to be target directory.");

        var workingDir = DirPath.GetCurrentDirectory();
        var target = workingDir.CombineDir(args[1]);
        Console.WriteLine($@"Target argument: ""{args[1]}""");
        Console.WriteLine($@"Target parsed:   ""{target}""");

        return target;
    }

    static void PrintArgs(string[] args)
    {
        Console.WriteLine($@"Args: {ExeName} ""{string.Join(@""" """, args)}""");
    }

    static string ExeName => Path.GetFileName(Assembly.GetEntryAssembly()!.Location);

    static void Build(DirPath target, SiteConfig config)
    {
        target.DeleteDir();
        target.CreateDirectory();

        var sg = new SiteBuilder(config, target);
        Console.WriteLine($"Generating: {config.BaseURL}");
        sg.Scan();
        sg.Build().Wait();

        Console.WriteLine("Done");
    }

}
