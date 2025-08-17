using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public Vector2 MovementInput   { get; private set; }
    public Vector2 MouseDelta      { get; private set; }
    public bool    IsSprintPressed { get; private set; }
    private Action<InputAction.CallbackContext>[] slotActions = new Action<InputAction.CallbackContext>[5];
    public bool IsInteract { get; private set; }
    public bool IsRecoding { get; private set; }
    private PlayerInput playerInput;
    public event Action<int> OnQuickSlotPressed;
    public event Action      OnLeftClick;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Player.Enable();
        playerInput.Player.LeftClick.performed += OnLeftClickInput;

        SubscribeQuickSlotInput();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            MovementInput = context.ReadValue<Vector2>();
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            MovementInput = Vector2.zero;
        }
    }

    public void OnOpenInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.Instance.CheckOpenPopup(UIInventory.Instance);
        }
    }

    public void OnOpenPauseMenu(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UIManager.Instance.CheckOpenPopup(UIPauseMenu.Instance);
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            IsInteract = true;
        }
    }

    public void ResetInteract()
    {
        IsInteract = false;
    }

    public void OnInputRecoding(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            IsRecoding = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            IsRecoding = false;
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        MouseDelta = context.ReadValue<Vector2>();
    }

    public void OnSprintInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            IsSprintPressed = true;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            IsSprintPressed = false;
        }
    }

    private void SubscribeQuickSlotInput()
    {
        for (int i = 0; i < 5; i++)
        {
            int slotIndex = i;
            slotActions[i] = ctx => OnQuickSlotPressed?.Invoke(slotIndex);

            var action = playerInput.asset.FindActionMap("Player").FindAction($"QuickSlot{slotIndex + 1}");
            if (action != null)
            {
                action.performed += slotActions[i];
            }
            else
            {
                Debug.LogWarning($"QuickSlot{slotIndex + 1} 액션을 찾을 수 없습니다.");
            }
        }
    }

    public void OnLeftClickInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnLeftClick?.Invoke();
        }
    }

    private void UnsubscribeQuickSlotInput()
    {
        for (int i = 0; i < 5; i++)
        {
            var action = playerInput.asset
                .FindActionMap("Player")
                .FindAction($"QuickSlot{i + 1}");

            if (action != null)
                action.performed -= slotActions[i];
        }
    }

    private void OnDestroy()
    {
        UnsubscribeQuickSlotInput();
        playerInput.Player.Disable();
    }
}