namespace BlogEngine.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string CollectionName { get; set; }
        public string CollectionName1 { get; set; }
        public string CollectionName2 { get; set; }
        public string CollectionName3 { get; set; }
        public string CollectionName4 { get; set; }
        public string CollectionName5 { get; set; }
        public string CollectionName6 { get; set; }
        public string CollectionName7 { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IDatabaseSettings
    {
        string CollectionName { get; set; }
        string CollectionName1 { get; set; }
        string CollectionName2 { get; set; }
        string CollectionName3 { get; set; }
        string CollectionName4 { get; set; }
        string CollectionName5 { get; set; }
        string CollectionName6 { get; set; }
        string CollectionName7 { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
