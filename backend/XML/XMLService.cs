using backend.Models;
using backend.Models.Analytics;
using backend.Service;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using static backend.XML.XMLModel;

namespace backend.XML
{
    public class XMLService
    {
        private readonly string _xmlFile;
        private static readonly object _fileLock = new();
        private readonly DeviceService _deviceService;

        public XMLService(IOptions<FilePaths> options, DeviceService deviceService)
        {
            var projectRoot = Directory.GetCurrentDirectory();
            _xmlFile = Path.Combine(projectRoot, options.Value.XmlFile);
            Directory.CreateDirectory(Path.GetDirectoryName(_xmlFile)!);

            _xmlFile = options.Value.XmlFile;
            _deviceService = deviceService;
        }

        public LinkAnalyticsXml CreateAndSaveLink(string longUrl)
        {
            var code = Guid.NewGuid().ToString("N")[..6];

            var link = new LinkAnalyticsXml
            {
                Id = Guid.NewGuid(),
                LongURL = longUrl,
                ShortURL = code,
                ExpiryDate = "2050",
                CreatedTimestamp = DateTime.UtcNow
            };

            var collection = LoadOrCreate();
            collection.Links.Add(link);
            Save(collection);

            return link;
        }

        public void TrackDevice(string code)
        {
            string ipAddress = _deviceService.GetClientIp();

            DeviceInfo info = _deviceService.GetDeviceInfo();

            var collection = LoadOrCreate();
            var link = collection.Links.FirstOrDefault(x => x.ShortURL == code) ?? new LinkAnalyticsXml();

            AppendDevice(link, info);
        }

        public void SaveOrAppend(Link link, DeviceInfo? device = null)
        {
            lock (_fileLock)
            {
                var collection = LoadOrCreate();

                var existingLink = collection.Links
                    .FirstOrDefault(l => l.Id == link.Id);

                if (existingLink == null)
                {
                    existingLink = new LinkAnalyticsXml
                    {
                        Id = link.Id,
                        Name = link.Name,
                        LongURL = link.LongURL,
                        ShortURL = link.ShortURL,
                        ExpiryDate = link.ExpiryDate,
                        CreatedTimestamp = link.CreatedTimestamp
                    };

                    collection.Links.Add(existingLink);
                }

                existingLink.Devices.Add(device);

                Save(collection);
            }
        }

        public void AppendDevice(LinkAnalyticsXml xmlNode, DeviceInfo device)
        {
            lock (_fileLock)
            {
                var collection = LoadOrCreate();

                var existing = collection.Links.FirstOrDefault(x => x.Id == xmlNode.Id);
                if (existing != null)
                    existing.Devices.Add(device);
                Save(collection);
            }
        }


        public LinkAnalyticsCollection LoadOrCreate()
        {
            if (!File.Exists(_xmlFile))
            {
                return new LinkAnalyticsCollection();
            }

            var serializer = new XmlSerializer(typeof(LinkAnalyticsCollection));

            using var stream = new FileStream(_xmlFile, FileMode.Open);
            return (LinkAnalyticsCollection)serializer.Deserialize(stream)!;
        }

        public void Save(LinkAnalyticsCollection collection)
        {
            var serializer = new XmlSerializer(typeof(LinkAnalyticsCollection));

            using var stream = new FileStream(_xmlFile, FileMode.Create);
            serializer.Serialize(stream, collection);
        }
    }
}
