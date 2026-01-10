namespace DiceyParty
{
    public class PlayerInfo
    {
        public string PlayerName;
        public int ColorIndex;
        public int ClientId;
        public bool IsHost;
    
        public PlayerInfo(){}

        public PlayerInfo(string playerName, int colorIndex, int clientId)
        {
            PlayerName = playerName;
            ColorIndex = colorIndex;
            ClientId = clientId;
        }

        public void SetIsHost(bool toggle)
        {
            IsHost = toggle;
        }

        public void SetName(string newName)
        {
            PlayerName = newName;
        }
        
    }
}