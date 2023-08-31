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
    public class EmailRecipeintMapping : IEntityTypeConfiguration<EmailRecipient>
    {
        public void Configure(EntityTypeBuilder<EmailRecipient> builder)
        {
            builder.HasKey(a => a.Id);
            builder.Property<string>(a => a.RecipientEmail).IsRequired().HasMaxLength(100);
            builder.Property<DateTime>(a => a.CreatedOn).IsRequired();
            builder.Property<int>(a => a.EmailMessageId).IsRequired();
            builder.HasOne(a => a.EmailMessage).WithMany(a => a.EmailRecipients).HasForeignKey(a => a.EmailMessageId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}
