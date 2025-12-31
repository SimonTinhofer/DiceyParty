namespace DiceyParty.MiniGame
{
    public class ResultCardInfo
    {
        public ResultCardInfo()
        {
        }

        public ResultCardInfo(string name, int placement/*, int receivedDice, int receivedItem*/)
        {
            Name = name;
            Placement = placement;
            /*ReceivedDice = receivedDice;
            ReceivedItem = receivedItem;*/
        }

        public string Name;
        public int Placement;
        /*public int ReceivedDice;
        public int ReceivedItem;*/
    }
}