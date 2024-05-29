using BaseModule.Repository.User;
using DomainModule.BaseRepo;
using DomainModule.Repository;
using DomainModule.RepositoryInterface;
using DomainModule.Service;
using DomainModule.ServiceInterface;
using InfrastructureModule.Repository;
using ServiceModule.Service;
using System.Runtime.CompilerServices;


namespace TodoApp.DiConfig
{
    public static class DiConfiguration
    {
        public static void  UseDIConfig(this IServiceCollection services)
        {
            UseRepository(services);
            UseService(services);
        }
        private static void UseRepository(IServiceCollection services)
        {
         services.AddScoped<IUserRepository,UserRepository>();
         services.AddScoped<IRoleRepository,RoleRepository>();
         services.AddScoped<ITodoRepository,TodoRepository>();
         services.AddScoped<ISharedTodoRepository,SharedTodoRepository>();
         services.AddScoped<ITodoHistoryRepository,TodoHistoryRepository>();
         services.AddScoped<ITodoRemainderRepository,TodoRemainderRepository>();
         services.AddScoped<INotificationRepository,NotificationRepository>();
         services.AddScoped<IEmailMessageRepository,EmailMessageRepository>();
        }
        private static void UseService(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<ISharedTodoService, SharedTodoService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailMessageService, EmailMessageService>();
        }
    }
}
