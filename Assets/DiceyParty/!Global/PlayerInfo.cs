namespace DiceyParty
{
    public class PlayerInfo
    {
        public string Name;
        public int ColorIndex;
        public int ClientId;
        public bool IsHost;
    
        public PlayerInfo(){}

        public PlayerInfo(string name, int colorIndex, int clientId)
        {
            Name = name;
            ColorIndex = colorIndex;
            ClientId = clientId;
        }

        internal void SetIsHost(bool toggle)
        {
            IsHost = toggle;
        }

        internal void SetName(string newName)
        {
            Name = newName;
        }
        
    }
}