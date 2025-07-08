using SilentOrbit.Disk;
using System.Reflection;

namespace BuildMyBlog;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine($@"Args: ""{string.Join(@""" """, args)}""");

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
            config.TargetDir = DirPath.GetCurrentDirectory().CombineDir(args[1].Trim());
            Console.WriteLine($@"Target ""{args[1]}""");
            Console.WriteLine($@"Target ""{config.TargetDir}""");

            Console.WriteLine($"URL: {config.BaseURL}");

            //Generating with found app
            await config.Build();
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"{ex.GetType().Name}: {ex.Message}");
            Console.Error.WriteLine(new StackTrace(ex));
            Console.WriteLine(@"Usage: StaticOnline.exe ../path/to/Blog.dll C:\Path\To\GeneratedOutput");
            return -1;
        }
    }
}
