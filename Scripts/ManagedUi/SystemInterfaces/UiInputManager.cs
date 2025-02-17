using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ManagedUi.SystemInterfaces
{
public class UiInputManager : MonoBehaviour
{

    public Action OnLeft;
    public Action OnRight;
    public Action OnUp;
    public Action OnDown;

    public Action OnConfirm;

    private int currentSecond = 0;

    public bool randomInput = false;
    private Vector2 _savedDirection;


    private void OnEnable()
    {
        var navigate = InputSystem.actions.FindAction("Navigate");
        navigate.performed += context => OnNavigate(context.ReadValue<Vector2>());
        var confirmed = InputSystem.actions.FindAction("Submit");
        confirmed.performed += context => OnConfirm?.Invoke();
    }

    private void OnNavigate(Vector2 direction)
    {
        // see if we are currently stopping
        if (direction.magnitude > 0.5f)
        {
            _savedDirection = direction;
            return;
        }
        // check if we ever were navigating
        if (_savedDirection.magnitude < 0.5f)
        {
            return;
        }
        
        // we stopped and want to set a value
        if (Mathf.Abs(_savedDirection.y) > Mathf.Abs(_savedDirection.x))
        {
            if (_savedDirection.y > 0)
            {
                OnUp?.Invoke();
            }
            else
            {
                OnDown?.Invoke();
            }
        }
        else
        {
            if (_savedDirection.x > 0)
            {
                OnRight?.Invoke();
            }
            else
            {
                OnLeft?.Invoke();
            }
        }
        _savedDirection = Vector2.zero;

    }

    void Update()
    {
        if (randomInput)
        {
            RandomInput();
        }
        ;
    }

    void RandomInput()
    {
        if (currentSecond != (int)(Time.time))
        {
            currentSecond++;

            switch (currentSecond%10)
            {
                case 0:
                    OnLeft?.Invoke();
                    break;
                case 2:
                    OnRight?.Invoke();
                    break;
                case 4:
                    OnUp?.Invoke();
                    break;
                case 6:
                    OnDown?.Invoke();
                    break;
                case 8:
                    OnConfirm?.Invoke();
                    break;
                default:
                    break;
            }
        }
    }



}
}