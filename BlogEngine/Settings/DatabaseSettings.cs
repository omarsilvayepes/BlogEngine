namespace BlogEngine.Settings
{
    public class DatabaseSettings : IDatabaseSettings
    {
        public string PostCollection { get; set; }
        public string UserCollection { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
    public interface IDatabaseSettings
    {
        string PostCollection { get; set; }
        string UserCollection { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
