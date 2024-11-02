using UnityEngine;

public interface IInputService
{
    bool IsQTEKeyPressed(KeyCode key);
    float GetHorizontalInput();
}
