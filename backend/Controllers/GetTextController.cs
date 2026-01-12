using backend.BL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography.X509Certificates;

namespace backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GetTextController : ControllerBase
    {
        public static readonly string folderPath = @"C:\tiLink\TestFolder\"; //define folder path

        public static readonly string filePath = Path.Combine(folderPath, "Test.csv"); //define csv file

        private static readonly object _lock = new();

        [HttpPost("retrieve-text")]
        public void retrieveText(string csvText)
        {
            IOptions<FilePaths> options = null;
            CreateFileStorage createFileStorage = new CreateFileStorage(options);
            createFileStorage.CreateCsv();

            string id = Guid.NewGuid().ToString();

            Directory.CreateDirectory(folderPath);

            lock (_lock)
            {

                if (!System.IO.File.Exists(filePath))
                {
                    using StreamWriter headerWriter = new StreamWriter(filePath, append: false);
                    headerWriter.WriteLine("GUID,Data");
                }

                try
                {
                    using StreamWriter writer = new StreamWriter(filePath, append: true); //write data to file
                    writer.WriteLine($"{id},{csvText}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error");
                }
            }

            URLShortener urlShortener = new URLShortener();
            urlShortener.UseRetrieveText(csvText);
        }
    }
}
