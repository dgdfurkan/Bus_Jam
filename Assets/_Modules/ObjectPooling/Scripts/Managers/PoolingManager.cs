using System.Collections.Generic;
using _Modules.ObjectPooling.Scripts.Data.UnityObjects;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Modules.ObjectPooling.Scripts.Managers
{
    public class PoolingManager : MonoBehaviour
    {
        #region Self Variables

        #region Serialized Variables
        
        [SerializeField] private Transform poolParent;
        [SerializeField] private GameObject hapYutan;

        #endregion

        #region Private Variables

        private CD_Pool _data;
        [ShowInInspector]
        private Dictionary<PoolTypes, Queue<GameObject>> _poolableObjectList;

        #endregion

        #endregion
        
        private void Awake()
        {
            _data = GetPoolData();
        }

        private CD_Pool GetPoolData()
        {
            return Resources.Load<CD_Pool>("Datas/Pools/CD_Pool");
        }

        #region Subscribe and Unsubscribe Events
        
        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            PoolSignals.OnGetPoolableGameObject += OnGetPoolableGameObject;
            PoolSignals.OnSetPooledGameObject += OnSetPooledGameObject;
            PoolSignals.OnGetPrefabScale += OnGetPrefabScale;
        }

        private GameObject OnGetPoolableGameObject(PoolTypes type, Transform parent = null, Vector3 position = default
            ,Quaternion rotation = default)
        {
            if (!_poolableObjectList.TryGetValue(type, out var value))
            {
                Debug.LogError($"Dictionary does not contain this key: {type}...");
                return null;
            }
            
            if (value.Count == 0) // Expand pool if necessary
            {
                var newObject = Instantiate(_data.PoolList[type].prefab, parent, true);
                newObject.transform.position = position;
                newObject.transform.rotation = rotation;
                newObject.SetActive(true);
                SetNewObject(type, newObject);
                return newObject;
            }
            
            var deQueuedPoolObject = _poolableObjectList[type].Dequeue();
            if (deQueuedPoolObject.activeSelf) return OnGetPoolableGameObject(type, parent, position, rotation);
            deQueuedPoolObject.transform.parent = parent;
            deQueuedPoolObject.transform.position = position;
            deQueuedPoolObject.transform.rotation = rotation;
            deQueuedPoolObject.SetActive(true);
            return deQueuedPoolObject;
        }

        private void OnSetPooledGameObject(GameObject poolObject, PoolTypes type)
        {
            if (!_poolableObjectList.ContainsKey(type))
            {
                Debug.LogError($"Dictionary does not contain this key: {type}...");
                return;
            }
            
            poolObject.transform.parent = poolParent.transform.Find(type.ToString()) 
                                          ?? new GameObject(type.ToString()) { transform = { parent = poolParent.transform } }.transform;            
            //poolObject.transform.parent = poolParent.transform;
            //poolObject.transform.localScale = Vector3.one;
            poolObject.transform.localPosition = Vector3.zero;
            poolObject.transform.localEulerAngles = Vector3.zero;

            poolObject.gameObject.SetActive(false);

            _poolableObjectList[type].Enqueue(poolObject);
        }
        
        private void SetNewObject(PoolTypes type, GameObject newObject)
        {
            if (!_poolableObjectList.ContainsKey(type))
            {
                print("Dictionary does not contain this key...");
                _poolableObjectList[type] = new Queue<GameObject>();
            }

            print("Enqueue new object...");
            _poolableObjectList[type].Enqueue(newObject);
        }

        private Vector3 OnGetPrefabScale(PoolTypes type)
        {
            if (_poolableObjectList.TryGetValue(type, out var value)) return value.Peek().transform.localScale;
            Debug.LogError($"Dictionary does not contain this key: {type}...");
            return Vector3.one;
        }
        
        private void UnsubscribeEvents()
        {
            PoolSignals.OnGetPoolableGameObject -= OnGetPoolableGameObject;
            PoolSignals.OnSetPooledGameObject -= OnSetPooledGameObject;
            PoolSignals.OnGetPrefabScale -= OnGetPrefabScale;
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
        
        #endregion

        private void Start()
        {
            Setup();
        }

        private void Setup()
        {
            if (_data == null)
            {
                Debug.LogError("Pool data is null in Setup.");
                return;
            }
            
            _poolableObjectList = new Dictionary<PoolTypes, Queue<GameObject>>();

            foreach (var pool in _data.PoolList)
            {
                var poolableObjects = new Queue<GameObject>();

                for (var i = 0; i < pool.Value.amount; i++)
                {
                    var go = Instantiate(pool.Value.prefab, poolParent, true);
                    go.transform.parent = poolParent.transform.Find(pool.Key.ToString()) 
                                                  ?? new GameObject(pool.Key.ToString()) { transform = { parent = poolParent.transform } }.transform;
                    go.SetActive(false);
                    pool.Value.type = pool.Key;

                    poolableObjects.Enqueue(go);
                }

                _poolableObjectList.Add(pool.Key, poolableObjects);
            }
        }
    }
}