using Microsoft.AspNetCore.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline.Building;
using SilentOrbit.StaticOnline.Config;
using SilentOrbit.StaticOnline.Tools;
using System.Buffers.Text;
using System.Reflection;

namespace SilentOrbit.StaticOnline;

static class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine(@"Args: {0} ""{1}""",
            Path.GetFileName(Assembly.GetEntryAssembly()!.Location),
            string.Join(@""" """, args));

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
            if (args.Length < 2)
                throw new ArgumentException("Expected 2:nd argument to be target directory.");

            var target = DirPath.GetCurrentDirectory().CombineDir(args[1]);
            Console.WriteLine($@"Target: ""{args[1]}"" ==> ""{target}""");

            //Build site
            var sb = new SiteBuilder(config, target);

            Console.WriteLine($"Generating: {config.BaseURL}");
            sb.Build().Wait();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(new StackTrace(ex));
            Console.WriteLine(@"Usage: StaticOnline.exe ../path/to/Blog.dll C:\Path\To\GeneratedOutput");
            throw;
        }
    }

}
