using JamKit;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameUi : UiBase
    {
        [SerializeField] private FlashInfo _openFlashInfo;
        [SerializeField] private TextMeshProUGUI _oxygenText;
        [SerializeField] private TextMeshProUGUI _gemText;
        
        void Start()
        {
            Flash(_openFlashInfo);
        }

        public void SetOxygen(int oxy)
        {
            _oxygenText.text = $"O2: {oxy.ToString()}";
        }
        
        public void SetGem(int gem)
        {
            _gemText.text = $"Gem: {gem.ToString()}";
        }
    }
}