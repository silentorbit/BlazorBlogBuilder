namespace SilentOrbit.BlazorBlogBuilder.Components;

public partial class Comments
{
    string CommentLink()
    {
        var subject = Uri.EscapeDataString($"Comment on {_Page.Title}");
        
        var body = Uri.EscapeDataString(@$"Hi,

I loved your post at {_Page.URL}

Specifically ...

When posting my comment, please refer to me as: ...
");

        return $"mailto:{_Site.CommentEmail}?subject={subject}&body={body}";
    }

}
