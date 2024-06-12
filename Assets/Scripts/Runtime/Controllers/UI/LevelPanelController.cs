using System;
using System.Globalization;
using DG.Tweening;
using Runtime.Datas.ValueObjects;
using Runtime.Signals;
using TMPro;
using UnityEngine;

namespace Runtime.Controllers.Objects.UI
{
    public class LevelPanelController: MonoBehaviour
    {
        #region Self Variables

        #region Public Variables
        
        //

        #region Serialized Variables
        
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI timerText;
        [SerializeField] private GameObject clickHand;
        #endregion

        #region Self Variables

        private bool _isTimerRunning = false;
        private float _levelID;
        private float _time;
        private Tween _timerTween;
        private LevelData _levelData;
        
        #endregion
        
        #endregion

        #endregion

        private void OnEnable()
        {
            LevelInitialize();
            CoreGameSignals.Instance.OnFirstInputTaken += OnFirstInputTaken;
            
        }
        
        private void LevelInitialize()
        {
            _levelData = CoreGameSignals.Instance.OnGetCurrentLevelData.Invoke();
            _levelID = _levelData.levelID;
            levelText.text = _levelID.ToString();
            _time = _levelData.time;
            _isTimerRunning = false;
            var timeSpan = TimeSpan.FromSeconds(_time);
            timerText.text = timeSpan.ToString("mm\\:ss");
        }
        
        private void OnFirstInputTaken()
        {
            clickHand.SetActive(false);
            StartTimer();
        }
        
        private void OnDisable()
        {
            _timerTween.Kill();
            CoreGameSignals.Instance.OnFirstInputTaken -= OnFirstInputTaken;
        }

        private void StartTimer()
        {
            if(_isTimerRunning) return;
            _isTimerRunning = true;
            
            _timerTween = DOVirtual.Float(_time, 0, _time, UpdateTimer).SetEase(Ease.Linear).OnComplete(() =>
            {
                _isTimerRunning = false;
                CoreGameSignals.Instance.OnLevelFailed.Invoke();
            });
        }
        
        private void UpdateTimer(float value)
        {
            var timeSpan = TimeSpan.FromSeconds(value);
            timerText.text = timeSpan.ToString("mm\\:ss");
        }
    }
}