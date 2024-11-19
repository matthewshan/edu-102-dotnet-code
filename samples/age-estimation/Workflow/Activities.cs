using System.Text.Json;
using Temporalio.Activities;

namespace AgeEstimation;
public class AgeEstimationActivities
{
    private static readonly HttpClient Client = new();

    [Activity]
    public async Task<int> RetrieveEstimateAsync(string name)
    {
        var encodedName = Uri.EscapeDataString(name);
        var url = $"https://api.agify.io/?name={encodedName}";
        var response = await Client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EstimatorResponse>(content);
        return result?.age ?? 0;
    }
}