using JamKit;

namespace Game
{
    public class ResultData : SingletonBehaviour<ResultData>
    {
        public bool HasWon;
        public int Score;
        public int Depth;

        public void Clear()
        {
            HasWon = false;
            Score = 0;
            Depth = 0;
        }
    }
}