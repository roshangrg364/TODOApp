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
    public class SettingEntityMapping : IEntityTypeConfiguration<SettingEntity>
    {
        public void Configure(EntityTypeBuilder<SettingEntity> builder)
        {
            builder.HasKey(a => a.SettingId);
            builder.Property<string>(a => a.SettingKey).IsRequired();
            builder.Property<string>(a => a.SettingGroup).IsRequired();
            builder.Property<string>(a => a.SettingValue).IsRequired();
        }
    }
}
