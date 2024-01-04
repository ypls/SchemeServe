I used ASP.Net Core to create the web API. 

------------------------------------
Controllers/ProvidersController:

Two end points:

public async Task<IActionResult> Get(string provider_id):
To get individual provider  by request of /Providers/{provider_id}, and response with the Json format which was metioned in the "Backend Tech Test".

public async Task<IActionResult> Get():
To get providers by request of /Providers. Because the the "Backend Tech Test" did not metion the response format, I just response what received from CQC API.

------------------------------------
Models/Cqc:

Define a class for response format.

------------------------------------
Database for cache:

I stored cached records into Azure blob of "test_Blob_URL". Before I uploaded to GitHub, I just deleted my test blob URL because it is belong to my curent employer.

------------------------------------
Models/Individual:

public async Task<Cqc> ProviderAsync(string provider_id):
To check cached blob if records exists in the cache.  If the record exists in the cache then return; if it is not found, then call the CQC API.

public async Task<Cqc> GetFromBlobAsync(string provider_id):
To check blob and download cached records.

public async Task<Cqc> CqcAPIAsync(string provider_id):
Call CQC API

public async Task<Cqc> InsertBlob(string responseBody):
To insert records into blob for caching, and return the records for response.

------------------------------------
Models/GetProviders:

public async Task<string> ProvidersAsync():
To check cached blob if records exists in the cache. If the records exists in the cache then return; if it is not found, then call the CQC API.

public async Task<Cqc> GetFromBlobAsync(string provider_id):
To check blob and download cached records.

public async Task<Cqc> CqcAPIAsync(string provider_id):
Call CQC API

public async Task<Cqc> InsertBlob(string responseBody):
To insert records into blob for caching.

------------------------------------
appsetting.json:
Set CQC URL and test blob URL.

------------------------------------
Due to time constraints, I didn't finish the code for unit testing.


