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
    public class SharedTodoMapping : IEntityTypeConfiguration<SharedTodoEntity>
    {
        public void Configure(EntityTypeBuilder<SharedTodoEntity> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<int>(a => a.TodoId).IsRequired();
            builder.Property<string>(a => a.UserId).IsRequired();
            builder.HasOne(a => a.Todo).WithMany(a => a.SharedTodos).HasForeignKey(a => a.TodoId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}
