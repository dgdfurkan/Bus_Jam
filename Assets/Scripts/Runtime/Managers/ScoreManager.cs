using Runtime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        #region Self Variables

        #region Private Variables

        [ShowInInspector] private int _money;
        [ShowInInspector] private int _heart;
        [ShowInInspector] private bool _isFirstTime;

        #endregion

        #endregion
        
        private void Awake()
        {
            _money = GetMoneyValue();
        }
        
        private int GetMoneyValue()
        {
            if (!ES3.FileExists()) return 0;
            return ES3.KeyExists("Money") ? ES3.Load<int>("Money") : 0;
        }
        
        private void OnEnable()
        {
            SubscribeEvents();
        }
        
        private void SubscribeEvents()
        {
            ScoreSignals.OnGetMoney = () => _money;
            ScoreSignals.OnSendMoney += OnSendMoney;
            CoreGameSignals.OnReset += OnReset;
            CoreGameSignals.OnLevelSuccessful += RefreshMoney;
            CoreGameSignals.OnLevelFailed += RefreshMoney;
        }
        
        private void OnSendMoney(int value)
        {
            _money = value;
        }
        
        private void RefreshMoney()
        {
            //UISignals.OnSetMoneyValue?.Invoke(_money);
        }
        
        private void UnsubscribeEvents()
        {
            ScoreSignals.OnGetMoney = () => _money;
            ScoreSignals.OnSendMoney -= OnSendMoney;
            CoreGameSignals.OnReset -= OnReset;
            CoreGameSignals.OnLevelSuccessful -= RefreshMoney;
            CoreGameSignals.OnLevelFailed -= RefreshMoney;
        }
        
        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        private void OnReset()
        {
            _money = 0;
        }
    }
}