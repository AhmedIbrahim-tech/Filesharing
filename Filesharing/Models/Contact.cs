using System.ComponentModel.DataAnnotations.Schema;

namespace Filesharing.Models
{
    public class Contact
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
