using System.Collections;
using JamKit;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class EndUi : UiBase
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _depthText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private FlashInfo _closeFlashInfo;
        
        [SerializeField] private TextMeshProUGUI _highScoreNamesText;
        [SerializeField] private TextMeshProUGUI _highScoreScoreText;
        
        void Start()
        {
            CoroutineStarter.Run(InitLeaderboard());
            Sfx.Instance.ChangeMusicTrack("MusicSplash", false);
            
            if (ResultData.Instance.HasWon)
            {
                _titleText.text = Random.value > 0.5f ? "You made it!" : "That was awesome!";
                _depthText.gameObject.SetActive(true);
                _depthText.text = $"Depth: {ResultData.Instance.Depth.ToString()}";
                _scoreText.gameObject.SetActive(true);
                _scoreText.text = $"Gem: {ResultData.Instance.Score.ToString()}";
            }
            else
            {
                _titleText.text = "You lost, but it's OK\nJust try again";
                _depthText.gameObject.SetActive(false);
                _scoreText.gameObject.SetActive(false);
            }
            
            Flash(_openFlashInfo);
        }
        
        IEnumerator InitLeaderboard()
        {
            yield return dreamloLeaderBoard.GetSceneDreamloLeaderboard().GetScores();

            var highScores = dreamloLeaderBoard.GetSceneDreamloLeaderboard().ToListHighToLow();
            _highScoreNamesText.text = "";
            _highScoreScoreText.text = "";
            for (int i = 1; i <= 5; i++)
            {
                float hScore = (highScores[i-1].score);
                _highScoreNamesText.text += $"{highScores[i - 1].playerName}";
                _highScoreScoreText.text += $"{highScores[i - 1].score} ({highScores[i - 1].seconds})";
                if (i != 5)
                {
                    _highScoreNamesText.text += "\n";
                    _highScoreScoreText.text += "\n";
                }
            }
        }

        
        public void OnClickedPlayButton()
        {
            Sfx.Instance.Play("FirstSplash");
            Sfx.Instance.FadeOutMusic(_closeFlashInfo.Duration - 0.1f);
            _playButton.interactable = false;
            Flash(_closeFlashInfo, () => SceneManager.LoadScene("Game"));
        }
    }
}