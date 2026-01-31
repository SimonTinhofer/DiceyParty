namespace DiceyParty.MiniGame.RollOff
{
    public class PlayerScoreInfo
    {
        public int ClientId;
        public int ColorIndex;
        public int LongestRun;

        public PlayerScoreInfo(int clientId, int colorIndex, int longestRun)
        {
            ClientId = clientId;
            ColorIndex = colorIndex;
            LongestRun = longestRun;
        }
        public PlayerScoreInfo(){}
    }
}