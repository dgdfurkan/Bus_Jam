using UnityEngine;

namespace Runtime.Managers
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        [SerializeField] private float fastMoveSpeed;
        [SerializeField] private float minXRot, maxXRot;
        [SerializeField] private float minZoom, maxZoom;
        [SerializeField] private float zoomSpeed;
        [SerializeField] private float rotateSpeed;
        [SerializeField] private float curZoom;

        private Camera _cam;
        private float _curXRot;
        private float _curYRot;

        private Vector3 _currentPosition;
        private Vector3 _inputDirection = new Vector3(0, 0, 0);
        private Vector3 _moveDirection;
        private Vector2 _lastMousePosition;
        private Vector2 _mouseMovementDelta;
        private readonly float _moveSpeed = 20f;
        private readonly float _dragPanSpeed = .5f;
        private readonly int _edgeScrollSize = 20;
        private bool _useEdgeScrolling = false;
        private bool _useDragPan = false;
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _cam = Camera.main;
            curZoom = _cam.transform.localPosition.y;
            
            _currentPosition = transform.position;
            //transform.position = _cam.transform.position;
            //transform.eulerAngles = _cam.transform.eulerAngles;
        }

        private void Update()
        {
            //HandleCameraMovementKey();
            if(_useEdgeScrolling) HandleCameraMovementEdgeScrolling();
            if(_useDragPan) HandleCameraMovementDragPan();
            
            if(Input.GetKeyUp(KeyCode.R)) transform.position = _currentPosition;
            if(Input.GetKeyUp(KeyCode.F)) _useEdgeScrolling = !_useEdgeScrolling;
            if(Input.GetKeyUp(KeyCode.G)) _useDragPan = !_useDragPan;
            
            // ZoomCamera();
            // RotateCamera();
            // MoveCamera();
        }

        private void HandleCameraMovementKey()
        {
            if(Input.GetKey(KeyCode.W)) _inputDirection.z = +1;
            if(Input.GetKey(KeyCode.S)) _inputDirection.z = -1;
            if(Input.GetKey(KeyCode.A)) _inputDirection.x = -1;
            if(Input.GetKey(KeyCode.D)) _inputDirection.x = +1;
            
            _moveDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;
            
            transform.position += _moveDirection * (_moveSpeed * Time.deltaTime);
        }

        private void HandleCameraMovementEdgeScrolling()
        {
            if (!_useEdgeScrolling) return;
            if(Input.mousePosition.x < _edgeScrollSize) _inputDirection.x = -1;
            if(Input.mousePosition.y < _edgeScrollSize) _inputDirection.z = -1;
            if(Input.mousePosition.x > Screen.width - _edgeScrollSize) _inputDirection.x = +1;
            if(Input.mousePosition.y > Screen.height - _edgeScrollSize) _inputDirection.z = +1;
            
            _moveDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;
            
            transform.position += _moveDirection * (_moveSpeed * Time.deltaTime);
        }

        private void HandleCameraMovementDragPan()
        {
            if (Input.GetMouseButtonDown(1))
            {
                _useDragPan = true;
                _lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(1))
            {
                _useDragPan = false;
            }
            
            if (_useDragPan)
            {
                _mouseMovementDelta = (Vector2) Input.mousePosition - _lastMousePosition;
                _inputDirection.x = -_mouseMovementDelta.x * _dragPanSpeed;
                _inputDirection.z = -_mouseMovementDelta.y * _dragPanSpeed;
                _lastMousePosition = Input.mousePosition;
            }
            
            _moveDirection = transform.forward * _inputDirection.z + transform.right * _inputDirection.x;
            
            transform.position += _moveDirection * (_moveSpeed * Time.deltaTime);
        }
        
        private void ZoomCamera()
        {
            curZoom += Input.GetAxis("Mouse ScrollWheel") * -zoomSpeed;
            curZoom = Mathf.Clamp(curZoom, minZoom, maxZoom);
            var newPosition = _cam.transform.position;
            newPosition.y = curZoom;
            _cam.transform.position = newPosition;
        }

        private void RotateCamera()
        {
            if (!Input.GetMouseButton(1)) return;
            
            var mouseInput = new Vector3(-Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0f);
            transform.Rotate(mouseInput * rotateSpeed);
            var eulerRotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(eulerRotation.x, eulerRotation.y, 0f);
            
            // var x = Input.GetAxis("Mouse X");
            // var y = Input.GetAxis("Mouse Y");
            //
            // // Eğer mouse hareket etmiyorsa, fonksiyondan çık
            // if (Mathf.Approximately(x, 0) && Mathf.Approximately(y, 0)) return;
            //
            // _curXRot += rotateSpeed * -y;
            // _curXRot = Mathf.Clamp(_curXRot, minXRot, maxXRot);
            //
            // _curYRot += rotateSpeed * x;
            //
            // transform.eulerAngles = new Vector3(_curXRot, _curYRot, 0f);
        }

        private void MoveCamera()
        {
            var forward = _cam.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            var right = _cam.transform.right;
            
            var moveZ = Input.GetAxisRaw("Vertical");
            var moveX = Input.GetAxisRaw("Horizontal");

            var currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? fastMoveSpeed : moveSpeed; // More Speed
            
            var dir = forward * moveZ + right * moveX;
            dir.Normalize();
            dir *= currentMoveSpeed * Time.deltaTime;

            transform.position += dir;
        }
    }
}
