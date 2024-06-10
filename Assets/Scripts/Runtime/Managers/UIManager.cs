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
            //OpenStartPanel();
        }

        private void SubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize += OnLevelInitialize;
            CoreGameSignals.OnLevelFailed += OnLevelFailed;
            CoreGameSignals.OnLevelSuccessful += OnLevelSuccessful;
            CoreGameSignals.OnReset += OnReset;
        }
        
        private void OnLevelInitialize(int levelValue)
        {
            OpenStartPanel();
            //UISignals.onSetNewLevelValue?.Invoke(levelValue);
        }
        
        private void OnLevelFailed()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Fail, 2);
        }
        
        private void OnLevelSuccessful()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Win, 2);
        }
        
        private void OnReset()
        {
            CoreUISignals.OnCloseAllPanels?.Invoke();
        }

        private void UnsubscribeEvents()
        {
            CoreGameSignals.OnLevelInitialize -= OnLevelInitialize;
            CoreGameSignals.OnLevelFailed -= OnLevelFailed;
            CoreGameSignals.OnLevelSuccessful -= OnLevelSuccessful;
            CoreGameSignals.OnReset -= OnReset;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        public void OpenStartPanel()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Home, 0);
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.SelectButton, 2);
        }

        #region UIEventSubscribers

        public void OnPlay()
        {
            CoreGameSignals.OnPlay?.Invoke();
            CoreUISignals.OnCloseAllPanels?.Invoke();
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Level, 0);
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
            //CameraSignals.Instance.onChangeCameraState?.Invoke(CameraStates.Follow);
        }
        
        public void OnNextLevel()
        {
            CoreGameSignals.OnNextLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
        }
        
        public void OnRestartLevel()
        {
            CoreGameSignals.OnRestartLevel?.Invoke();
            CoreGameSignals.OnReset?.Invoke();
        }

        public void OnShop()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Shop, 0);
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
        }
        
        public void OnSkins()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Skins, 0);
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Health, 1);
        }
        
        public void OnWinStreak()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.WinStreak, 0);
            CoreUISignals.OnClosePanel?.Invoke(1);
        }
        
        public void OnLeaderboard()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Leaderboard, 0);
            CoreUISignals.OnClosePanel?.Invoke(1);
        }
        
        public void OnSettings()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Settings, 3);
        }
        
        public void OnCredits()
        {
            CoreUISignals.OnOpenPanel?.Invoke(UIPanelTypes.Credits, 4);
        }

        #endregion
    }
}