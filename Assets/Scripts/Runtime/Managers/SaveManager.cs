using Runtime.Keys;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Managers
{
    public class SaveManager
    {
        #region Subscribe and Unsubscribe Events

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            SaveSignals.OnSaveGameData += SaveData;
        }

        private void UnsubscribeEvents()
        {
            SaveSignals.OnSaveGameData -= SaveData;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        private void SaveData()
        {
            Debug.LogWarning(ScoreSignals.OnGetMoney());
            OnSaveGame(
                new SaveGameDataParams()
                {
                    Money = ScoreSignals.OnGetMoney(),
                    Level = CoreGameSignals.OnGetLevelID(),
                    //IsFirstStart = CoreGameSignals.OnGetIncomeLevel(),
                    //Heart = CoreGameSignals.OnGetHeart()
                }
            );
        }
        
        private void OnSaveGame(SaveGameDataParams saveDataParams)
        {
            ES3.Save("Level", saveDataParams.Level);
            ES3.Save("Money", saveDataParams.Money);
            ES3.Save("Heart", saveDataParams.Heart);
            ES3.Save("IsFirstStart", saveDataParams.IsFirstStart);
        }
    }
}