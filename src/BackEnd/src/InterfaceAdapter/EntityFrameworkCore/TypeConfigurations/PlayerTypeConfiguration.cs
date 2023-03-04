using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wsa.Gaas.Werewolf.Domain.Entities;

namespace Wsa.Gaas.Werewolf.EntityFrameworkCore.TypeConfigurations
{
    internal class PlayerTypeConfiguration : IEntityTypeConfiguration<Player>
    {
        public void Configure(EntityTypeBuilder<Player> builder)
        {
            builder.HasOne<Game>()
                .WithMany("_players");
        }
    }
}
