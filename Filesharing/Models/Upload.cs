namespace Filesharing.Models;

public class Upload
{
    public int ID { get; set; }
    public string OriginalFileName { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public decimal Size { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.Now;
    public string UserId { get; set; } = null!;
    public IdentityUser? User { get; set; }
    public long DownloadCount { get; set; }
}
