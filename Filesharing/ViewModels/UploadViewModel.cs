namespace Filesharing.ViewModels
{
    public class InputFile
    {
        public IFormFile File { get; set; } = null!;
    }

	public class InputUpload
	{
		public string OriginalFileName { get; set; } = string.Empty;
		public string FileName { get; set; } = string.Empty;
		public string ContentType { get; set; } = string.Empty;
		public long Size { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

	public class UploadViewModel
    {
        public int ID { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public decimal Size { get; set; }
        public string ContentType { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public long DownloadCount { get; set; }

    }
}
