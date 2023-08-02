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
    public class TodoHistoryMapping : IEntityTypeConfiguration<TodoHistory>
    {
        public void Configure(EntityTypeBuilder<TodoHistory> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<int>(a => a.TodoId).IsRequired();
            builder.Property<string>(a => a.CommentedByUserId).IsRequired();
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.Property<string>(a => a.Comment).IsRequired();
            builder.Property<string>(a => a.Status).IsRequired();
            builder.HasOne(a => a.Todo).WithMany(a => a.SharedTodoHistory).HasForeignKey(a => a.TodoId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.CommentedByUser).WithMany().HasForeignKey(a => a.CommentedByUserId).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
