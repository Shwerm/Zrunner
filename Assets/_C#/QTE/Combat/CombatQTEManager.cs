using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///<summary>
///Manages the combat QTE system functionaility
///Dependencies:
///</summary>
public class CombatQTEManager : MonoBehaviour
{
    #region Singleton
    public static CombatQTEManager Instance { get; private set; }
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    
}
