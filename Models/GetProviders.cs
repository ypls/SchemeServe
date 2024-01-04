using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using System.Text;

namespace SchemeServe.Models
{
    public class GetProviders
    {
        private readonly IConfiguration _configuration;
        private static string cqc_Url;
        private static string test_Blob_URL;
        public GetProviders(IConfiguration config)
        {
            _configuration = config;
            cqc_Url = _configuration.GetConnectionString("CQC_Url");
            test_Blob_URL = _configuration.GetConnectionString("test_Blob_URL");
        }

        //Get provider
        public async Task<string> ProvidersAsync()
        {
            //get the cached providers file from Azure blob 
            var response = await GetFromBlobAsync();

            //if cached providers file does not exist, call CQC API
            if (response is null)
            {
                response = await CqcAPIAsync();
            }

            return response;
        }

        //get the cached providers from Azure blob
        public async Task<string> GetFromBlobAsync()
        {
            string results = null;

            try
            {
                //Get json file from blob
                BlobContainerClient categoryStorageClient = new BlobContainerClient(new Uri(test_Blob_URL), null);
                BlobClient categoryBlobClient = categoryStorageClient.GetBlobClient("providers.json");

                //check if the file exists on blob
                if (!categoryBlobClient.Exists())
                {
                    Console.WriteLine("providers.json" + " does not exist on blob !");
                    return null;
                }

                //If the data date is over 1 month old from todays date just return
                var lastModified = categoryBlobClient.GetProperties().Value.LastModified;
                var timenow = DateTimeOffset.Now;
                var difference = timenow - lastModified;
                var totalDays = difference.TotalDays;
                if (totalDays > 30.0)
                {
                    return null;
                }

                //download content
                BlobDownloadResult downloadResult = await categoryBlobClient.DownloadContentAsync();
                results = downloadResult.Content.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("GetFromBlobAsync Error: {0}\n", e);
            }

            return results;
        }

        //Call CQC API
        public async Task<string> CqcAPIAsync()
        {
            string results = "";

            try
            {
                var client = new HttpClient();
                var response = await client.GetAsync(cqc_Url);

                if (response is not null && response.StatusCode.ToString() == "OK")
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    //insert into Azure blob for cache
                    await InsertBlob(responseBody);

                    results = responseBody;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CqcAPIAsync Error: {0}\n", e);
            }

            return results;
        }

        //Insert into Azure blob for cache
        public async Task InsertBlob(string responseBody)
        {
            try
            {
                string fileName = "providers.json";

                BlobContainerClient storageClient = new BlobContainerClient(new Uri(test_Blob_URL), null);
                BlobClient blob = storageClient.GetBlobClient(fileName);

                var content = Encoding.UTF8.GetBytes(responseBody);
                using (var ms = new MemoryStream(content))
                {
                    await blob.UploadAsync(ms, true);
                }

                Console.WriteLine("- Inserted json into blob: {0}", fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine("InsertBlob error: " + e);
            }
        }
    }
}
