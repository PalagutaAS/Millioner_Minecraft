using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Localization;

public class GameStateMachine
{
    private static readonly Dictionary<GameState, HashSet<GameState>> ValidTransitions = new()
    {
        { GameState.Idle, new HashSet<GameState> { GameState.Intro } },
        { GameState.Intro, new HashSet<GameState> { GameState.QuestionActive } },
        { GameState.QuestionActive, new HashSet<GameState> { GameState.AnswerLocked, GameState.GameWon } },
        { GameState.AnswerLocked, new HashSet<GameState> { GameState.QuestionActive, GameState.GameOver, GameState.GameWon } },
        { GameState.GameOver, new HashSet<GameState> { GameState.Idle } },
        { GameState.GameWon, new HashSet<GameState> { GameState.Idle } }
    };

    private readonly Dictionary<GameState, IGameState> _states = new();

    public GameState CurrentState { get; private set; } = GameState.Idle;

    public event Action<GameState, GameState> OnStateChanged;

    public GameStateMachine(GameStateFactory factory, LocalizationManager localizationManager)
    {
        CurrentState = GameState.Idle;

        var states = factory.CreateStates(this, localizationManager);
        foreach (var state in states)
            RegisterState(state);
    }

    private void RegisterState(IGameState state)
    {
        _states[state.StateId] = state;
    }

    private bool CanTransitionTo(GameState newState)
    {
        return ValidTransitions.TryGetValue(CurrentState, out var allowed) && allowed.Contains(newState);
    }

    public async UniTask TransitionTo(GameState newState)
    {
        if (!CanTransitionTo(newState))
            return;

        var previousState = CurrentState;

        if (_states.TryGetValue(CurrentState, out var oldState))
            await oldState.Exit();

        CurrentState = newState;
        OnStateChanged?.Invoke(previousState, newState);

        if (_states.TryGetValue(newState, out var state))
            await state.Enter();
    }
}
