using backend.Models;
using backend.Models.Analytics;
using Microsoft.Extensions.Options;
using System.Xml.Linq;
using System.Xml.Serialization;
using UAParser;
using static backend.XML.XMLModel;

namespace backend.XML
{
    public class XMLMapper
    {
        private readonly string _xmlFile;

        public XMLMapper(IOptions<FilePaths> options)
        {
            _xmlFile = options.Value.XmlFile;
        }

        public LinkAnalyticsXml Map(Link link, DeviceInfo? device)
        {
            var xml = new LinkAnalyticsXml
            {
                Id = link.Id,
                Name = link.Name,
                LongURL = link.LongURL,
                ShortURL = link.ShortURL,
                ExpiryDate = link.ExpiryDate
            };

            if (device != null)
            {
                xml.Devices.Add(ToDeviceXml(device));
            }

            return xml;
        }

        public void AddDevices(LinkAnalyticsXml xml, IEnumerable<DeviceInfo> devices)
        {
            foreach (var device in devices)
            {
                xml.Devices.Add(ToDeviceXml(device));
            }
        }

        private static DeviceInfo ToDeviceXml(DeviceInfo device)
        {
            return new DeviceInfo
            {
                UserAgent = device.UserAgent,
                Browser = device.Browser,
                OperatingSystem = device.OperatingSystem,
                DeviceType = device.DeviceType
            };
        }

        public void Save(XMLModel xml)
        {
            var serializer = new XmlSerializer(typeof(XMLModel));

            using var stream = new FileStream(_xmlFile, FileMode.Create);
            serializer.Serialize(stream, xml);
        }
    }
}
