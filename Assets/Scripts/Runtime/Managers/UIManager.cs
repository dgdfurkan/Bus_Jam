using Runtime.Enums;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Managers
{
    public class UIManager : MonoBehaviour
    {
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize += OnLevelInitialize;
            CoreGameSignals.Instance.OnLevelFailed += OnLevelFailed;
            CoreGameSignals.Instance.OnLevelSuccessful += OnLevelSuccessful;
            CoreGameSignals.Instance.OnReset += OnReset;
        }
        
        private void OnLevelInitialize(int levelValue)
        {
            OpenStartPanel();
        }
        
        private void OnLevelFailed()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Fail, 2);
        }
        
        private void OnLevelSuccessful()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Win, 2);
        }
        
        private void OnReset()
        {
            CoreUISignals.Instance.OnCloseAllPanels?.Invoke();
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.Instance.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.Instance.OnLevelFailed -= OnLevelFailed;
            CoreGameSignals.Instance.OnLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.Instance.OnReset -= OnReset;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        public void OpenStartPanel()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Home, 0);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.SelectButton, 2);
        }

        #region UIEventSubscribers

        public void OnPlay()
        {
            CoreGameSignals.Instance.OnPlay?.Invoke();
            CoreUISignals.Instance.OnCloseAllPanels?.Invoke();
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Level, 0);
        }
        
        public void OnNextLevel()
        {
            CoreUISignals.Instance.OnCloseAllPanels?.Invoke();
            CoreGameSignals.Instance.OnNextLevel?.Invoke();
            CoreGameSignals.Instance.OnReset?.Invoke();
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Home, 0);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.SelectButton, 2);
        }
        
        public void OnRestartLevel()
        {
            CoreUISignals.Instance.OnCloseAllPanels?.Invoke();
            CoreGameSignals.Instance.OnRestartLevel?.Invoke();
            CoreGameSignals.Instance.OnReset?.Invoke();
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Level, 0);
        }

        public void OnShop()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Shop, 0);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
        }
        
        public void OnSkins()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Skins, 0);
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
        }
        
        public void OnWinStreak()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.WinStreak, 0);
            CoreUISignals.Instance.OnClosePanel?.Invoke(1);
        }
        
        public void OnLeaderboard()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Leaderboard, 0);
            CoreUISignals.Instance.OnClosePanel?.Invoke(1);
        }
        
        public void OnSettings()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Settings, 3);
        }
        
        public void OnCredits()
        {
            CoreUISignals.Instance.OnOpenPanel?.Invoke(UIPanelTypes.Credits, 4);
        }

        #endregion
    }
}