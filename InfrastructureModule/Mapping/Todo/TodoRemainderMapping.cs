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
    public class TodoRemainderMapping : IEntityTypeConfiguration<TodoRemainder>
    {
        public void Configure(EntityTypeBuilder<TodoRemainder> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<DateTime>(a => a.RemainderOn).IsRequired();
            builder.Property<bool>(a => a.IsActive).IsRequired().HasDefaultValue(true);
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.Property<string>(a => a.SetById).IsRequired();
            builder.Property<int>(a => a.TodoId).IsRequired();
            builder.HasOne(a => a.Todo).WithMany(a=>a.TodoRemainder).HasForeignKey(a => a.TodoId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(a => a.SetBy).WithMany().HasForeignKey(a => a.SetById).OnDelete(DeleteBehavior.NoAction);

        }
    }
}
