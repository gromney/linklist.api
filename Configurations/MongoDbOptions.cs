namespace LinkList.api.Configurations
{
    public class MongoDbOptions
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string LinkListCollection { get; set; }
        // public string UrlCollectionName { get; set; }
    }
}