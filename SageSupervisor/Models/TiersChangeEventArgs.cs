namespace SageSupervisor.Models;

public class TiersChangeEventArgs(string recordId, TableChangeType changeType, DateTime timestamp, int type) : EventArgs
{
    public string RecordId { get; } = recordId;
    public TableChangeType ChangeType { get; } = changeType;
    public DateTime Timestamp { get; } = timestamp;
    public int Type { get; } = type;
}