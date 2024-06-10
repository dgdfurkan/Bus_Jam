using UnityEngine;

namespace Runtime.Extentions
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static volatile T _instance;

        public static T Instance
        {
            get
            {
                if (_instance is not null) return _instance;
                _instance = FindObjectOfType<T>();
                if (_instance is not null) return _instance;
                var newGo = new GameObject(_instance?.GetType().Name);
                _instance = newGo.AddComponent<T>();

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this as T;
        }
    }
}