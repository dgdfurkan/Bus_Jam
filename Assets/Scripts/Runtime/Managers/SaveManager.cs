using Runtime.Keys;
using Runtime.Signals;
using UnityEngine;

namespace Runtime.Managers
{
    public class SaveManager : MonoBehaviour
    {
        private readonly string _filePath = "SaveFile.es3";
        
        #region Subscribe and Unsubscribe Events

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            SaveSignals.OnSaveGameData += SaveData;
            SaveSignals.OnLoadSaveData += LoadData;
        }

        private void UnsubscribeEvents()
        {
            SaveSignals.OnSaveGameData -= SaveData;
            SaveSignals.OnLoadSaveData -= LoadData;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }

        #endregion

        private void SaveData(int levelID)
        {
            OnSaveGame(
                new SaveGameDataParams()
                {
                    Level = levelID,
                }
            );
            Debug.Log("Bu level saved" + ES3.Load<int>("Level"));
            Debug.Log("Bu level savedd" + CoreGameSignals.Instance.OnGetLevelID.Invoke());
        }
        
        private void OnSaveGame(SaveGameDataParams saveDataParams)
        {
            ES3.Save("Level", saveDataParams.Level, _filePath);
            CoreGameSignals.Instance.OnSetLevelID.Invoke(saveDataParams.Level);
        }

        private void LoadData()
        {
            if (!ES3.FileExists(_filePath))
            {
                ES3.Save("Level", 0, _filePath);
            }
            
            var saveDataParams = new SaveGameDataParams
            {
                Level = ES3.Load("Level", 0)
            };
            CoreGameSignals.Instance.OnSetLevelID.Invoke(saveDataParams.Level);
            Debug.Log("Bu level loaded" + ES3.Load<int>("Level"));
            Debug.Log("Bu level loadedd" + CoreGameSignals.Instance.OnGetLevelID.Invoke());
        }
    }
}