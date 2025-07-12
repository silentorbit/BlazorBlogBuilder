namespace SilentOrbit.StaticOnline.Config.Data;

public class MenuItem
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; } 
    public RelUrl Link { get; set; } = null!;
}