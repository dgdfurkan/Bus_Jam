using UnityEngine;

namespace Runtime.LevelEditor
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
        
        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            _cam = Camera.main;
            curZoom = _cam.transform.localPosition.y;
            
            transform.position = _cam.transform.position;
            transform.eulerAngles = _cam.transform.eulerAngles;
        }

        private void Update()
        {
            ZoomCamera();
            RotateCamera();
            MoveCamera();
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
