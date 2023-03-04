using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wsa.Gaas.Werewolf.Domain.Entities.Rules;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore.TypeConfigurations
{
    internal class RoleTypeConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.Property(e => e.Name).HasMaxLength(128).IsUnicode(true);

            builder.HasDiscriminator(e => e.Id)
                .HasValue<Domain.Entities.Rules.Werewolf>(1)
                .HasValue<AlphaWerewolf>(2)
                .HasValue<Villager>(3)
                .HasValue<Witch>(4)
                .HasValue<Seer>(5)
                .HasValue<Hunter>(6)
                .HasValue<Guardian>(7)
                ;

        }
    }
}
