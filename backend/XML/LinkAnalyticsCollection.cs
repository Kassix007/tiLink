using System.Xml.Serialization;
using static backend.XML.XMLModel;

namespace backend.XML
{
    [XmlRoot("LinkAnalyticsCollection")]
    public class LinkAnalyticsCollection
    {
        [XmlElement("LinkAnalytics")]
        public List<LinkAnalyticsXml> Links { get; set; } = new();
    }
}
