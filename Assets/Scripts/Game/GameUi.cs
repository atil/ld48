using System.Collections;
using JamKit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class GameUi : UiBase
    {
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private TextMeshProUGUI _oxygenText;
        [SerializeField] private TextMeshProUGUI _gemText;
        [SerializeField] private Slider _slider;
        [SerializeField] private RectTransform _depthArrow;
        
        public AnimationCurve SliderMoveCurve;
        private const float _sliderMoveDuration = 0.5f;

        void Start()
        {
            Flash(_openFlashInfo);
        }

        public void SetOxygen(int oxy)
        {
            _oxygenText.text = $"{oxy.ToString()}";
            
            int initValue = (int)_slider.value;
            
            Curve.Tween(SliderMoveCurve,
                _sliderMoveDuration,
                t =>
                {
                    _slider.value = Mathf.Lerp(initValue, oxy, t);
                },
                () =>
                {
                    _slider.value = oxy;
                });
        }
        
        public void SetGem(int gem)
        {
            _gemText.text = $"{gem.ToString()}";
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
        
    }
}