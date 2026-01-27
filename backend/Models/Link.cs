namespace backend.Models
{
    public class Link
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string LongURL { get; set; }
        public string ShortURL { get; set; }
        public string ExpiryDate { get; set; }

        public override bool Equals(object? obj)
    => obj is Link l && ShortURL == l.ShortURL;

        public override int GetHashCode()
            => ShortURL.GetHashCode();
    }
}


// XML -> Read/write
// Use Link Object to map to XML
// Link -> many small DeviceInfo Objects