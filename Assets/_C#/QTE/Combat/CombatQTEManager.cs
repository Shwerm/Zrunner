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

    [SerializeField]private GameObject rightKillableEnemy;
    [SerializeField]private GameObject leftKillableEnemy;
    [SerializeField]private GameObject bullet;
    [SerializeField]private GameObject bulletSpawnPoint;

    [SerializeField]private float rushSpeed = 100f;

    private PlayerCameraManager playerCameraManager;

    private IInputService inputService;

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

    public void Initialize(IInputService input)
    {
        inputService = input;
    }

    void Start()
    {
        //Disable enemies at start of game
        leftEnemy.SetActive(false);
        rightEnemy.SetActive(false);

        //Disable killable enemies at start of game
        leftKillableEnemy.SetActive(false);
        rightKillableEnemy.SetActive(false);

        playerCameraManager = PlayerCameraManager.Instance;
        if(playerCameraManager == null)
        {
            Debug.LogError("PlayerCameraManager is null");
        }

        PlayerManager.Instance.OnPlayerStateChanged += HandlePlayerStateChange;
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

    public IEnumerator ShootEnemy(string activeCombatQte)
    {
        if(activeCombatQte == "Left")
        {
            leftKillableEnemy.SetActive(true);
            playerCameraManager.LookLeft();
            //Instantiate bullet
            yield return new WaitForSeconds(0.5f);
            SpawnBulletLeft();
        }
        else if(activeCombatQte == "Right")
        {
            rightKillableEnemy.SetActive(true);
            playerCameraManager.LookRight();
            //Instantiate bullet
            yield return new WaitForSeconds(0.5f);
            SpawnBulletRight();
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

    private void SpawnBulletLeft()
    {
        Quaternion rotation = Quaternion.Euler(90f, 0f, 90f);
        Instantiate(bullet, bulletSpawnPoint.transform.position, rotation);
    }

    private void SpawnBulletRight()
    {
        Quaternion rotation = Quaternion.Euler(90f, 0f, -90f);
        Instantiate(bullet, bulletSpawnPoint.transform.position, rotation);
    }

    void OnEnable()
    {
        KillableEnemyToggle.onBulletHit += HandleEnemyHit;
    }

    void OnDisable()
    {
        KillableEnemyToggle.onBulletHit -= HandleEnemyHit;
    }

    private void HandleEnemyHit()
    {
        if (leftKillableEnemy.GetComponent<MeshRenderer>().enabled == false)
        {
            leftKillableEnemy.SetActive(false);
        }
        
        if (rightKillableEnemy.GetComponent<MeshRenderer>().enabled == false)
        {
            rightKillableEnemy.SetActive(false);
        }
    }
}
