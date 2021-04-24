using JamKit;

namespace Game
{
    public class ResultData : SingletonBehaviour<ResultData>
    {
        public bool HasWon;
        public int Score;

        public void Clear()
        {
            HasWon = false;
            Score = 0;
        }
    }
}