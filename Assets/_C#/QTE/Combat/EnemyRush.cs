using System.Collections;
using UnityEngine;

/// <summary>
/// Handles moving the non killable enemy objects towards the player.
/// </summary>
public class EnemyRush : MonoBehaviour
{
    [SerializeField]private float enemySpeed = 10f;

    void OnEnable()
    {
        //Rush the enemy forward
        StartCoroutine(RushForward());
    }

    IEnumerator RushForward()
    {
        while (true)
        {
            transform.Translate(Vector3.forward * enemySpeed * Time.deltaTime);
            yield return null;
        }
    }
}
