using NoteMapper.Infrastructure;

namespace NoteMapper.Web.Blazor.Services
{
    public class DependencyContainer : IDependencyContainer
    {
        private readonly IServiceCollection _services;

        public DependencyContainer(IServiceCollection services)
        {
            _services = services;
        }

        public IDependencyContainer AddScoped<T>() where T : class
        {
            _services.AddScoped<T>();
            return this;
        }

        public IDependencyContainer AddScoped<TInterface, TImplementation>()
            where TInterface : class
            where TImplementation : class, TInterface
        {
            _services.AddScoped<TInterface, TImplementation>();
            return this;
        }

        public IDependencyContainer AddSingleton<T>(T instance) where T : class
        {
            _services.AddSingleton(instance);
            return this;
        }
    }
}
