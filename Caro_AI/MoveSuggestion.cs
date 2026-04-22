namespace Caro_AI
{
    public class MoveSuggestion
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public MoveSuggestion(int row, int col)
        {
            Row = row;
            Col = col;
        }
    }
}
