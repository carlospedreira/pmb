using PrivateMailboxNumber.ApiService.Models;

namespace PrivateMailboxNumber.ApiService.Repositories;

public interface IMailboxRepository
{
    MailboxAssignment? GetByLocationAndAccount(string locationNumber, string accountId);
    MailboxAssignment? GetByLocationAndMailbox(string locationNumber, int mailboxNumber);
    MailboxAssignment AssignMailbox(string locationNumber, string accountId);
    int GetNextMailboxNumber(string locationNumber);
} 