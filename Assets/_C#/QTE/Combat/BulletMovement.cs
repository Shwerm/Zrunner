using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField]private float bulletSpeed = 20f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.up * bulletSpeed, ForceMode.Impulse);
    }
}
