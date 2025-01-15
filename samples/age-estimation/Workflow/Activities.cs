namespace AgeEstimation;

using System.Text.Json;
using Temporalio.Activities;
using Temporalio.Api.Dependencies.Google.Api;

public class AgeEstimationActivities
{
    private readonly HttpClient client;
    public AgeEstimationActivities(HttpClient client) => this.client = client;

    [Activity]
    public async Task<int> RetrieveEstimateAsync(string name)
    {
        var encodedName = Uri.EscapeDataString(name);
        var url = $"https://api.agify.io/?name={encodedName}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EstimatorResponse>(content);
        return result?.age ?? 0;
    }
}