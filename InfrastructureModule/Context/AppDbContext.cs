using DomainModule.Entity;
using InfrastructureModule.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureModule.Context
{
    public class AppDbContext:IdentityDbContext
    {
        private readonly IConfiguration _configuration;
        public AppDbContext(DbContextOptions<AppDbContext> options,IConfiguration configuration):base(options)
        {
            _configuration = configuration;
        }
        public DbSet<TodoEntity> Todos { get; set; }
        public DbSet<SharedTodoEntity> SharedTodos { get; set; }
        public DbSet<TodoRemainder> TodoRemainders { get; set; }
        public DbSet<TodoHistory> TodoHistories { get; set; }
        public DbSet<EmailMessage> EmailMessages { get; set; }
        public DbSet<EmailRecipient> EmailRecipients { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Notification> Setting { get; set; }
 
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserEntityMapping());
            builder.ApplyConfiguration(new TodoEntityMapping());
            builder.ApplyConfiguration(new SharedTodoMapping());
            builder.ApplyConfiguration(new TodoHistoryMapping());
            builder.ApplyConfiguration(new TodoRemainderMapping());
            builder.ApplyConfiguration(new NotificationMapping());
            builder.ApplyConfiguration(new EmailMessageMapping());
            builder.ApplyConfiguration(new EmailRecipeintMapping());
            builder.ApplyConfiguration(new SettingEntityMapping());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var conString = _configuration.GetConnectionString("DefaultConnection");
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseLazyLoadingProxies().UseSqlServer(conString);
            }
        }
    }
}
