namespace Messages
{
    public class Command
    {
        public Environment Environment { get; set; } 
        public CommandType CommandType { get; set; }
        public int? DatabaseId { get; set; }

        public CacheEnum Cache { get; set; }
        public string Key { get; set; }
    }
}
