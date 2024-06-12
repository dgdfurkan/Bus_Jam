using System.Collections.Generic;
using Runtime.Enums;
using Runtime.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Runtime.Controllers.Objects.UI
{
    public class UIPanelController : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables

        [SerializeField] private List<GameObject> layers = new List<GameObject>();

        #endregion

        #endregion
        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            CoreUISignals.Instance.OnOpenPanel += OnOpenPanel;
            CoreUISignals.Instance.OnClosePanel += OnClosePanel;
            CoreUISignals.Instance.OnCloseTopPanel += OnCloseTopPanel;
            CoreUISignals.Instance.OnCloseAllPanels += OnCloseAllPanel;
        }
        
        [Button("OpenPanel")]
        private void OnOpenPanel(UIPanelTypes panel, int layerValue)
        {
            CoreUISignals.Instance.OnClosePanel?.Invoke(layerValue);
            Instantiate(Resources.Load<GameObject>($"Screens/{panel}Panel"), layers[layerValue].transform);
        }
        
        [Button("ClosePanel")]
        private void OnClosePanel(int layerValue)
        {
            if (layers[layerValue].transform.childCount <= 0) return;
            
            for (var i = 0; i < layers[layerValue].transform.childCount; i++)
            {
                Destroy(layers[layerValue].transform.GetChild(i).gameObject);
            }
        }
        
        [Button("CloseTopPanel")]
        private void OnCloseTopPanel()
        {
            for (var i = layers.Count - 1; i > 0; i--)
            {
                if (layers[i].transform.childCount <= 0) continue;
                
                OnClosePanel(i);
                break;
            }
        }
        
        [Button("CloseAllPanels")]
        private void OnCloseAllPanel()
        {
            foreach (var layer in layers)
            {
                for (var i = 0; i < layer.transform.childCount; i++)
                {
                    Destroy(layer.transform.GetChild(i).gameObject);
                }
            }
        }

        private void UnsubscribeEvents()
        {
            CoreUISignals.Instance.OnOpenPanel -= OnOpenPanel;
            CoreUISignals.Instance.OnClosePanel -= OnClosePanel;
            CoreUISignals.Instance.OnCloseTopPanel -= OnCloseTopPanel;
            CoreUISignals.Instance.OnCloseAllPanels -= OnCloseAllPanel;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
    }
}