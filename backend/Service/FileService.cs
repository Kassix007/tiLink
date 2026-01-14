using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using static System.IO.StreamWriter;
using System.Globalization;

namespace backend.Service
{
    public class FileService //: FileManipulation
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

        public HashSet<Link> GetLongUrlsById()
        {
            var links = new HashSet<Link>();

            if (!File.Exists(_csvFile))
                return links;

            foreach (var line in File.ReadLines(_csvFile).Skip(1)) // skip header
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                var link = new Link
                {
                    Id = Guid.Parse(parts[0]),
                    Name = parts[1],
                    LongURL = parts[2],
                    ShortURL = parts[3],
                    ExpiryDate = parts[4],
                };

                links.Add(link); // ✅ HashSet enforces unique ShortURL
            }

            return links;
        }


        //readAllLinksByID - return Link object
        public Link GetLongUrlById(Guid id)
        {
            var link = new Link();

            if (!File.Exists(_csvFile))
                return link;

            foreach (var line in File.ReadLines(_csvFile).Skip(1)) // skip header
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length < 5)
                    continue;

                if (!Guid.TryParse(parts[0].Trim(), out var rowId))
                    continue;

                if (rowId == id)
                {
                    link.Id = Guid.Parse(parts[0].Trim());
                    link.Name = parts[1].Trim();
                    link.LongURL = parts[2].Trim();
                    link.ShortURL = parts[3].Trim();
                    link.ExpiryDate = parts[4].Trim();
                }
            }
            return link;
        }
    }
}
