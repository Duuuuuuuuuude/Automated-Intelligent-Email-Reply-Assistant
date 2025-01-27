﻿using Microsoft.Extensions.DependencyInjection;

namespace Common.IoC.SharedFactories;

public interface IGenericFactory<TBase>
{
    TImplementation Create<TImplementation>() where TImplementation : class, TBase;
}


public class GenericFactory<TBase> : IGenericFactory<TBase>
{
    private readonly IServiceProvider _serviceProvider;

    public GenericFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public TImplementation Create<TImplementation>() where TImplementation : class, TBase
    {
        return _serviceProvider.GetRequiredService<TImplementation>();
    }
}
