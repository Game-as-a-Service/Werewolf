﻿namespace Wsa.Gaas.Werewolf.Domain.Exceptions;
public class PlayerNotWitchException : GameException
{
    public PlayerNotWitchException(string message) : base(message)
    {
    }
}