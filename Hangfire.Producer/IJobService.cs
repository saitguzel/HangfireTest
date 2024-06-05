namespace Hangfire.Producer;

public interface IJobService
{
    Task HealthCheck();
}
