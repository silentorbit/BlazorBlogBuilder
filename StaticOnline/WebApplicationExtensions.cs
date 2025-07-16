namespace SilentOrbit.StaticOnline;

public static class WebApplicationExtensions
{
    /// <summary>
    /// Also add <see cref="MapStaticOnline(WebApplication)"/>
    /// </summary>
    public static void AddStaticOnline(this IServiceCollection services, SiteConfig config)
    {
        config.SiteBuilder ??= new SiteBuilder(config);

        services.AddSingleton<SiteConfig>(config);
        services.AddSingleton<SiteBuilder>(config.SiteBuilder);
        services.AddTransient<PageData>(config.SiteBuilder.TransientPage);
    }

    /// <summary>
    /// Place before
    /// <see cref="RazorComponentsEndpointRouteBuilderExtensions.MapRazorComponents"/>
    /// 
    /// Also add <see cref="AddStaticOnline(IServiceCollection, SiteBuilder)"/>
    /// </summary>
    public static void BuildStaticOnline(this WebApplication app, SiteConfig config)
    {
        //Feeds and sitemap
        var fr = new FileBuilder(config.SiteBuilder);
        foreach (var file in fr.GetGenerators())
        {
            app.MapGet(file.URL.Href, file.Generate);
        }

        //Start building once the webserver is running
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            await config.SiteBuilder.Build(app);
            
            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StaticOnline");
            logger.LogInformation("Generation complete");

            if (config.BuildConfig.ExitAfterBuildComplete)
                lifetime.StopApplication();
        });
    }

}
