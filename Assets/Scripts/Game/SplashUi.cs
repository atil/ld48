using System.Runtime.InteropServices;
using JamKit;
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
        
        [DllImport("__Internal")]
        private static extern void OpenNewTab(string url);

        void Start()
        {
            Sfx.Instance.StartMusic("MusicSplash", false);
            Flash(_openFlashInfo);
        }
        
        public void OnClickedPlayButton()
        {
            Sfx.Instance.Play("FirstSplash");
            _playButton.interactable = false;
            Sfx.Instance.FadeOutMusic(_closeFlashInfo.Duration - 0.1f);
            Flash(_closeFlashInfo, () => SceneManager.LoadScene("Game"));
        }
        
        public void OnOpenLink(string link)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             OpenNewTab(link);
#endif
        }
    }
}