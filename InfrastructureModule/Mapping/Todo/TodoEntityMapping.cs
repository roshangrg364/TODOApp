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
    public class TodoEntityMapping : IEntityTypeConfiguration<TodoEntity>
    {
        public void Configure(EntityTypeBuilder<TodoEntity> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<string>(a => a.Title).IsRequired();
            builder.Property<string>(a => a.Status).HasMaxLength(50).IsRequired();
            builder.Property<int>(a => a.PriorityLevel).IsRequired();
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.Property<DateTime?>(a => a.CompletedOn);
            builder.Property<DateTime>(a => a.DueDate).IsRequired();
            builder.Property<string>(a => a.CreatedBy);
            builder.Property<string?>(a => a.CompletedBy);
            builder.Property<string?>(a => a.Description);
            builder.HasOne(a => a.CreatedByUser).WithMany().HasForeignKey(a => a.CreatedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.CompletedByUser).WithMany().HasForeignKey(a => a.CompletedBy).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(a => a.SharedTodos).WithOne().HasForeignKey(a => a.TodoId).OnDelete(DeleteBehavior.NoAction);
           
        }
    }
}
