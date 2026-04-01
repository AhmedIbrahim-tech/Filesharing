namespace Filesharing.Models;

public class Contact
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public virtual IdentityUser? User { get; set; }
}
