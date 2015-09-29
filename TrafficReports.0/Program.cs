using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace TrafficReports1
{
    class Program
    {
        private const int MaxEvents = 10000;

        static void Main(string[] args)
        {
            var storageAccount = CloudStorageAccount.Parse(
                ConfigurationManager.ConnectionStrings["VegvesenEventStorage"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            Console.WriteLine("Reading Norwegian traffic incident data...");

            var container = blobClient.GetContainerReference("getsituation-events");
            var eventCount = ReadTrafficReportsAsync(container, MaxEvents).Result;

            Console.WriteLine("Retrieved {0} situation reports", eventCount);
        }

        private static async Task<int> ReadTrafficReportsAsync(CloudBlobContainer container, int maxEvents)
        {
            var count = 0;

            BlobContinuationToken dirToken = null;
            do
            {
                var dirResult = await container.ListBlobsSegmentedAsync(dirToken);
                dirToken = dirResult.ContinuationToken;
                foreach (var dirItem in dirResult.Results)
                {
                    if (dirItem is CloudBlobDirectory)
                    {
                        var dir = dirItem as CloudBlobDirectory;
                        BlobContinuationToken blobToken = null;
                        var blobResult = await dir.ListBlobsSegmentedAsync(blobToken);
                        foreach (var blobItem in blobResult.Results)
                        {
                            if (blobItem is CloudBlockBlob)
                            {
                                var blob = blobItem as CloudBlockBlob;
                                var content = await blob.DownloadTextAsync();
                                count++;
                                using (var writer = new StreamWriter(string.Format("{0:D5}.xml", count)))
                                {
                                    await writer.WriteAsync(content);
                                }
                                Console.WriteLine(count);
                                if (count >= maxEvents)
                                    break;
                            }
                        }
                    }
                    if (count >= maxEvents)
                        break;
                }
            } while (dirToken != null && count < maxEvents);
            return count;
        }
    }
}
