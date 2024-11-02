using UnityEngine;

public interface IPlayerCollision
{
    void HandleCollision(Collision collision);
    void HandleTrigger(Collider other);
}
