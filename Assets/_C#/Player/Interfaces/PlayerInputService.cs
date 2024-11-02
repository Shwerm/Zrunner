using UnityEngine;

public class PlayerInputService : MonoBehaviour, IInputService
{
    public bool IsQTEKeyPressed(KeyCode key)
    {
        return Input.GetKeyDown(key);
    }

    public float GetHorizontalInput()
    {
        return Input.GetAxis("Horizontal");
    }
}
