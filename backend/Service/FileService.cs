using backend.Models;
using Microsoft.Extensions.Options;

namespace backend.Service
{
    public class FileService
    {
        private readonly string _path;
        private readonly string _csvFile;
        private static readonly object _lock = new();

        public FileService(IOptions<FilePaths> options)
        {
            _path = options.Value.FolderPath;
            _csvFile = options.Value.CsvFile;
        }

        //methods
        //to refactor proper initialisation
        private bool isFileValid()
        {
            if (!Directory.Exists(_path))
                Directory.CreateDirectory(_path);
            //problematic
            if (!System.IO.File.Exists(_csvFile))
            {
                return true;
            }

            return false;
        }

        public bool addToFile(Link link)
        {
            if (isFileValid())
            {
                lock (_lock)
                {
                    //using StreamWriter headerWriter = new StreamWriter(_csvFile, append: false);
                    if (!System.IO.File.Exists(_csvFile))
                    {
                        using StreamWriter headerWriter = new StreamWriter(_csvFile, append: false);
                        headerWriter.WriteLine("ID,Name,LongURL,ShortURL,ExpiryDate");
                    }

                    //refact
                    try
                    {
                        string id = Guid.NewGuid().ToString();
                        using StreamWriter writer = new StreamWriter(_csvFile, append: true); //write data to file
                        writer.WriteLine($"{id},{link.LongURL},{link.ShortURL},{link.ExpiryDate}");
                    }

                    catch (Exception ex)
                    {
                        throw;
                        //Console.WriteLine("Error");
                    }
                }
            }

            return true;
        }

        //readAllLinksFromCSV - return HashSet/List of Link

        //readAllLinksByID - return Link object

        //for these 2 methods, add 





        //to add other file manipulation methods
        //read write create update by ID
    }
}
