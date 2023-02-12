namespace Wsa.Gaas.Werewolf.Domain.Objects
{
    public enum GameStatus
    {
        Created,
        Started,
        PlayerRoleConfirmationStarted,
        

        //天亮前置作業
        Sunriseing,

        //天亮了
        Sunrise,


        Ended,
    }
}