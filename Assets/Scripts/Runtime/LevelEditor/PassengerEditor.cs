using Runtime.Enums;
using UnityEngine;

namespace Runtime.LevelEditor
{
    public class PassengerEditor : MonoBehaviour
    {
        private static PassengerEditor _activePassenger;

        public ColorTypes colorType;
        public GameObject clone;
        
        private Camera _camera;
        
        private Vector3 _mOffset;
        private float _mZCameraDistance;
            
        private void Awake()
        {
            GetReferences();
        }
        
        private void GetReferences()
        {
            _camera = Camera.main;
        }
        
        public void Initialize(ColorTypes type)
        {
            colorType = type;
        }
        
        private void OnMouseDown()
        {
            clone = Instantiate(gameObject, transform.position, Quaternion.identity);
            clone.GetComponent<PassengerEditor>().enabled = false;
            clone.GetComponent<Collider>().enabled = false;
            
            _mZCameraDistance = _camera.WorldToScreenPoint(clone.gameObject.transform.position).z;
            _mOffset = clone.gameObject.transform.position - GetMouseWorldPos();
        }
        
        private void OnMouseDrag()
        {
            _mZCameraDistance = _camera.WorldToScreenPoint(clone.gameObject.transform.position).z;
            var newPosition = GetMouseWorldPos() + _mOffset;
            newPosition.y = clone.gameObject.transform.position.y; // Keep the y position constant
            clone.gameObject.transform.position = newPosition;
        }
        
        private void OnMouseUp()
        {
            var mouseWorldPos = GetMouseWorldPos();
            
            var ray = new Ray(mouseWorldPos, Vector3.down); // Aşağı doğru yönlendir
        
            if (Physics.Raycast(ray, out var hit))
            {
                if (hit.transform.TryGetComponent(out TileEditor tileEditorum))
                {
                    if (tileEditorum.cellData.gridTypes == GridTypes.Normal)
                    {
                        if (tileEditorum.cellData.colorTypes != ColorTypes.None && tileEditorum.transform.childCount > 0)
                        {
                            Destroy(tileEditorum.transform.GetChild(0).gameObject);
                        }
        
                        clone.transform.position = tileEditorum.transform.position;
                        clone.transform.SetParent(tileEditorum.transform);
                        clone = null;
                        tileEditorum.cellData.colorTypes = colorType;
                        LevelEditorManager.Instance.UpdateCellDatas(tileEditorum.cellData);
                    }
                    else
                    {
                        Destroy(clone);
                        clone = null;
                    }
                }
                else
                {
                    Destroy(clone);
                    clone = null;
                }
            }
            else
            {
                Destroy(clone);
                clone = null;
            }
        }
        
        private Vector3 GetMouseWorldPos()
        {
            var mousePoint = Input.mousePosition;
            mousePoint.z = _mZCameraDistance;
            return _camera.ScreenToWorldPoint(mousePoint);
        }
    }
}