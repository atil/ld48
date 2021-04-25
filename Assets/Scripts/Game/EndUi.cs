﻿using JamKit;
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
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private FlashInfo _closeFlashInfo;

        void Start()
        {
            Sfx.Instance.ChangeMusicTrack("MusicSplash", false);
            
            if (ResultData.Instance.HasWon)
            {
                _titleText.text = "You won!";
                _scoreText.gameObject.SetActive(true);
                _scoreText.text = $"Score: {ResultData.Instance.Score.ToString()}";
            }
            else
            {
                _titleText.text = "You lost, but it's OK\nJust try again";
                _scoreText.gameObject.SetActive(false);
            }
            
            Flash(_openFlashInfo);
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