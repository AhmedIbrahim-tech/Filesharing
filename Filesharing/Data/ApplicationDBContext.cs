using Microsoft.EntityFrameworkCore;
using Filesharing.Models;

namespace Filesharing.Data
{
    public class ApplicationDBContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Upload> Uploads { get; set; }

        public DbSet<Contact> Contacts { get; set; }
        
    }
}
