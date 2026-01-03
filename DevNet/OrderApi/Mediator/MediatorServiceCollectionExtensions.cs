using System.Reflection;

namespace OrderApi.Mediator
{
    public static class MediatorServiceCollectionExtensions
    {
        public static IServiceCollection AddMediator(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddSingleton<IMediator, Mediator>();
            RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));
            RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
          

            return services;
        }
        private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies, Type genericHandlerType)
        {
            var handlerTypes = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .Where(t => t.GetInterfaces().Any(i =>
                    i.IsGenericType && i.GetGenericTypeDefinition() == genericHandlerType))
                .ToList();

            foreach (var handlerType in handlerTypes)
            {
                var interfaceType = handlerType.GetInterfaces()
                    .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericHandlerType);

                services.AddTransient(interfaceType, handlerType);
            }
        }
    }
}
