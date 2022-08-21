namespace BlogEngine.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CollectionName { get; set; }
        public string CollectionName1 { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IDatabaseSettings
    {
        string CollectionName { get; set; }
        string CollectionName1 { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
