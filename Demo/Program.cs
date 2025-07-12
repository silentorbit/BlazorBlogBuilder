using Demo.Components;
using SilentOrbit.Disk;
using SilentOrbit.StaticOnline;
using SilentOrbit.StaticOnline.Config;

//StaticOnline
var config = new SiteConfig<App>
{
    Language = new() { Code = "en" },
    Title = "Demo",
    Description = "This is a Demo",
    //Build static files to be served in the subdirectory
    BaseURL = "https://www.silentorbit.com/static-online",
    Favicon = new()
    {
        URL = "https://www.silentorbit.com/favicon.png",
        MimeType = "image/png"
    },
    CommentEmail = "demo@silentorbit.com",
    DefaultRobots = new()
    {
        NoTranslate = true,
        Sitemap = true,
    },
    Author = new()
    {
        Name = "Silent Orbit",
    },
    Markdown = new(true),
};
config.TopMenu = new() {
    new() { Title = "Blog",     Link = config.BaseURL.Append("/")    },
    new() { Title = "Tags",     Link = config.BaseURL.Append("/tags")    },
    new() { Title = "Count 10", Link = config.BaseURL.Append("/mod10")    }
};

//For building from a GitHub Action
if (args.FirstOrDefault() == "build" && args.Length == 2)
{
    config.Target = DirPath.GetCurrentDirectory().CombineDir(args[1]);
    config.ExitAfterBuildComplete = true;
}
else
{
    //Generation target and options
    config.Target = new DirPath("../generated/");
    //config.ExitAfterBuildComplete = true;
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents();
//Static Online: inject SiteBuilder and SitePage
builder.Services.AddStaticOnline(config);

var app = builder.Build();

app.UsePathBase(config.BaseURL.Href.TrimEnd('/') + '/');

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>();

//Static Online: Serve generated feeds and sitemaps
app.BuildStaticOnline(config);

app.Run();
