using System.Collections.Concurrent;
using PrivateMailboxNumber.ApiService.Models;

namespace PrivateMailboxNumber.ApiService.Repositories;

public class InMemoryMailboxRepository : IMailboxRepository
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<int, MailboxAssignment>> _mailboxesByLocation = new();
    private readonly ConcurrentDictionary<string, int> _lastMailboxNumberByLocation = new();
    private readonly object _lockObject = new();

    public MailboxAssignment? GetByLocationAndAccount(string locationNumber, string accountId)
    {
        return _mailboxesByLocation.TryGetValue(locationNumber, out var locationMailboxes)
            ? locationMailboxes.Values.FirstOrDefault(m => m.AccountId == accountId)
            : null;
    }

    public MailboxAssignment? GetByLocationAndMailbox(string locationNumber, int mailboxNumber)
    {
        return _mailboxesByLocation.TryGetValue(locationNumber, out var locationMailboxes)
            ? locationMailboxes.TryGetValue(mailboxNumber, out var assignment) ? assignment : null
            : null;
    }

    public MailboxAssignment AssignMailbox(string locationNumber, string accountId)
    {
        var mailboxNumber = GetNextMailboxNumber(locationNumber);
        var assignment = new MailboxAssignment
        {
            LocationNumber = locationNumber,
            AccountId = accountId,
            MailboxNumber = mailboxNumber
        };

        var locationMailboxes = _mailboxesByLocation.GetOrAdd(locationNumber, _ => new ConcurrentDictionary<int, MailboxAssignment>());
        if (!locationMailboxes.TryAdd(mailboxNumber, assignment))
        {
            throw new InvalidOperationException($"Failed to assign mailbox number {mailboxNumber} for location {locationNumber}");
        }

        return assignment;
    }

    public int GetNextMailboxNumber(string locationNumber)
    {
        lock (_lockObject)
        {
            var nextNumber = _lastMailboxNumberByLocation.GetOrAdd(locationNumber, _ => 0) + 1;
            _lastMailboxNumberByLocation[locationNumber] = nextNumber;
            return nextNumber;
        }
    }
} 