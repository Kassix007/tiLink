namespace backend.Models
{
    public class Link
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LongURL { get; set; }
        public string ShortURL { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
