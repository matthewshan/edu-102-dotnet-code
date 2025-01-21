namespace TemporalAgeEstimation.Workflow.Models;

public record EstimatorResponse(
    int Age,
    int Count,
    string Name);