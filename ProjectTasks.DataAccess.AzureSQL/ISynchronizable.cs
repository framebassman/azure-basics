namespace ProjectTasks.DataAccess.AzureSQL;

public interface ISynchronizable
{
    public bool WasSynchronized { get; set; }
}
