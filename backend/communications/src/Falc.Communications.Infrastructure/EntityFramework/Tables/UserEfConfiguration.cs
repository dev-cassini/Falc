using Falc.Communications.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Falc.Communications.Infrastructure.EntityFramework.Tables;

public class UserEfConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(CommunicationsDbContext.Users), CommunicationsDbContext.Schema);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.EmailAddress);
        builder.Property(x => x.CreationTimestampUtc);
    }
}