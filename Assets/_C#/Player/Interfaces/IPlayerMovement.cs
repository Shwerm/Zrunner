using UnityEngine;

public interface IPlayerMovement
{
    void Move(float deltaTime);
    void Jump();
    void Dodge(float dodgeAmount);
    void ReverseDodge();
}
