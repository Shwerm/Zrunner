using UnityEngine;
using System;

/// <summary>
/// Toggles the enemy's non active state when hit by a bullet.
/// </summary>
public class KillableEnemyToggle : MonoBehaviour
{
    public static event Action onBulletHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            gameObject.SetActive(false);
            onBulletHit?.Invoke();
            Destroy(other.gameObject);
        }
    }
}