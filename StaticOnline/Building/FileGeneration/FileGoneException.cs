using System.Net;

namespace SilentOrbit.StaticOnline.Building.FileGeneration;

/// <summary>
/// Inform that the file generated is gone and should not be saved.
/// For example feeds for tags that only exist in drafts.
/// </summary>
class FileGoneException : Exception
{
    public const HttpStatusCode HttpStatusCodeGone = HttpStatusCode.Gone;
}
