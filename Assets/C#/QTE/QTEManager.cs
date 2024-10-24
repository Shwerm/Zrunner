using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages Quick Time Events (QTE) system including state handling and event dispatching.
/// </summary>
public class QTEManager : MonoBehaviour
{
    public static QTEManager Instance { get; private set; }

    public event Action<string> OnQTEStarted;
    public event Action<string> OnQTESucceeded;
    public event Action<string> OnQTEFailed;

    [SerializeField] private float m_SlowMotionTimeScale = 0.2f;
    
    private GameSceneUIManager m_GameSceneUIManager;
    private PlayerManager m_PlayerManager;
    private QTEState m_CurrentState;
    private readonly Dictionary<string, IQTEAction> m_QTEActions;

    public QTEManager()
    {
        m_QTEActions = new Dictionary<string, IQTEAction>
        {
            { "Jump", new JumpQTEAction() },
            { "Slide", new SlideQTEAction() },
            { "DodgeRight", new DodgeRightQTEAction() },
            { "DodgeLeft", new DodgeLeftQTEAction() }
        };
    }

    private void Awake()
    {
        InitializeSingleton();
        InitializeComponents();
        SetState(new IdleQTEState(this));
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeComponents()
    {
        m_GameSceneUIManager = FindObjectOfType<GameSceneUIManager>();
        m_PlayerManager = PlayerManager.playerManagerInstance;

        if (m_GameSceneUIManager == null || m_PlayerManager == null)
        {
            Debug.LogError("[QTEManager] Required components not found!");
            enabled = false;
        }
    }

    public void QTEStart()
    {
        m_CurrentState.OnQTEStart();
    }

    public void QTESuccess(string activeQTE)
    {
        if (m_QTEActions.TryGetValue(activeQTE, out IQTEAction action))
        {
            action.Execute(m_PlayerManager);
            OnQTESucceeded?.Invoke(activeQTE);
            ResetTimeScale();
        }
        else
        {
            Debug.LogError($"[QTEManager] Invalid QTE type: {activeQTE}");
        }
    }

    private void SetState(QTEState newState)
    {
        m_CurrentState = newState;
    }

    private void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

    private void OnDestroy()
    {
        OnQTEStarted = null;
        OnQTESucceeded = null;
        OnQTEFailed = null;
    }
}

#region QTE States
public abstract class QTEState
{
    protected readonly QTEManager Manager;

    protected QTEState(QTEManager manager)
    {
        Manager = manager;
    }

    public abstract void OnQTEStart();
}

public class IdleQTEState : QTEState
{
    public IdleQTEState(QTEManager manager) : base(manager) { }

    public override void OnQTEStart()
    {
        Time.timeScale = 0.2f;
        Manager.OnQTEStarted?.Invoke("QTE_START");
    }
}
#endregion

#region QTE Actions
public interface IQTEAction
{
    void Execute(PlayerManager player);
}

public class JumpQTEAction : IQTEAction
{
    public void Execute(PlayerManager player) => player.Jump();
}

public class SlideQTEAction : IQTEAction
{
    public void Execute(PlayerManager player) { /* Implement slide logic */ }
}

public class DodgeRightQTEAction : IQTEAction
{
    public void Execute(PlayerManager player)
    {
        player.Dodge(4);
        player.LookLeft();
    }
}

public class DodgeLeftQTEAction : IQTEAction
{
    public void Execute(PlayerManager player)
    {
        player.Dodge(-4);
        player.LookRight();
    }
}
#endregion