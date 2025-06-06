namespace PrivateMailboxNumber.ApiService.Services;

public interface IMailboxService
{
    int GetMailboxNumber(string locationNumber, string accountId);
    string GetAccountId(string locationNumber, int mailboxNumber);
} 