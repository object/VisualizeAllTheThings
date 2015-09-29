using System;
using System.Xml.Linq;

namespace TrafficReports
{
    class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    class Situation
    {
        public string SituationId { get; set; }
        public string Version { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime VersionTime { get; set; }
        public string Severity { get; set; }
        public string Probability { get; set; }
        public Location Location { get; set; }
        public string RoadNumber { get; set; }
        public int? CountyNumber { get; set; }
        public int? MunicipalityNumber { get; set; }

        public static Situation Parse(XDocument doc)
        {
            var ns = XNamespace.Get("http://datex2.eu/schema/2/2_0");

            var situation = new Situation();
            var root = doc.Element(ns + "situation");
            var situationRecord = root.Element(ns + "situationRecord");
            situation.SituationId = situationRecord.Attribute("id").Value;
            situation.Version = situationRecord.Attribute("version").Value;
            situation.CreationTime = DateTime.Parse(situationRecord.Element(ns + "situationRecordCreationTime").Value);
            situation.VersionTime = DateTime.Parse(situationRecord.Element(ns + "situationRecordVersionTime").Value);
            situation.Severity = root.Element(ns + "overallSeverity").Value;
            situation.Probability = situationRecord.Element(ns + "probabilityOfOccurrence").Value;
            var location = situationRecord
                .Element(ns + "groupOfLocations")
                .Element(ns + "locationForDisplay");
            situation.Location = new Location()
            {
                Latitude = double.Parse(location.Element(ns + "latitude").Value),
                Longitude = double.Parse(location.Element(ns + "longitude").Value),
            };
            var locationExtension = situationRecord
                .Element(ns + "groupOfLocations")
                .Element(ns + "locationExtension")
                .Element(ns + "locationExtension");
            situation.RoadNumber = locationExtension.Element(ns + "roadNumber").Value;
            situation.CountyNumber = ParseInt(locationExtension.Element(ns + "countyNumber"));
            situation.MunicipalityNumber = ParseInt(locationExtension.Element(ns + "municipalityNumber"));

            return situation;
        }

        private static int? ParseInt(XElement element)
        {
            if (element == null || element.Value == null)
                return null;
            else
                return int.Parse(element.Value);
        }
    }
}
