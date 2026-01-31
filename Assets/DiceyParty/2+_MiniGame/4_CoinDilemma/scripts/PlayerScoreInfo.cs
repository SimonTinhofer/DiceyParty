namespace DiceyParty.MiniGame.CoinDilemma
{
    public class PlayerScoreInfo
    {
        public int ClientId;
        public int ColorIndex;
        public int CoinAmount;

        public PlayerScoreInfo(int clientId, int colorIndex, int coinAmount)
        {
            ClientId = clientId;
            ColorIndex = colorIndex;
            CoinAmount = coinAmount;
        }
        public PlayerScoreInfo(){}
    }
}