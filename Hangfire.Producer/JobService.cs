namespace Hangfire.Producer;

public class JobService : IJobService
{
    public async Task HealthCheck()
    {
        try
        {
            string url = "api_url/api/v1/common/index";
            using var httpClient = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = httpClient.Send(request);
            var resultMessage = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
        }
    }
}