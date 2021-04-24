﻿using System.Collections;
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
    }
}