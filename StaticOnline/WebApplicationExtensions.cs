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
        if (config.BaseURL.Href != "")
        {
            //Required for sites not located at root.
            app.UsePathBase(config.BaseURL.Href + '/');
        }

        //Feeds and sitemap
        app.Use(async (http, next) =>
        {
            var url = config.BaseURL.Append(http.Request.Path);
            var page = config.SiteBuilder.Pages.GetExisting(url);
            if (page?.Generator == null)
            {
                await next(http);
                return;
            }

            var content = await page.Generator.Generate(url);
            await http.Response.WriteAsync(content);
        });

        //Images
        app.MapGet("media/{filename}", (string filename) => config.SiteBuilder.Image.MapGetContent(filename));

        //Start building once the webserver is running
        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            var stopwatch = Stopwatch.StartNew();

            var url = app.Urls.First() + config.BaseURL.Href + "/";

            await config.SiteBuilder.Build(url);

            stopwatch.Stop();

            var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("StaticOnline");
            logger.LogInformation($"Generation complete in {stopwatch.Elapsed.TotalSeconds:0.00} seconds");

            switch (config.BuildConfig.AfterBuild)
            {
                case AfterBuildConfig.Exit:
                    lifetime.StopApplication();
                    break;

                case AfterBuildConfig.LaunchBrowser:
                    var psi = new ProcessStartInfo(url) { UseShellExecute = true };
                    Process.Start(psi);
                    break;

                case AfterBuildConfig.KeepRunning:
                    break;

                default:
                    throw new NotImplementedException(config.BuildConfig.AfterBuild.ToString());
            }
        });
    }

}
