using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Filesharing.Data;

public class ApplicationUser : IdentityUser
{
    public string Firstname { get; set; }
    public string Lastname { get; set; }
}
