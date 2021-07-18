
using Microsoft.Extensions.DependencyInjection;
using ToDoList.DB.Repositories;
using ToDoList.Interfaces.Interfaces;

namespace ToDoList.DB.DependencyInjection
{
    public static class DependencyInjection
    {
  

        public static IServiceCollection AddInfrastructure(this IServiceCollection serviceDescriptors) 
        {
            serviceDescriptors.AddTransient<IIdentity, IdentityRepository>();
            serviceDescriptors.AddTransient<IToDo, ToDoRepository>();
            serviceDescriptors.AddTransient<IIdentityToDo, IIdentityToDoRepository>();
            serviceDescriptors.AddTransient<INotification, NotificationRepository>();
            return serviceDescriptors;
        }

    }
}
