namespace SageSupervisor.Models;

public class DocChangeEventArgs(string recordId, TableChangeType changeType, DateTime timestamp, int domaine, int type) : EventArgs
{
    public string RecordId { get; } = recordId;
    public TableChangeType ChangeType { get; } = changeType;
    public DateTime Timestamp { get; } = timestamp;
    public int Domaine { get; } = domaine;
    public int Type { get; } = type;
}
