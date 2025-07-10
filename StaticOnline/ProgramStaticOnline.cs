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
    public static bool BuildMain(string[] args, SiteConfig config, out int exitCode)
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
            Build(config, target);

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

            //Parse Dll from args
            var asmPath =
                Path.GetFullPath(
                Path.Combine(
                    Directory.GetCurrentDirectory(),
                    args[0].Trim()));
            Console.WriteLine(@$"Loading ""{args[0]}""");
            Console.WriteLine(@$"Loading ""{asmPath}""");
            var asm = Assembly.LoadFile(asmPath);
            //Find the Type of SiteConfig to build
            var types = asm.GetTypes();
            var configTypes = types.Where(t => t.IsAssignableTo(typeof(SiteConfig))).ToArray();
            if (configTypes.Length != 1)
                throw new Exception($"Expected one subclass of {nameof(SiteConfig)}<App>, found {configTypes.Length}.");

            //Create config
            var config = (SiteConfig)Activator.CreateInstance(configTypes[0])!;

            //Parse target from args
            var target = ParseTargetDir(args);

            //Build site
            Build(config, target);
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
        Console.WriteLine($@"Target: ""{args[1]}""
   ==> ""{target}""");

        return target;
    }

    static void PrintArgs(string[] args)
    {
        Console.WriteLine($@"Args: {ExeName} ""{string.Join(@""" """, args)}""");
    }

    static string ExeName => Path.GetFileName(Assembly.GetEntryAssembly()!.Location);

    public static void Build(SiteConfig config, DirPath target)
    {
        var sg = new SiteBuilder(config, target);

        config.WwwRoot ??= FindWwwRoot(config);

        Console.WriteLine($"Generating: {config.BaseURL}");

        sg.Scan();

        target.DeleteDir();
        target.CreateDirectory();

        sg.Build().Wait();

        Console.WriteLine("Done");
    }

    static DirPath FindWwwRoot(SiteConfig site)
    {
        var asmPath = new FilePath(site.AppType.Assembly.Location);

        var dir = asmPath.Parent.CombineDir("wwwroot");
        if (dir.Exists())
        {
            Console.WriteLine($"Found {dir} next to {asmPath}");
            return dir;
        }

        //Try to find wwwroot when running in Visual Studio
        dir = asmPath.Parent;
        if (dir.Path.EndsWith(@"\bin\Debug\net9.0") ||
            dir.Path.EndsWith(@"\bin\Release\net9.0"))
        {
            dir = dir.Parent.Parent.Parent.CombineDir("wwwroot");
            if (dir.Exists())
            {
                Console.WriteLine($"Found {dir}\n   in project root above {asmPath}");
                return dir;
            }
        }

        Debug.Fail($"Failed to find wwwroot near\n   {asmPath}");
        Console.Error.WriteLine($"Failed to find wwwroot near:\n   {asmPath}");
        Console.Error.WriteLine($"You must configure {nameof(SiteConfig.WwwRoot)} in code.");
        throw new ArgumentException("Missing wwwroot path in config.");
    }

}
