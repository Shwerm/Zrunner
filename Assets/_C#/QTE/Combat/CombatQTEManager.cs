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

    [SerializeField]private GameObject leftEnemy;
    [SerializeField]private GameObject rightEnemy;

    [SerializeField]private float rushSpeed = 100f;

    private PlayerCameraManager playerCameraManager;

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

    void Start()
    {
        //Disable enemies at start of game
        leftEnemy.SetActive(false);
        rightEnemy.SetActive(false);

        playerCameraManager = PlayerCameraManager.Instance;
        if(playerCameraManager == null)
        {
            Debug.LogError("PlayerCameraManager is null");
        }
    }

    public void RushEnemyToPlayer(string activeCombatQte)
    {
        if(activeCombatQte == "Left")
        {
            leftEnemy.SetActive(true);

            playerCameraManager.LookLeft();
        }
        else if(activeCombatQte == "Right")
        {
            rightEnemy.SetActive(true);

            playerCameraManager.LookRight();
        }
    }
    

    void Update()
    {
        if(leftEnemy.activeSelf)
        {
            leftEnemy.transform.position = Vector3.MoveTowards(leftEnemy.transform.position, PlayerManager.Instance.transform.position, rushSpeed * Time.deltaTime * 9f);
        }
        
        if(rightEnemy.activeSelf)
        {
            rightEnemy.transform.position = Vector3.MoveTowards(rightEnemy.transform.position, PlayerManager.Instance.transform.position, rushSpeed * Time.deltaTime * 9f);
        }
    }
    
}
