﻿using BaseModule.Repository.User;
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
         services.AddScoped<UserRepositoryInterface,UserRepository>();
         services.AddScoped<RoleRepositoryInterface,RoleRepository>();
         services.AddScoped<TodoRepositoryInterface,TodoRepository>();
         services.AddScoped<SharedTodoRepositoryInterface,SharedTodoRepository>();
         services.AddScoped<TodoHistoryRepositoryInterface,TodoHistoryRepository>();
         services.AddScoped<TodoRemainderRepositoryInterface,TodoRemainderRepository>();
         services.AddScoped<NotificationRepositoryInterface,NotificationRepository>();
         services.AddScoped<EmailMessageRepositoryInterface,EmailMessageRepository>();
        }
        private static void UseService(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddScoped<UserServiceInterface, UserService>();
            services.AddScoped<RoleServiceInterface, RoleService>();
            services.AddScoped<TodoServiceInterface, TodoService>();
            services.AddScoped<SharedTodoServiceInterface, SharedTodoService>();
            services.AddScoped<DashboardServiceInterface, DashboardService>();
            services.AddScoped<NotificationServiceInterface, NotificationService>();
            services.AddScoped<EmailMessageServiceInterface, EmailMessageService>();
        }
    }
}
