using System.Text.Json;
using Temporalio.Activities;

namespace AgeEstimation;

public class AgeEstimationActivities
{
    private static readonly HttpClient Client = new();

    [Activity]
    public async Task<int> RetrieveEstimate(string name)
    {
        var encodedName = Uri.EscapeDataString(name);
        var response = await Client.GetAsync($"https://api.agify.io/?name={encodedName}");
        return await response.Content.ReadAsStringAsync();
    }
}