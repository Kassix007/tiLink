using backend.Models;
using backend.Models.Analytics;
using Microsoft.Extensions.Options;
using System.Xml.Serialization;
using static backend.XML.XMLModel;

namespace backend.XML
{
    public class XMLMapper
    {
        private readonly string _xmlFile;
        private static readonly object _fileLock = new();

        public XMLMapper(IOptions<FilePaths> options)
        {
            var projectRoot = Directory.GetCurrentDirectory();
            _xmlFile = Path.Combine(projectRoot, options.Value.XmlFile);
            Directory.CreateDirectory(Path.GetDirectoryName(_xmlFile)!);

            _xmlFile = options.Value.XmlFile;
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
                        ExpiryDate = link.ExpiryDate
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

        private void Save(LinkAnalyticsCollection collection)
        {
            var serializer = new XmlSerializer(typeof(LinkAnalyticsCollection));

            using var stream = new FileStream(_xmlFile, FileMode.Create);
            serializer.Serialize(stream, collection);
        }
    }
}
