using SilentOrbit.StaticOnline.Building.FileGeneration;
using System.Reflection;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Generate other types of files
/// such as feeds and sitemaps
/// </summary>
class FileBuilder(SiteBuilder builder)
{
    public void Init()
    {
        foreach (var generator in FindGenerators())
        {
            generator.Init();
        }
    }

    public List<FileGeneratorBase> FindGenerators()
    {
        var list = new List<FileGeneratorBase>();

        //Scan site assembly
        AddList(list, builder.Config.BuildConfig.AppType.Assembly);
        //Scan StaticOnline assembly
        AddList(list, Assembly.GetExecutingAssembly());

        return list;
    }

    void AddList(List<FileGeneratorBase> list, Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAbstract)
                continue;
            if (type.IsAssignableTo(typeof(FileGeneratorBase)))
            {
                var file = (FileGeneratorBase)Activator.CreateInstance(type)!;
                file.Builder = builder;
                list.Add(file);
            }
        }

    }
}
