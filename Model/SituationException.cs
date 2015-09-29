using System;

namespace TrafficReports
{
    class SituationException : Exception
    {
        public Situation Situation { get; set; }
    }
}
