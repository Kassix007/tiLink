using System.Net;

namespace backend.BL
{
    public class URLShortener
    {
        public static readonly string shortenedUrlFolderPath = @"C:\tiLink\TestFolder\"; //define folder path

        public static readonly string shortenedUrlFilePath = Path.Combine(shortenedUrlFolderPath, "ShortenedOnlyTest.csv"); //define csv file

        private static readonly object _lock = new();

        public void UseRetrieveText(string csvText) 
        {
            string shortUrl = Shorten(csvText);
            Console.WriteLine(shortUrl);
            StoreShortenedUrl(shortUrl);
        }

        public string Shorten(string longUrl)
        {
            string apiUrl = $"https://tinyurl.com/api-create.php?url={Uri.EscapeDataString(longUrl)}";

            //httpClient
            using WebClient client = new WebClient();
            return client.DownloadString(apiUrl);
        }

        public void StoreShortenedUrl(string shortUrl)
        {
            Directory.CreateDirectory(shortenedUrlFolderPath);

            lock (_lock)
            {

                if (!System.IO.File.Exists(shortenedUrlFilePath))
                {
                    using StreamWriter headerWriter = new StreamWriter(shortenedUrlFilePath, append: false);
                }

                try
                {
                    using StreamWriter writer = new StreamWriter(shortenedUrlFilePath, append: true); //write data to file
                    writer.WriteLine($"{shortUrl}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                }
            }
        }
    }
}
