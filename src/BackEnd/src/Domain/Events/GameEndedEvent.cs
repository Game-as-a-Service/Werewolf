using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wsa.Gaas.Werewolf.Domain.Objects;

namespace Wsa.Gaas.Werewolf.Domain.Events
{
    public class GameEndedEvent : GameEvent
    {
        public GameEndedEvent(Game data) : base(data)
        {
        }
    }
}
