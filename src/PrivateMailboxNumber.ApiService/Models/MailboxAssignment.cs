namespace PrivateMailboxNumber.ApiService.Models;

public class MailboxAssignment
{
    public string LocationNumber { get; set; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public int MailboxNumber { get; set; }
} 