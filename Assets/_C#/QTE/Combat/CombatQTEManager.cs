using System.Collections;
using UnityEngine;

/// <summary>
/// Manages the combat QTE system functionaility.
/// 
/// Dependencies: PlayerCameraManager.cs
/// </summary>
public class CombatQTEManager : MonoBehaviour
{
    #region Singleton
    public static CombatQTEManager Instance { get; private set; }
    #endregion

    #region Private Fields
    [SerializeField]private GameObject leftEnemy;
    [SerializeField]private GameObject rightEnemy;

    [SerializeField]private GameObject rightKillableEnemy;
    [SerializeField]private GameObject leftKillableEnemy;
    [SerializeField]private GameObject bullet;
    [SerializeField]private GameObject bulletSpawnPoint;

    [SerializeField]private float rushSpeed = 100f;

    private PlayerCameraManager playerCameraManager;
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
    }

    /// <summary>
    /// Initiates enemy rush attack sequence based on specified direction.
    /// Controls enemy visibility and camera orientation.
    /// </summary>
    /// <param name="activeCombatQte">Direction of the attack ("Left" or "Right")</param>
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

    /// <summary>
    /// Executes the shooting sequence with timing delays and proper enemy positioning.
    /// Manages bullet spawning and camera orientation for the attack.
    /// </summary>
    /// <param name="activeCombatQte">Direction of the shot ("Left" or "Right")</param>
    /// <returns>IEnumerator for coroutine execution</returns>
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
