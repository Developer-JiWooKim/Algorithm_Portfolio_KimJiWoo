using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _inputVector = Vector2.zero;

    public Vector2 InputVector => _inputVector;

    /// <summary>
    /// InputSystem
    /// </summary>
    public void InputKeyboardValue()
    {
        float h = 0;
        float v = 0;

        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  h = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    v = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  v = -1;

        _inputVector = new Vector2(h, v);
    }
}
