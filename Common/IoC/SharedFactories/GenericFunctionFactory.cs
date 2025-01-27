namespace Common.IoC.SharedFactories;


public interface IGenericFunctionFactory<TReturn, TParameter>
{
    TReturn Create(TParameter authResult);
}

public class GenericFunctionFactory<TReturn, TParameter> : IGenericFunctionFactory<TReturn, TParameter>
{
    private readonly Func<TParameter, TReturn> _factory;
    public GenericFunctionFactory(Func<TParameter, TReturn> factory) => _factory = factory;

    public TReturn Create(TParameter returnVallue) => _factory(returnVallue);
}