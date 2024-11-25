using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float elapsedTime = 0f;
    private float twoMinuteInterval = 120f; // 2 minutes in seconds
    private float tenMinuteInterval = 600f; // 10 minutes in seconds

    private void Update()
    {
        // Increment elapsed time
        elapsedTime += Time.deltaTime;

        // Check if 2 minutes have passed
        if (elapsedTime >= twoMinuteInterval)
        {
            Debug.Log("Two minutes");
            elapsedTime = 0f; // Reset the timer
        }

        // Check if 10 minutes have passed
        if (elapsedTime >= tenMinuteInterval)
        {
            Debug.Log("10 MINUTES");
            // You might want to perform additional actions here when 10 minutes is up
            enabled = false; // Disable the script to stop further updates
        }
    }
}
