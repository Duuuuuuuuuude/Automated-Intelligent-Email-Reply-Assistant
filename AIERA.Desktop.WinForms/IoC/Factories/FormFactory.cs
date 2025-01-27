using Microsoft.Extensions.DependencyInjection;

namespace AIERA.Desktop.WinForms.IoC.Factories;

/// <summary>
///     Inspired by: <see href="https://www.wiktorzychla.com/2022/01/winforms-dependency-injection-in-net6.html"/>
/// </summary>
public interface IFormFactory
{
    SettingsForm GetOpenOrCreateNewSettingsForm();
    SettingsForm? GetOpenSettingsForm();
    LoginForm GetOpenOrCreateNewLoginForm();
    MainApplicationContext CreateMainApplicationContext();
}

/// <summary>
/// <para>
///     Use this class to create <see cref="Form"/>s, using the factory pattern, 
///     with the required dependencies already injected into them.
/// </para>
///     Inspired by: <see href="https://www.wiktorzychla.com/2022/01/winforms-dependency-injection-in-net6.html"/>
/// <br/>
/// <br/>
/// <example>
///     To create a form, use the factory like this:
///     <code>new FormFactory().CreateForm1();</code> 
///     for "simple" forms (with service dependencies only)
///     or use
///     <code>new FormFactory().CreateForm2("foo");</code>
///     for forms that need both service dependencies and other free parameters.
///     The dependency injection should take care of the dependencies, so no need to pass them in the above examples.
/// </example>
/// </summary>
/// <remarks>
/// <para>
///     This class exists both to avoid the service locator anti-pattern (e.g. <see cref="ServiceProvider.GetService(Type)"/>),
///     or alternativly having to pass dependencies through the constructor of <see cref="Form"/>s, 
///     that doesn't even need them, just beause they need to be passed to another constructor,
///     and also to make it easier to do unit testing, 
///     by just replacing <see cref="FormFactoryImplProduction"/> with a stub/mock <see cref="IFormFactory"/> provider.
///     This class doesn't have access to the dependency injection container, and therefor can't resolve the dependencies itself,
///     but delegates the Form creation to another concrete <see cref="IFormFactory"/> implementation.
///     This is how it is possible to inject different kinds of providers, 
///     and for example replace the <see cref="FormFactoryImplProduction"/> used for production code
///     with a stub/mock <see cref="IFormFactory"/> for unit testing.
/// </para>
/// </remarks>
public class FormFactory : IFormFactory
{
    private static IFormFactory? _provider;

    public static void SetProvider(IFormFactory provider) => _provider = provider;


    /// <summary>
    /// 
    /// </summary>
    /// <returns>A new instance of <see cref="SettingsForm"/> unless it has been garbage collected or disposed, then a new instance of <see cref="SettingsForm"/> would be created and returned.</returns>
    public SettingsForm GetOpenOrCreateNewSettingsForm() => _provider!.GetOpenOrCreateNewSettingsForm();
    public SettingsForm? GetOpenSettingsForm() => _provider!.GetOpenSettingsForm();

    /// <summary>
    /// 
    /// </summary>
    /// <returns>A new instance of <see cref="LoginForm"/> unless it has been garbage collected or disposed, then a new instance of <see cref="LoginForm"/> would be created and returned.</returns>
    public LoginForm GetOpenOrCreateNewLoginForm() => _provider!.GetOpenOrCreateNewLoginForm();

    public MainApplicationContext CreateMainApplicationContext() => _provider!.CreateMainApplicationContext();
}


/// <summary>
/// <para>
///     <br/>
///     This class is one of the concrete implementations of <see cref="IFormFactory"/> 
///     and is used to create <see cref="Form"/>s with the required dependencies using the "dependency injection" based <see cref="IServiceProvider"/> passed to the constructor.
///     This class is intended to be used in production code and not for testing.
/// </para>
///     Inspired by: <see href="https://www.wiktorzychla.com/2022/01/winforms-dependency-injection-in-net6.html"/>
/// <br/>
/// <br/> 
/// </summary>
/// <remarks>
///     Don't use this directly, to create <see cref="Form"/>s, but use the <see cref="FormFactory"/> class instead.
///     This is intended to be used by the <see cref="FormFactory"/> class, to create <see cref="Form"/>s with the required dependencies.
///     <br/>
/// <para>
///     This class takes a "dependency injection" based <see cref="IServiceProvider"/>, to resolve the dependencies,
///     but can be replaced in the <see cref="FormFactory"/> class with a stub/mock <see cref="IFormFactory"/> for unit testing,
///     or any other kind of <see cref="IFormFactory"/> implementation, that uses different kinds of service providers.
/// </para>
///
/// </remarks>
/// 
/// <param name="serviceProvider">
///     "dependency injection" based service provider, 
///     used to resolve the services needed by the <see cref="Form"/>s created by this factory.
/// </param>
public class FormFactoryImplProduction(IServiceProvider serviceProvider) : IFormFactory
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;


    /// <summary>
    /// Returns the open <see cref="SettingsForm"/> if it exists and is not disposed, otherwise creates a new instance of <see cref="SettingsForm"/>.
    /// </summary>
    /// <returns></returns>
    /// <remarks>
    ///  <para>
    ///  <see cref="Form"/>s are created here using factory pattern, instead of by the Dependency Injection, 
    ///  since services should never be disposed by code that resolved the service from the DI container,
    ///  and <see cref="Form"/>s would be disposed by the reciever of the Form, 
    ///  unless that is turned off every time a new <see cref="Form"/> is created and used.
    ///  This is per Microsofts own guidelines <see href="https://learn.microsoft.com/en-us/dotnet/core/extensions/dependency-injection-guidelines"/>
    ///  
    ///  Something close to a normal Singleton pattern is used here, to avoid more than one instance existing at a time. 
    ///  Instances are not static, so the garbage collector can still manage them, since we don't care about preserving state between instances and actually want the state to be reloaded everytime they are opened, and lastly because we don't want to use memory on a closed form.
    ///  </para>
    /// </remarks>
    public SettingsForm GetOpenOrCreateNewSettingsForm()
    {
        if (GetOpenSettingsForm() is { IsDisposed: false } settingsForm)
            return settingsForm;

        return ActivatorUtilities.CreateInstance<SettingsForm>(_serviceProvider);
    }
    public SettingsForm? GetOpenSettingsForm() => Application.OpenForms.OfType<SettingsForm>().SingleOrDefault();


    public LoginForm GetOpenOrCreateNewLoginForm()
    {
        if (Application.OpenForms.OfType<LoginForm>().SingleOrDefault() is { IsDisposed: false } loginForm)
            return loginForm;

        return ActivatorUtilities.CreateInstance<LoginForm>(_serviceProvider);
    }

    public MainApplicationContext CreateMainApplicationContext() => ActivatorUtilities.CreateInstance<MainApplicationContext>(_serviceProvider);
}