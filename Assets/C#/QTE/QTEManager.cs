using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QTEManager : MonoBehaviour
{
    // Define Singleton instance
    public static QTEManager QTEManagerInstance { get; private set; }


    void Start()
    {
        // Create Singleton Instance of the QTEManager
        if (QTEManagerInstance == null)
        {
            QTEManagerInstance = this;
        }
        else
        {
            Destroy(this);
        }
    }


    public void JumpQTE()
    {
        Debug.Log("Jump QTE");
    }

    public void SlideQTE()
    {
        Debug.Log("Slide QTE");
    }

    public void DodgeRightQTE()
    {
        Debug.Log("Dodge Right QTE");
    }

    public void DodgeLeftQTE()
    {
        Debug.Log("Dodge Left QTE");
    }
}
