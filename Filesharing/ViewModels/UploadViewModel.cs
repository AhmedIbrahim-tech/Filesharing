namespace Filesharing.ViewModels
{
    public class InputFile
    {
        public IFormFile File { get; set; }
    }

	public class InputUpload
	{
		public string OriginalFileName { get; set; }
		public string FileName { get; set; }
		public string ContentType { get; set; }
		public long Size { get; set; }
        public string UserId { get; set; }
    }

	public class UploadViewModel
    {
        public int ID { get; set; }
        public string OriginalFileName { get; set; }
        public string FileName { get; set; }
        public decimal Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadDate { get; set; }
        public long DownloadCount { get; set; }

    }
}
