namespace TemporalAgeEstimation.Workflow;

public record EstimatorResponse(
    int Age,
    int Count,
    string Name);