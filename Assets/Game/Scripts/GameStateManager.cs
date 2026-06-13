using System;
using System.Collections;
using OneTimeGames.CoreSystems;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    public event Action OnEnterStart;
    public event Action OnEnterPlaying;
    public event Action OnEnterGameOver;

    private GameStateMachine _stateMachine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _stateMachine = new GameStateMachine();
        _stateMachine.RegisterState("Start", () => OnEnterStart?.Invoke(), null, null);
        _stateMachine.RegisterState("Playing", () => OnEnterPlaying?.Invoke(), null, null);
        _stateMachine.RegisterState("GameOver", () => OnEnterGameOver?.Invoke(), null, null);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private IEnumerator Start()
    {
        yield return null;
        GoToStart();
    }

    private void Update()
    {
        _stateMachine.Tick();
    }

    public string CurrentState => _stateMachine.CurrentState;

    public bool IsInState(string stateId) => _stateMachine.IsInState(stateId);

    public void GoToStart() => _stateMachine.TransitionTo("Start");

    public void StartGame() => _stateMachine.TransitionTo("Playing");

    public void GoToGameOver() => _stateMachine.TransitionTo("GameOver");
}
