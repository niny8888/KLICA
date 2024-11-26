namespace Klica.Classes
{
    public class GameplayRules
    {
        public int MaxTime { get; private set; } // Max cs za lvl v sec
        public int ScoreToWin { get; private set; } //score da dokoncas level
        public bool HasTimeLimit => MaxTime > 0; //ce ma level time limit

        public GameplayRules(int maxTime, int scoreToWin)
        {
            MaxTime = maxTime;
            ScoreToWin = scoreToWin;
        }

        public bool CheckWinCondition(int currentScore)
        {
            return currentScore >= ScoreToWin;
        }

        public bool CheckLoseCondition(int elapsedTime)
        {
            return HasTimeLimit && elapsedTime > MaxTime;
        }
    }
}
