using SilentOrbit.StaticOnline.Building.FileGeneration;

namespace SilentOrbit.StaticOnline.Building;

/// <summary>
/// Generate other types of files
/// such as feeds and sitemaps
/// </summary>
class FileBuilder
{
    readonly SiteBuilder builder;

    public FileBuilder(SiteBuilder builder)
    {
        this.builder = builder;
    }

    FileGeneratorBase[] files = null!;

    public void Scan()
    {
        files = GetGenerators().ToArray();
        foreach (var file in files)
        {
            file.Init();
            builder.Pages.AddIndex(file);
        }
    }

    public async Task Generate()
    {
        foreach (var file in files)
        {
            var content = await file.Generate();
            if (content == null)
                continue;

            builder.Target.Store(file.URL, content);
            builder.Pages.DoneStatic(file.URL);
        }
    }

    public IEnumerable<FileGeneratorBase> GetGenerators()
    {
        //Scan site assmebly
        var types = builder.Config.BuildConfig.AppType.Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAssignableTo(typeof(FileGeneratorBase)))
            {
                var file = (FileGeneratorBase)Activator.CreateInstance(type)!;
                file.Builder = builder;
                yield return file;
            }
        }

        //Scan this assmebly
        types = GetType().Assembly.GetTypes();
        foreach (var type in types)
        {
            if (type.IsAbstract)
                continue;
            if (type.IsAssignableTo(typeof(FileGeneratorBase)))
            {
                var file = (FileGeneratorBase)Activator.CreateInstance(type)!;
                file.Builder = builder;
                yield return file;
            }
        }
    }
}
