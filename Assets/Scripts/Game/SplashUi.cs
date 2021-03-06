using System.Collections;
using System.Runtime.InteropServices;
using JamKit;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class SplashUi : UiBase
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private FlashInfo _closeFlashInfo;
        
        [SerializeField] private TextMeshProUGUI _highScoreNamesText;
        [SerializeField] private TextMeshProUGUI _highScoreScoreText;
        [SerializeField] private TMP_InputField _inputField;

        [DllImport("__Internal")]
        private static extern void OpenNewTab(string url);

        void Start()
        {
            Sfx.Instance.StartMusic("MusicSplash", false);
            Flash(_openFlashInfo);
            CoroutineStarter.Run(InitLeaderboard());
        }
        
        public void OnClickedPlayButton()
        {
            PlayerPrefs.SetString("Name", _inputField.text.ToUpper());
            Sfx.Instance.Play("FirstSplash");
            _playButton.interactable = false;
            Sfx.Instance.FadeOutMusic(_closeFlashInfo.Duration - 0.1f);
            Flash(_closeFlashInfo, () => SceneManager.LoadScene("Game"));
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
        
        public void PlayButtonVisibility()
        {
            if (_inputField.text.Length > 3)
            {
                _playButton.interactable = true;
            }
            else
            {
                _playButton.interactable = false;
            }
        }

        public void OnOpenLink(string link)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenNewTab(link);
#endif
        }
    }
}