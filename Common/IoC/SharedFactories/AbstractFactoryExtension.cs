using Microsoft.Extensions.DependencyInjection;

namespace Common.IoC.SharedFactories;


public static class AbstractFactoryExtension
{
    public static void AddAbstractFactory<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _ = services.AddTransient<TInterface, TImplementation>()
        .AddSingleton<Func<TInterface>>(container => () => container.GetRequiredService<TInterface>())
        .AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();
    }
}


public class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;
    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }
    public T Create() => _factory();
}

public interface IAbstractFactory<T>
{
    T Create();
}