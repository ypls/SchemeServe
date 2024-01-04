using Newtonsoft.Json;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SchemeServe.Models
{
    public class Individual
    {
        private readonly IConfiguration _configuration;
        private static string cqc_Url;
        private static string test_Blob_URL;
        public Individual(IConfiguration config)
        {
            _configuration = config;
            cqc_Url = _configuration.GetConnectionString("CQC_Url");
            test_Blob_URL = _configuration.GetConnectionString("test_Blob_URL");
        }

        //Get provider
        public async Task<Cqc> ProviderAsync(string provider_id)
        {
            //get the cached provider from Azure blob 
            Cqc response = await GetFromBlobAsync(provider_id);

            //if cached provider does not exist, call CQC API
            if (response is null)
            {
                response = await CqcAPIAsync(provider_id);
            }

            return response;
        }

        //get the cached provider from Azure blob
        public async Task<Cqc> GetFromBlobAsync(string provider_id)
        {
            Cqc results = new Cqc();

            try
            {
                //Get json file from blob
                BlobContainerClient categoryStorageClient = new BlobContainerClient(new Uri(test_Blob_URL), null);
                BlobClient categoryBlobClient = categoryStorageClient.GetBlobClient(provider_id + ".json");

                //check if the file exists on blob
                if (!categoryBlobClient.Exists())
                {
                    Console.WriteLine("provider_id " + provider_id + ".json" + " does not exist on blob !");
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
                string blobContents = downloadResult.Content.ToString();
                results = System.Text.Json.JsonSerializer.Deserialize<Cqc>(blobContents);
            }
            catch (Exception e)
            {
                Console.WriteLine("GetFromBlobAsync Error: {0}\n", e);
            }

            return results;
        }

        //Call CQC API
        public async Task<Cqc> CqcAPIAsync(string provider_id)
        {
            Cqc results = new Cqc();

            try
            {
                string url = cqc_Url + "/" + provider_id;

                var client = new HttpClient();
                var response = await client.GetAsync(url);

                if (response is not null && response.StatusCode.ToString() == "OK")
                {
                    string responseBody = await response.Content.ReadAsStringAsync();

                    //insert into Azure blob for cache
                    results = await InsertBlob(responseBody);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("CqcAPIAsync Error: {0}\n", e);
            }

            return results;
        }

        //Insert into Azure blob for cache, return Json results
        public async Task<Cqc> InsertBlob(string responseBody)
        {
            Cqc results = new Cqc();
            try
            {
                results = JsonConvert.DeserializeObject<Cqc>(responseBody);
                string jsonBody = JsonConvert.SerializeObject(results);

                string providerId = results.providerId;
                string fileName = providerId + ".json";

                BlobContainerClient storageClient = new BlobContainerClient(new Uri(test_Blob_URL), null);
                BlobClient blob = storageClient.GetBlobClient(fileName);

                var content = Encoding.UTF8.GetBytes(jsonBody);
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
            return results;
        }
    }
}
