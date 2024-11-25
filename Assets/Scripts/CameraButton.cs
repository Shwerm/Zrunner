using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraButton : MonoBehaviour
{
    public GameObject Playercamera;
    public GameObject CinemaCameras;
    public GameObject MainCanvas;
    public GameObject ViewFinderCanvas;
    public bool CameraTaskActive = false;

    public GameObject TaskList;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCameraTask()
    {
        // Set Playercamera and ViewFinderCanvas active
        Playercamera.SetActive(true);
        ViewFinderCanvas.SetActive(true);

        // Set CinemaCameras and MainCanvas inactive
        CinemaCameras.SetActive(false);
        MainCanvas.SetActive(false);

        CameraTaskActive = true;
    }

    public void EndCameraTask()
    {
        // Set Playercamera and ViewFinderCanvas inactive
        Playercamera.SetActive(false);
        ViewFinderCanvas.SetActive(false);

        // Set CinemaCameras and MainCanvas active
        CinemaCameras.SetActive(true);
        MainCanvas.SetActive(true);

        CameraTaskActive = false;
    }

    public void CloseTaskList()
    {
        TaskList.SetActive(false);
    }

    public void OpenTaskList()
    {
        TaskList.SetActive(true);
    }
}
