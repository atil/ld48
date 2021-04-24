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
        [SerializeField] private Button _returnButton;
        
        void Start()
        {
            Flash(_openFlashInfo);
        }

        public void SetOxygen(int oxy)
        {
            _oxygenText.text = $"{oxy.ToString()}";
        }
        
        public void SetGem(int gem)
        {
            _gemText.text = $"{gem.ToString()}";
        }

        public void OnReturnButtonClicked()
        {
            _returnButton.gameObject.SetActive(false);
        }
    }
}