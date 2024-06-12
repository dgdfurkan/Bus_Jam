using System;
using Runtime.Enums;
using Runtime.Managers;
using Runtime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.Handler
{
    public class UIEventSubscriber : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private UIEventSubscriptionTypes type;
        [SerializeField] private Button button;

        #endregion

        #region Private Variables

        [ShowInInspector] private UIManager _manager;

        #endregion

        #endregion

        private void Awake()
        {
            FindReferences();
        }

        private void FindReferences()
        {
            _manager = FindObjectOfType<UIManager>();
            button = button ? button : GetComponent<Button>();
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            switch (type)
            {
                case UIEventSubscriptionTypes.OnPlay:
                    button.onClick.AddListener(_manager.OnPlay);
                    break;
                case UIEventSubscriptionTypes.OnNextLevel:
                    button.onClick.AddListener(_manager.OnNextLevel);
                    break;
                case UIEventSubscriptionTypes.OnRestartLevel:
                    button.onClick.AddListener(_manager.OnRestartLevel);
                    break;
                case UIEventSubscriptionTypes.OnHome:
                    button.onClick.AddListener(_manager.OpenStartPanel);
                    break;
                case UIEventSubscriptionTypes.OnShop:
                    button.onClick.AddListener(_manager.OnShop);
                    break;
                case UIEventSubscriptionTypes.OnSkins:
                    button.onClick.AddListener(_manager.OnSkins);
                    break;
                case UIEventSubscriptionTypes.OnWinStreak:
                    button.onClick.AddListener(_manager.OnWinStreak);
                    break;
                case UIEventSubscriptionTypes.OnLeaderboard:
                    button.onClick.AddListener(_manager.OnLeaderboard);
                    break;
                case UIEventSubscriptionTypes.OnSettings:
                    button.onClick.AddListener(_manager.OnSettings);
                    break;
                case UIEventSubscriptionTypes.OnCredits:
                    button.onClick.AddListener(_manager.OnCredits);
                    break;
                case UIEventSubscriptionTypes.OnCloseTopPanel:
                    button.onClick.AddListener(CoreUISignals.Instance.OnCloseTopPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void UnSubscribeEvents()
        {
            switch (type)
            {
                case UIEventSubscriptionTypes.OnPlay:
                    button.onClick.RemoveListener(_manager.OnPlay);
                    break;
                case UIEventSubscriptionTypes.OnNextLevel:
                    button.onClick.RemoveListener(_manager.OnNextLevel);
                    break;
                case UIEventSubscriptionTypes.OnRestartLevel:
                    button.onClick.RemoveListener(_manager.OnRestartLevel);
                    break;
                case UIEventSubscriptionTypes.OnHome:
                    button.onClick.RemoveListener(_manager.OpenStartPanel);
                    break;
                case UIEventSubscriptionTypes.OnShop:
                    button.onClick.RemoveListener(_manager.OnShop);
                    break;
                case UIEventSubscriptionTypes.OnSkins:
                    button.onClick.RemoveListener(_manager.OnSkins);
                    break;
                case UIEventSubscriptionTypes.OnWinStreak:
                    button.onClick.RemoveListener(_manager.OnWinStreak);
                    break;
                case UIEventSubscriptionTypes.OnLeaderboard:
                    button.onClick.RemoveListener(_manager.OnLeaderboard);
                    break;
                case UIEventSubscriptionTypes.OnSettings:
                    button.onClick.RemoveListener(_manager.OnSettings);
                    break;
                case UIEventSubscriptionTypes.OnCredits:
                    button.onClick.RemoveListener(_manager.OnCredits);
                    break;
                case UIEventSubscriptionTypes.OnCloseTopPanel:
                    button.onClick.RemoveListener(CoreUISignals.Instance.OnCloseTopPanel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnDisable()
        {
            UnSubscribeEvents();
        }
    }
}