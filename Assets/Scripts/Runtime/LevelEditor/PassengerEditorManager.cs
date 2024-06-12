using System.Collections.Generic;
using _Modules.ObjectPooling.Scripts.Enums;
using _Modules.ObjectPooling.Scripts.Signals;
using Runtime.Datas.ValueObjects;
using Runtime.Enums;
using Runtime.Extentions;
using UnityEngine;

namespace Runtime.Managers
{
    public class PassengerEditorManager: MonoBehaviour
    {
        public static PassengerEditorManager Instance;
        
        [Header("Selected Passenger")]
        public PassengerEditor selectedPassengerEditor;
        [Space(10)]
        
        [Header("References")]
        [Space(10)]
        public Transform passengerParent;
        [SerializeField] private List<PassengerEditor> passengerEditors;
        
        private Camera _camera;
        private GameObject _clone;
        private int _currentPassengerEditorIndex = 0;
        
        public GameObject PoolObject;

        private void Awake()
        {
            Instance = this;
            _camera = Camera.main;
        }

        private void Start()
        {
            foreach (var passengerEditor in passengerEditors)
            {
                passengerEditor.OnClick += HandlePassengerEditorClick;
            }
        }

        private void HandlePassengerEditorClick(PassengerEditor passengerEditor)
        {
            if (selectedPassengerEditor == passengerEditor)
            {
                DestroyIt();
            }
            else
            {
                selectedPassengerEditor = passengerEditor;
                CreateClone();
            }
        }

        private void Update()
        {
            if (selectedPassengerEditor is null) return;
            if (_clone is not null)
            {
                var mousePos = GetMouseWorldPos();
                _clone.transform.position = new Vector3(mousePos.x, _clone.transform.position.y, mousePos.z + 0.2f);
            }

            if (Input.GetMouseButtonDown(0))
            {
                HandleMouseClick();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                DestroyIt();
            }
            
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll == 0f) return;
            
            _currentPassengerEditorIndex += (int)Mathf.Sign(scroll);
            _currentPassengerEditorIndex = (_currentPassengerEditorIndex + passengerEditors.Count) % passengerEditors.Count;

            DestroyIt();
            selectedPassengerEditor = passengerEditors[_currentPassengerEditorIndex];
            CreateClone();
        }

        private void CreateClone()
        {
            _clone = Instantiate(selectedPassengerEditor.gameObject, selectedPassengerEditor.transform.position, Quaternion.identity);
            
            _clone.SetColliderPassengerEditor(false);
        }
        
        private void HandleMouseClick()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out TileEditor tileEditor))
                {
                    if (tileEditor.cellData.gridType != GridTypes.Normal) return;
                    
                    foreach (Transform child in passengerParent)
                    {
                        if (child.position != tileEditor.transform.position) continue;
                        child.gameObject.SetColliderPassengerEditor(true);
                        PoolSignals.Instance.OnSetPooledGameObject?.Invoke(child.gameObject, PoolTypes.PassengerEditor);
                        break;
                    }
                    
                    var newPassenger = PoolSignals.Instance.OnGetPoolableGameObject?.Invoke(PoolTypes.PassengerEditor, passengerParent, tileEditor.transform.position, Quaternion.identity);
                    newPassenger?.transform.SetParent(passengerParent.transform);
                    tileEditor.cellData.passengerArea.colorType = selectedPassengerEditor.colorType;
                        
                    newPassenger.SetColliderPassengerEditor(false);
                    
                    var cellCopy = new CellArea()
                    {
                        position = tileEditor.cellData.position,
                        gridType = tileEditor.cellData.gridType,
                        passengerArea = tileEditor.cellData.passengerArea
                    };
                    
                    LevelEditorManager.Instance.UpdateCellDatas(cellCopy);
                }
                else
                {
                    DestroyIt();
                }
            }
            else
            {
                DestroyIt();
            }
        }

        private void DestroyIt()
        {
            selectedPassengerEditor = null;
            if (_clone is null) return;
            Destroy(_clone);
            _clone = null;
        }
        
        private Vector3 GetMouseWorldPos()
        {
            var mousePoint = Input.mousePosition;
            mousePoint.z = _camera.WorldToScreenPoint(_clone.transform.position).z;
            return _camera.ScreenToWorldPoint(mousePoint);
        }
    }
}

