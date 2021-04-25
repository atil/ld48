using System.Collections;
using JamKit;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public enum ScoreType
    {
        None,
        Depth,
        Gem
    }
    
    public class GameUi : UiBase
    {
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private TextMeshProUGUI _oxygenText;
        [SerializeField] private TextMeshProUGUI _gemText;
        [SerializeField] private Slider _slider;
        [SerializeField] private RectTransform _depthArrow;
        [SerializeField] private RectTransform _returnButton;
        
        [SerializeField] private GameObject _oxygenBarPrefab;
        [SerializeField] private Transform _oxygenBarParent;

        [SerializeField] private FlashInfo _closeFlashInfo;

        public AnimationCurve SliderMoveCurve;
        private const float _sliderMoveDuration = 0.5f;
        
        [SerializeField] private TextMeshProUGUI _normalFloatingText;
        [SerializeField] private TextMeshProUGUI _gemFloatingText;
        private const float _floatingTextDuration = 1f;
        private bool _isFloatingTextActive = false;

        void Start()
        {
            Flash(_openFlashInfo);
        }

        public void SetOxygen(int oxy)
        {
            _oxygenText.text = $"{oxy.ToString()}";
            
            // int initValue = (int)_slider.value;
            //
            // Curve.Tween(SliderMoveCurve,
            //     _sliderMoveDuration,
            //     t =>
            //     {
            //         _slider.value = Mathf.Lerp(initValue, oxy, t);
            //     },
            //     () =>
            //     {
            //         _slider.value = oxy;
            //     });


            foreach (Transform t in _oxygenBarParent)
            {
                Destroy(t.gameObject);
            }

            for (int i = 0; i < oxy; i++)
            {
                Instantiate(_oxygenBarPrefab, _oxygenBarParent);
            }
        }
        
        public void SetScore(int gem, int delta = 0, ScoreType type = ScoreType.None)
        {
            switch (type)
            {
                case ScoreType.Depth:
                    CoroutineStarter.Run(ScoreFloatingText(delta.ToString(), _normalFloatingText, gem.ToString()));
                    break;
                case ScoreType.Gem:
                    CoroutineStarter.Run(ScoreFloatingText(delta.ToString(), _gemFloatingText, gem.ToString()));
                    break;
                default:
                    _gemText.text = $"{gem.ToString()}";
                    break;
            }
        }

        public void SetDepth(int depthIndex)
        {
            const int maxDepth = 150; // Source: depths of my ass
            
            Vector2 srcPos =_depthArrow.anchoredPosition; 
            Vector2 pos = _depthArrow.anchoredPosition;
            pos.y = -540 * (float) depthIndex / maxDepth;
            Vector2 targetPos = pos;
            
            Curve.Tween(SliderMoveCurve,
                _sliderMoveDuration,
                t =>
                {
                    _depthArrow.anchoredPosition = Vector2.Lerp(srcPos, targetPos, t);
                },
                () =>
                {
                    _depthArrow.anchoredPosition = targetPos;
                });
        }

        public void ShowReturnButton()
        {
            _returnButton.gameObject.SetActive(true);
        }
        
        public void HideReturnButton()
        {
            Sfx.Instance.Play("SwimUpButton");
            _returnButton.gameObject.SetActive(false);
        }

        public void CloseFlash()
        {
            Flash(_closeFlashInfo, () => SceneManager.LoadScene("End"));
        }

        private IEnumerator ScoreFloatingText(string text, TextMeshProUGUI textUI, string finalScore)
        {
            while (_isFloatingTextActive)
            {
                yield return new WaitForEndOfFrame();
            }

            _isFloatingTextActive = true;

            textUI.gameObject.SetActive(true);
            
            textUI.text = text;
            Vector3 srcPos = _gemText.transform.position + (Vector3.down * 50);
            textUI.transform.position = srcPos;
            
            Vector3 targetPos = _gemText.transform.position;

            for (float f = 0f; f < _floatingTextDuration; f += Time.deltaTime)
            {
                textUI.transform.position = Vector3.Lerp(srcPos, targetPos, f / _floatingTextDuration);
                textUI.alpha = Mathf.Lerp(1f, 0.5f, f / _floatingTextDuration);
                yield return new WaitForEndOfFrame();
            }

            textUI.transform.position = targetPos;
            textUI.gameObject.SetActive(false);
            _isFloatingTextActive = false;
            
            _gemText.text = finalScore;
        }
    }
}