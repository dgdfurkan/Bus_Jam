using Runtime.Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.Managers
{
    [RequireComponent(typeof(PlayerInput))]
    public class InputManager : MonoBehaviour
    {
        #region Self Variables

        #region Public  Variables

        //

        #endregion

        #region Serialized Variables

        //
    
        #endregion

        #region Private Variables
    
        private PlayerInput _playerInputAction;
        private InputAction _leftClickAction;
        private InputAction _rightClickAction;
        private InputAction _mousePositionAction;
        private InputAction _scrollAction;
        
        #endregion

        #endregion

        private void Awake()
        {
            _playerInputAction = GetComponent<PlayerInput>();
            _leftClickAction = _playerInputAction.currentActionMap.FindAction("LeftClick");
            _rightClickAction = _playerInputAction.currentActionMap?.FindAction("RightClick");
            _mousePositionAction = _playerInputAction.currentActionMap?.FindAction("MousePosition");
            _scrollAction = _playerInputAction.currentActionMap?.FindAction("Scroll");
        }

        private void OnEnable()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (_leftClickAction is not null)
            {
                _leftClickAction.started += OnLeftClick;
                _leftClickAction.Enable();
            }
            
            if (_rightClickAction is not null)
            {
                _rightClickAction.started += OnRightClick;
                _rightClickAction.Enable();
            }
            
            if (_mousePositionAction is not null)
            {
                _mousePositionAction.performed += OnMousePosition;
                _mousePositionAction.Enable();
            }
            
            if (_scrollAction is not null)
            {
                _scrollAction.performed += OnScroll;
                _scrollAction.Enable();
            }
        }

        private void UnsubscribeEvents()
        {
            if (_leftClickAction is not null)
            {
                _leftClickAction.started -= OnLeftClick;
                _leftClickAction.Disable();
            }
            
            if (_rightClickAction is not null)
            {
                _rightClickAction.started -= OnRightClick;
                _rightClickAction.Disable();
            }
            
            if (_mousePositionAction is not null)
            {
                _mousePositionAction.performed -= OnMousePosition;
                _mousePositionAction.Disable();
            }
            
            if (_scrollAction is not null)
            {
                _scrollAction.performed -= OnScroll;
                _scrollAction.Disable();
            }
        }

        private void OnDisable()
        {
            UnsubscribeEvents();
        }
    
        private void OnLeftClick(InputAction.CallbackContext context)
        {
            if (Touchscreen.current is not null)
            {
                //Debug.Log("Touch pressed");
                if (Touchscreen.current.primaryTouch.press.isPressed)
                {
                    Ray(Touchscreen.current.primaryTouch.position.ReadValue());
                }
            }

            if (Mouse.current is null) return;
            if (!Mouse.current.leftButton.isPressed) return;
            //Debug.Log("Mouse left button pressed");
            Ray(Mouse.current.position.ReadValue());
        }

        private static void Ray(Vector2 position)
        {
            if(Camera.main is null) return;
        
            var ray = Camera.main.ScreenPointToRay(position);

            if (!Physics.Raycast(ray, out var hit)) return;
        
            Debug.Log($"Hit object: {hit.transform.name}");
        
            if (hit.transform.TryGetComponent<IClickable>(out var click))
            {
                click.Click();
            }
            
            // if (hit.transform.TryGetComponent<TileEditor>(out var tileEditor))
            // {
            //     tileEditor.Touch();
            // }
            
            //
        }
        
        private void OnRightClick(InputAction.CallbackContext context)
        {
            // if (Mouse.current is null) return;
            // if (!Mouse.current.rightButton.isPressed) return;
            
            Debug.Log("Mouse right button pressed");
        }
        
        private void OnMousePosition(InputAction.CallbackContext context)
        {
            if (Mouse.current is null) return;
        }
        
        private void OnScroll(InputAction.CallbackContext context)
        {
            var scrollValue = context.ReadValue<float>();
        }
    }
}
