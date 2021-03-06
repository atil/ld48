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
            for (int i = 1; i <= 10; i++)
            {
                string colorText = "";
                if (i == 1)
                {
                    colorText = "<color=#FFCC00>";
                }
                else if (i == 2)
                {
                    colorText = "<color=#ACC0C1>";
                }
                else if (i == 3)
                {
                    colorText = "<color=#BF8851>";
                }
                else
                {
                    colorText = "<color=#8AE0C5>";
                }
                int hScore = highScores[i-1].score / 1000;
                int gem = highScores[i-1].score % 1000;
                _highScoreNamesText.text += $"{colorText}{highScores[i - 1].playerName}</color>";
                _highScoreScoreText.text += $"{colorText}{hScore} ({gem})</color>";
                if (i != 10)
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