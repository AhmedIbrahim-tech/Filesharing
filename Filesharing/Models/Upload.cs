namespace Filesharing.Models
{
    public class Upload
    {
        /*
         * To Make ID is index Number And Primary Key
         * 
         * public string ID { get; set; }
            public Upload()
            {
                ID = Guid.NewGuid().ToString();
            }
        */
        
        [Key]
        public int ID { get; set; }

        [Display(Name = "Original File Name")]
        public string OriginalFileName { get; set; }

        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "Content Type")]
        public string ContentType { get; set; }
        public decimal Size { get; set; }

        [Display(Name = "Upload Date")]
        public DateTime UploadDate { get; set; }   = DateTime.Now;

        [Display(Name = "User")]
        public string UserId { get; set; }

        // To get All Uploads by User Id
        public IdentityUser User { get; set; }

        public long DownloadCount { get; set; }
    }
}
