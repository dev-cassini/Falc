using MassTransit;

namespace Falc.Communications.Infrastructure.MassTransit;

public class CustomEndpointNameFormatter : IEndpointNameFormatter
{
    private static readonly IEndpointNameFormatter Instance = DefaultEndpointNameFormatter.Instance;
    
    public string TemporaryEndpoint(string tag)
    {
        return Instance.TemporaryEndpoint(tag);
    }

    public string Consumer<T>() where T : class, IConsumer
    {
        return GetTypeFullName<T>();
    }

    public string Message<T>() where T : class
    {
        return GetTypeFullName<T>();
    }

    public string Saga<T>() where T : class, ISaga
    {
        return GetTypeFullName<T>();
    }

    public string ExecuteActivity<T, TArguments>() where T : class, IExecuteActivity<TArguments> where TArguments : class
    {
        return GetTypeFullName<T>() + "_Execute";
    }

    public string CompensateActivity<T, TLog>() where T : class, ICompensateActivity<TLog> where TLog : class
    {
        return GetTypeFullName<T>() + "_Compensate";
    }

    public string SanitizeName(string name)
    {
        return name;
    }

    public string Separator { get; } = Instance.Separator;

    private string GetTypeFullName<T>() => typeof(T).FullName ?? string.Empty;
}