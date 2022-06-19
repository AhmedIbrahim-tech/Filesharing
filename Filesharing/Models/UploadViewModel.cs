namespace Filesharing.Models
{
    public class InputUpolad
    {
        public IFormFile File { get; set; }
    }

    public class UploadViewModel
    {
        public int ID { get; set; }
        public string? OriginalFileName { get; set; }
        public string? FileName { get; set; }
        public decimal? Size { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public long DownloadCount { get; set; }

    }
}
