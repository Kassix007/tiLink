namespace backend.Service
{
    public class UrlStore
    {
        private readonly Dictionary<string, string> _store = new();

        public void Save(string code, string longUrl)
        {
            _store[code] = longUrl;
        }

        public string? Get(string code)
        {
            _store.TryGetValue(code, out var url);
            return url;
        }
    }

}
