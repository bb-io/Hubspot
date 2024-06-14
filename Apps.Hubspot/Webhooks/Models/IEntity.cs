namespace Apps.Hubspot.Webhooks.Models;

public interface IEntity
{
    string Id { get; set; }
    string Created { get; set; }
    string Updated { get; set; }
}