namespace Filesharing.Models
{
    public class Upload
    {
        [Key]
        public int ID { get; set; }

        [Display(Name = "Original File Name")]
        public string? OriginalFileName { get; set; }

        [Display(Name = "File Name")]
        public string? FileName { get; set; }

        [Display(Name = "Contant Type")]
        public string? ContantType { get; set; }
        public decimal Size { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }

        [Display(Name = "User")]
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

        public long DownloadCount { get; set; }
    }
}
