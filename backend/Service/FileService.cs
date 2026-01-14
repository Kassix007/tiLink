using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using static System.IO.StreamWriter;

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
        private bool IsFileValid()
        {
            Directory.CreateDirectory(_path);
            return File.Exists(_csvFile);
        }

        public bool AddToFile(Link link)
        {
            if (IsFileValid())
            {
                lock (_lock)
                {
                    if (!System.IO.File.Exists(_csvFile))
                    {
                        StreamWriter headerWriter = new StreamWriter(_csvFile, append: false);
                        headerWriter.WriteLine("ID,Name,LongURL,ShortURL,ExpiryDate");
                    }

                    //refact
                    try
                    {
                        string id = Guid.NewGuid().ToString();

                        using (var writer = new StreamWriter(_csvFile, append: true))
                        {
                            writer.WriteLine($"{id},{link.Name},{link.LongURL},{link.ShortURL},{link.ExpiryDate}");
                        }
                    }

                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            }
            return true;
        }
    }
}
