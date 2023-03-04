namespace Wsa.Gaas.Werewolf.EntityFrameworkCore;

public class SimpleIdGenerator : IIdGenerator
{
    public long GenerateId()
    {
        var rnd = new Random();

        //by CHATGPT...
        return ((long) rnd.Next() << 32) | (uint) rnd.Next();
    }
}