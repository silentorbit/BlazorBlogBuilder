using System.Xml.Linq;

namespace SilentOrbit.StaticOnline.Extensions;

static class XmlExtensions
{
    [return: NotNullIfNotNull(nameof(value))]
    public static XElement? AddElement(this XElement root, string name, string? value)
    {
        if (value == null)
            return null;

        var element = new XElement(root.Name.Namespace + name, value);
        root.Add(element);
        return element;
    }

    /// <summary>
    /// Get with '<?xml' header
    /// </summary>
    public static string ToUtf8String(this XElement element, bool xmlHeader)
    {
        var doc = new XDocument(new XDeclaration("1.0", "utf-8", null), element);

        if (xmlHeader)
        {
            var writer = new Utf8StringWriter();
            doc.Save(writer);
            return writer.ToString();
        }
        else
        {
            return element.ToString();
        }
    }

    class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}
