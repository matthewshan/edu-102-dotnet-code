namespace AgeEstimation;

using System.Text.Json;
using TemporalAgeEstimation.Workflow.Models;
using Temporalio.Activities;
using Temporalio.Api.Dependencies.Google.Api;

namespace TemporalAgeEstimation;

public class AgeEstimationActivities
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private readonly HttpClient client;

    public AgeEstimationActivities(HttpClient client) => this.client = client;

    [Activity]
    public async Task<int> RetrieveEstimateAsync(string name)
    {
        var encodedName = Uri.EscapeDataString(name);
        var url = $"https://api.agify.io/?name={encodedName}";
        var response = await client.GetAsync(url);
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<EstimatorResponse>(content, JsonOptions);
        return result?.Age ?? 0;
    }
}