using backend.Models;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using static System.IO.StreamWriter;
using System.Globalization;
using backend.Models.Analytics;

namespace backend.Service
{
    public class FileService //: FileManipulation
    {
        private readonly string _path;
        private readonly string _csvFile;
        private static readonly object _lock = new();
        private readonly string _analyticsCsv;

        public FileService(IOptions<FilePaths> options)
        {
            _path = options.Value.FolderPath;
            _csvFile = options.Value.CsvFile;
            _analyticsCsv = options.Value.AnalyticsCsv;
        }

        //methods
        //to refactor proper initialisation
        private bool IsFileValid()
        {
            if (!Directory.Exists(_path))
            Directory.CreateDirectory(_path);

            if (!File.Exists(_csvFile))
            {
                using var writer = new StreamWriter(_csvFile, append: false);
                writer.WriteLine("ID,Name,LongURL,ShortURL,ExpiryDate");
            }

            if (!File.Exists(_analyticsCsv))
            {
                using var writer = new StreamWriter(_analyticsCsv, append: false);
                writer.WriteLine("ID,UserAgent,Browser,OperatingSystem,DeviceType");
            }
            
            return File.Exists(_csvFile) && File.Exists(_analyticsCsv);
        }

        public bool AddShortenedLinkToFile(Link link)
        {
            if (IsFileValid())
            {
                lock (_lock)
                {
                    string id = Guid.NewGuid().ToString();

                    using var writer = new StreamWriter(_csvFile, append: true);
                    writer.WriteLine($"{id},{link.Name},{link.LongURL},{link.ShortURL},{link.ExpiryDate}");
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

        public bool AddDeviceInfoToFile(DeviceInfo deviceInfo, string id)
        {
            if (IsFileValid())
            {
                lock (_lock)
                {
                    using var writer = new StreamWriter(_analyticsCsv, append: true);
                    writer.WriteLine($"{id},{deviceInfo.IPAddress},{deviceInfo.UserAgent},{deviceInfo.Browser},{deviceInfo.OperatingSystem},{deviceInfo.DeviceType}");
                }
            }
            return true;
        }
    }
}
