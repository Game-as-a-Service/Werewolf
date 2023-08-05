using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore.TypeConfigurations;

internal class GameTypeConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Ignore(e => e.Players);

        builder.HasMany("_players").WithOne();
        builder.HasOne(e => e.CurrentSpeakingPlayer)
            .WithMany()
            .HasForeignKey(e => e.CurrentSpeakingPlayerId);

        builder.Navigation("_players").AutoInclude();

    }
}
