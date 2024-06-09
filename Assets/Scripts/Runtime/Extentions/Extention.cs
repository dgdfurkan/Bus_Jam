using System.Linq;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using DG.Tweening;
using Runtime.LevelEditor;
using TMPro;
using UnityEngine;

namespace Runtime.Extentions
{
    public static class Extention
    {
        public static void CreateAndFadeOut(this TextMeshProUGUI original, Transform parent, string message
            ,float duration, Color color = default)
        {
            if (color == default) color = Color.white;
            var text = Object.Instantiate(original, parent);
            text.transform.position = new Vector3(original.transform.position.x, original.transform.position.y, original.transform.position.z - 1);
            text.enabled = true;
            text.color = color;
            text.text = message;
            text.DOFade(0, duration).OnComplete(() => Object.Destroy(text.gameObject));
        }
        
        public static void ClearChildren(this Transform transform, PoolTypes type = default)
        {
            transform.Cast<Transform>().ToList().ForEach(child 
                => PoolSignals.OnSetPooledGameObject?.Invoke(child.gameObject, type));
        }
        
        public static void SetColliderPassengerEditor(this GameObject gameObject, bool value)
        {
            var passengerCollider = gameObject.GetComponent<Collider>();
            var passengerEditor = gameObject.GetComponent<PassengerEditor>();
            
            if (passengerCollider is not null) passengerCollider.enabled = value;
            if (passengerEditor is not null) passengerEditor.enabled = value;
        }
    }
}

