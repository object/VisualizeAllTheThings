using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace TrafficReports
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 1)
            {
                Console.WriteLine("Usage: TrafficReports <data_folder>");
                return;
            }

            var dir = args[0];
            if (!Directory.Exists(dir))
            {
                Console.WriteLine("Directory {0} does not exist", dir);
                return;
            }

            var files = Directory.EnumerateFiles(args[0]);
            Console.WriteLine("Reading {0} traffic events", files.Count());

            ReadTrafficEvents(files);

            Console.WriteLine("Done!");
        }

        static void ReadTrafficEvents(IEnumerable<string> filenames)
        {
            var logger = new Logger();
            var random = new Random();
            foreach (var filename in filenames)
            {
                using (var reader = new StreamReader(filename))
                {
                    var situation = Situation.Parse(XDocument.Load(reader));
                    logger.Debug("Time: {0:u}, Id: {1}, version: {2}, roadNumber: {3}, location: ({4},{5})",
                        situation.VersionTime,
                        situation.SituationId,
                        situation.Version,
                        situation.RoadNumber,
                        situation.Location.Latitude,
                        situation.Location.Longitude);
                    if (random.Next(100) == 0)
                        logger.Error(new SituationException() {Situation = situation}, "Invalid situation information");
                    Console.Write(".");
                }
            }
            Console.WriteLine();
        }
    }
}
