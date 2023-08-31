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
    public class EmailMessageMapping : IEntityTypeConfiguration<EmailMessage>
    {
        public void Configure(EntityTypeBuilder<EmailMessage> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<string>(a => a.From).IsRequired().HasMaxLength(100);
            builder.Property<string>(a => a.Content).IsRequired();
            builder.Property<string>(a => a.Subject).IsRequired().HasMaxLength(200);
            builder.Property<string>(a => a.Status).IsRequired().HasMaxLength(100);
            builder.Property<string>(a => a.Priority).IsRequired().HasMaxLength(100);
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.Property<DateTime?>(a => a.DeliveredOn).IsRequired();
            builder.HasMany(a => a.EmailRecipients).WithOne(a => a.EmailMessage).HasForeignKey(a => a.EmailMessageId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
