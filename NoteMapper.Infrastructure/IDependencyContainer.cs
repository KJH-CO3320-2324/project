namespace NoteMapper.Infrastructure
{
    public interface IDependencyContainer
    {
        IDependencyContainer AddScoped<T>() where T : class;

        IDependencyContainer AddScoped<TInterface, TImplementation>() 
            where TInterface : class
            where TImplementation : class, TInterface;

        IDependencyContainer AddSingleton<T>(T instance)
            where T : class;
    }
}
