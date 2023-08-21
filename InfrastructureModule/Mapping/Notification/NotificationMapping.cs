using DomainModule.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureModule.Mapping
{
    public class NotificationMapping : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<string>(a => a.UserId).IsRequired();
            builder.Property<bool>(a => a.MarkedAsRead).IsRequired().HasDefaultValue(false);
            builder.Property<string>(a => a.NotificationMessage).IsRequired();
            builder.Property<int?>(a => a.TodoId);
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.Todo).WithMany().HasForeignKey(a => a.TodoId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
