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
        [SerializeField] private TextMeshProUGUI _gemText;
        [SerializeField] private RectTransform _depthArrow;
        [SerializeField] private TextMeshProUGUI _depthText;
        [SerializeField] private RectTransform _flagArrow;
        [SerializeField] private RectTransform _recordArrow;
        [SerializeField] private TextMeshProUGUI _recordText;
        [SerializeField] private RectTransform _returnButton;
        
        [SerializeField] private GameObject _oxygenBarPrefab;
        [SerializeField] private Transform _oxygenBarParent;
        [SerializeField] private FlashInfo _closeFlashInfo;
        
        public AnimationCurve SliderMoveCurve;
        private const float _sliderMoveDuration = 0.5f;
        
        [SerializeField] private TextMeshProUGUI _normalFloatingText;
        [SerializeField] private TextMeshProUGUI _gemFloatingText;
        private const float _floatingTextDuration = 0.5f;
        private bool _isFloatingTextActive = false;

        [SerializeField] private Color _oxygenBarAddColor;
        [SerializeField] private Color _oxygenBarRemoveColor;
        
        [SerializeField] private Button _refreshButton;

        [SerializeField] private GameObject _signs;

        private const int maxDepth = 50; // Source: depths of my ass
        private int _maxDepthCoefficient = 1;

        private int _currentOxy;
        private int _depthRecord;
        private bool _newRecord;
        
        void Start()
        {
            Flash(_openFlashInfo);
            _currentOxy = FindObjectOfType<GameMain>().Oxygen;

            _depthRecord = dreamloLeaderBoard.GetSceneDreamloLeaderboard().ToListHighToLow()[0].score / 1000;
            _newRecord = false;
            _maxDepthCoefficient = (int) Mathf.Pow(2.0f, _depthRecord / maxDepth);
            
            if (_depthRecord > 0)
            {
                _recordArrow.gameObject.SetActive(true);
                Vector2 pos = _depthArrow.anchoredPosition;
                pos.y = -540 * ((float) (_depthRecord + 1) / (maxDepth * _maxDepthCoefficient));
                _recordArrow.anchoredPosition = pos;
                _recordText.text = _depthRecord.ToString();
            }
        }

        public void OnClickRefresh()
        {
            Sfx.Instance.Play("FirstSplash");
            Sfx.Instance.FadeOutMusic(_closeFlashInfo.Duration - 0.1f);
            _refreshButton.interactable = false;
            Flash(_closeFlashInfo, () => SceneManager.LoadScene("Game"));
        }

        public void SetOxygen(int oxy)
        {
            if (oxy > _currentOxy)
            {
                int added = oxy - _currentOxy;
                
                for (int i = 0; i < added; i++)
                {
                    GameObject go = Instantiate(_oxygenBarPrefab, _oxygenBarParent);
                    go.GetComponent<Image>().color = _oxygenBarAddColor;
                    PlayOxygenBarAdd(_oxygenBarParent.GetChild(i).gameObject);
                }
            }
            else if (oxy < _currentOxy && _oxygenBarParent.childCount > 0)
            {
                int removed = _currentOxy - oxy;

                for (int i = 0; i < removed; i++)
                {
                    if (_oxygenBarParent.childCount <= i)
                    {
                        continue;
                    }
                    
                    int childIndex = i;
                    GameObject removedGo = _oxygenBarParent.GetChild(childIndex).gameObject;
                    PlayOxygenBarRemove(removedGo);
                }
            }

            _currentOxy = oxy;
        }

        private void PlayOxygenBarAdd(GameObject entryGo)
        {
            Image image = entryGo.GetComponent<Image>();
            Color srcColor = Color.white;
            Color targetColor = _oxygenBarAddColor;
            Curve.Tween(SliderMoveCurve,
                0.3f,
                t =>
                {
                    image.color = Color.Lerp(srcColor, targetColor, t);
                },
                () =>
                {
                    image.color = targetColor;
                });
        }

        private void PlayOxygenBarRemove(GameObject entryGo)
        {
            Image image = entryGo.GetComponent<Image>();
            Color srcColor = image.color;
            Color targetColor = _oxygenBarRemoveColor;
            Curve.Tween(SliderMoveCurve,
                0.3f,
                t =>
                {
                    image.color = Color.Lerp(srcColor, targetColor, t);
                },
                () =>
                {
                    image.color = targetColor;
                    Destroy(entryGo);
                });
        }
        
        public void SetScore(int gem, int delta = 0, ScoreType type = ScoreType.None)
        {
            switch (type)
            {
                case ScoreType.Depth:
                    CoroutineStarter.Run(ScoreFloatingText($"+{delta.ToString()}", _normalFloatingText, gem.ToString()));
                    break;
                case ScoreType.Gem:
                    CoroutineStarter.Run(ScoreFloatingText($"+{delta.ToString()}", _gemFloatingText, gem.ToString()));
                    break;
                default:
                    _gemText.text = $"{gem.ToString()}";
                    break;
            }
        }

        public void SetDepth(int depthIndex, GameDirection direction)
        {
            if (direction == GameDirection.Down)
            {
                _maxDepthCoefficient = (int)Mathf.Pow(2.0f, Mathf.Max(_depthRecord, depthIndex) / maxDepth);
            }
            if (depthIndex < _depthRecord || direction == GameDirection.Up)
            {
                _depthArrow.gameObject.SetActive(true);
                _depthText.text = (depthIndex + 1).ToString();
                Vector2 srcPos = _depthArrow.anchoredPosition; 
                Vector2 pos = srcPos;
                pos.y = -540 * ((float) (depthIndex + 1) / (maxDepth * _maxDepthCoefficient));
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
            else
            {
                _newRecord = true;
                _recordArrow.gameObject.SetActive(true);
                _depthArrow.gameObject.SetActive(false);
                
                _recordText.text = (depthIndex + 1).ToString();
                Vector2 srcPos = _recordArrow.anchoredPosition;
                Vector2 pos = srcPos;
                pos.y = -540 * ((float) (depthIndex + 1) / (maxDepth * _maxDepthCoefficient));
                Vector2 targetPos = pos;

                Curve.Tween(SliderMoveCurve,
                    _sliderMoveDuration,
                    t =>
                    {
                        _recordArrow.anchoredPosition = Vector2.Lerp(srcPos, targetPos, t);
                    },
                    () =>
                    {
                        _recordArrow.anchoredPosition = targetPos; 
                        
                    });
            }
        }

        public void ShowReturnButton()
        {
            _returnButton.gameObject.SetActive(true);
        }
        
        public void HideReturnButton()
        {
            if (!_newRecord)
            {
                _flagArrow.gameObject.SetActive(true);
                _flagArrow.transform.position = _depthArrow.transform.position;
            }
            
            Sfx.Instance.Play("SwimUpButton");
            _returnButton.gameObject.SetActive(false);
            
            _signs.SetActive(false);
        }

        public void CloseFlash()
        {
            Sfx.Instance.FadeOutMusic(_closeFlashInfo.Duration / 2f);
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
            Vector3 srcPos = _gemText.transform.position + (Vector3.down * 50)  + (Vector3.right * 10);
            textUI.transform.position = srcPos;
            
            Vector3 targetPos = _gemText.transform.position + (Vector3.down * 10) + (Vector3.right * 10);

            yield return Curve.Tween(SliderMoveCurve,
                _floatingTextDuration,
                t =>
                {
                    textUI.transform.position = Vector3.Lerp(srcPos, targetPos, t);
                    textUI.alpha = Mathf.Lerp(1f, 0.5f, t);
                });

            textUI.transform.position = targetPos;
            textUI.gameObject.SetActive(false);
            _isFloatingTextActive = false;
            
            _gemText.text = finalScore;
        }
    }
}