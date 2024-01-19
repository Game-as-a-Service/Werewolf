using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore.TypeConfigurations;
internal class PlayerTypeConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Role).WithMany();

        builder.Navigation(e => e.Role).AutoInclude();
    }
}
