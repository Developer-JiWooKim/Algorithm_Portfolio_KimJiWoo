using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Vector2 _inputVector = Vector2.zero;

    public Vector2 InputVector => _inputVector;

    // InputSystem
    private void InputKeyboardValue(ref Vector2 inputVector)
    {
        inputVector = Vector2.zero;

        float h = 0;
        float v = 0;

        // isPressed는 해당 키를 누르고 있는 동안 true가 되는 프로퍼티입니다.
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)  h = -1;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)    v = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)  v = -1;

        inputVector = new Vector2(h, v);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current is not null) InputKeyboardValue(ref _inputVector);
    }
}
