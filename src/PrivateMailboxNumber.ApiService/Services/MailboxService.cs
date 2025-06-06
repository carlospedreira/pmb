using PrivateMailboxNumber.ApiService.Repositories;

namespace PrivateMailboxNumber.ApiService.Services;

public class MailboxService : IMailboxService
{
    private readonly IMailboxRepository _repository;

    public MailboxService(IMailboxRepository repository)
    {
        _repository = repository;
    }

    public int GetMailboxNumber(string locationNumber, string accountId)
    {
        var existing = _repository.GetByLocationAndAccount(locationNumber, accountId);
        if (existing != null)
        {
            return existing.MailboxNumber;
        }

        var assignment = _repository.AssignMailbox(locationNumber, accountId);
        return assignment.MailboxNumber;
    }

    public string GetAccountId(string locationNumber, int mailboxNumber)
    {
        var assignment = _repository.GetByLocationAndMailbox(locationNumber, mailboxNumber);
        if (assignment == null)
        {
            throw new KeyNotFoundException($"No mailbox assignment found for location {locationNumber} and mailbox number {mailboxNumber}");
        }

        return assignment.AccountId;
    }
} 