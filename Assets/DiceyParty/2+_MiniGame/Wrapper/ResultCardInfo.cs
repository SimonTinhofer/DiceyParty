namespace DiceyParty.MiniGame
{
    public class ResultCardInfo
    {
        public readonly string Name;
        public readonly int Placement;
        public readonly int ColorIndex;
        public ResultCardInfo(){}

        public ResultCardInfo(string name, int placement, int colorIndex)
        {
            Name = name;
            Placement = placement;
            ColorIndex = colorIndex;
        }
    }
}