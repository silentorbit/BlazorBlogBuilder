using System.Text.Encodings.Web;
using System.Text.Json;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

class JsonFeed : FeedGeneratorBase
{
    protected override string Filename { get; } = "feed.json";
    protected override string MimeType { get; } = "application/feed+json";

    JsonSerializerOptions options = new JsonSerializerOptions()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingDefault,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    protected override async Task<string> GenerateFeed(RelUrl url, IEnumerable<PageData> posts)
    {
        var feed = new FeedData();
        feed.title = Config.Title;
        feed.description = Config.Description;
        feed.home_page_url = Config.BaseURL;
        feed.feed_url = url;
        feed.authors = Generate(Config.Author);

        foreach (var post in posts)
        {
            var content_html = await GetPostContent(post);
            feed.items.Add(new()
            {
                id = post.ID ?? post.URL,
                title = post.Title,
                url = post.URL,
                authors = Generate(post.Author),
                date_published = post.Published?.ToRFC3339(),
                date_modified = post.Modified?.ToRFC3339(),
                //One of the content is required.
                content_html = content_html,
                content_text = content_html == null ? "" : null,
            });
        }
        return JsonSerializer.Serialize(feed, options);
    }

    [return: NotNullIfNotNull(nameof(author))]
    AuthorData[]? Generate(Author? author)
    {
        if (author == null)
            return null;

        var ad = new AuthorData
        {
            name = author.Name,
            url = author.Email,
            //avatar =
        };
        if (author.Email != null)
            ad.url = "mailto:" + author.Email;
        return [ad];
    }

    /// <summary>
    /// https://www.jsonfeed.org/version/1.1/
    /// </summary>
    class FeedData
    {
        public string version { get; set; } = "https://jsonfeed.org/version/1.1";

        public string? title { get; set; }

        public string? description { get; set; }

        public string? home_page_url { get; set; }

        public string? feed_url { get; set; }
        public AuthorData[]? authors { get; set; }

        public List<ItemData> items { get; set; } = new();

        //TODO
        public string? next_url { get; set; }
        public string? icon { get; set; }
        public string? favicon { get; set; }
        public string? language { get; set; }
        public bool expired { get; set; }
        public HubData[]? hubs { get; set; }
    }

    class ItemData
    {
        public string id { get; set; } = null!;
        public string? url { get; set; }
        public string? title { get; set; }
        public string? date_published { get; set; } = null!;
        public string? date_modified { get; set; }

        //TODO
        public string? external_url { get; set; }
        public string? content_html { get; set; }
        public string? content_text { get; set; }
        public string? summary { get; set; }
        public string? image { get; set; }
        public string? banner_image { get; set; }
        public AuthorData[]? authors { get; set; }
        public string[]? tags { get; set; }
        public string? language { get; set; }
        public AttachmentData[]? attachments { get; set; }
    }

    class AuthorData
    {
        public string? name { get; set; }
        public string? url { get; set; }
        public string? avatar { get; set; }
    }

    class HubData
    {
        public string type { get; set; } = null!;
        public string url { get; set; } = null!;
    }

    class AttachmentData
    {
        public string url { get; set; } = null!;
        public string mime_type { get; set; } = null!;
        public string? title { get; set; }
        public long size_in_bytes { get; set; }
        public int duration_in_seconds { get; set; }
    }

}
