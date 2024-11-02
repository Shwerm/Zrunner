using UnityEngine;
using System;

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