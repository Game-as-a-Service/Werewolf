using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wsa.Gaas.Werewolf.Domain.Objects;
using Wsa.Gaas.Werewolf.Domain.Objects.Roles;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore.TypeConfigurations;
internal class RoleTypeConfiguration : IEntityTypeConfiguration<Role>
    , IEntityTypeConfiguration<Domain.Objects.Roles.Werewolf>
    , IEntityTypeConfiguration<AlphaWerewolf>
    , IEntityTypeConfiguration<Villager>
    , IEntityTypeConfiguration<Witch>
    , IEntityTypeConfiguration<Seer>
    , IEntityTypeConfiguration<Hunter>
    , IEntityTypeConfiguration<Guardian>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(e => e.Name).HasMaxLength(128).IsUnicode(true);

        builder.HasDiscriminator(e => e.Id)
            .HasValue<Domain.Objects.Roles.Werewolf>(1)
            .HasValue<AlphaWerewolf>(2)
            .HasValue<Villager>(3)
            .HasValue<Witch>(4)
            .HasValue<Seer>(5)
            .HasValue<Hunter>(6)
            .HasValue<Guardian>(7)
            ;
    }

    public void Configure(EntityTypeBuilder<Domain.Objects.Roles.Werewolf> builder)
    {
        builder.HasData(Role.WEREWOLF);
    }

    public void Configure(EntityTypeBuilder<AlphaWerewolf> builder)
    {
        builder.HasData(Role.ALPHAWEREWOLF);
    }

    public void Configure(EntityTypeBuilder<Witch> builder)
    {
        builder.HasData(Role.WITCH);
    }

    public void Configure(EntityTypeBuilder<Seer> builder)
    {
        builder.HasData(Role.SEER);
    }

    public void Configure(EntityTypeBuilder<Hunter> builder)
    {
        builder.HasData(Role.HUNTER);
    }

    public void Configure(EntityTypeBuilder<Guardian> builder)
    {
        builder.HasData(Role.GUARDIAN);
    }

    public void Configure(EntityTypeBuilder<Villager> builder)
    {
        builder.HasData(Role.VILLAGER);
    }
}
