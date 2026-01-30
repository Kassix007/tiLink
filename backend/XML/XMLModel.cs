using backend.Models.Analytics;
using System.Xml.Serialization;

namespace backend.XML
{
    public class XMLModel
    {
        [XmlRoot("LinkAnalytics")]
        public class LinkAnalyticsXml()
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;
            public string LongURL { get; set; } = string.Empty;
            public string ShortURL { get; set; } = string.Empty;
            public string ExpiryDate { get; set; } = string.Empty;

            [XmlArray("Devices")]
            [XmlArrayItem("Device")]
            public List<DeviceInfo> Devices { get; set; } = new();
        }
    }
}
