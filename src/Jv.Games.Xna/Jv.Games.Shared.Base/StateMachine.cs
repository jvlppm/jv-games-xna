namespace Jv.Games.Xna.Base
{
    using Microsoft.Xna.Framework;
    using System;

    public delegate void RepeatingEventHandler (object sender, GameTime gameTime);
    public delegate void StateChangeEventHandler<T> (object sender, ref StateChangeEventArgs<T> args);
    public delegate void PreviewStateChangeEventHandler<T> (object sender, ref PreviewStateChangeEventArgs<T> args);

    public readonly struct StateChangeEventArgs<T>
    {
        public readonly T PreviousState;
        public readonly T NextState;

        public StateChangeEventArgs(T previous, T next)
        {
            PreviousState = previous;
            NextState = next;
        }
    }

    public struct PreviewStateChangeEventArgs<T>
    {
        public readonly T PreviousState;
        public readonly T NextState;
        public bool Cancel;

        public PreviewStateChangeEventArgs(T previous, T next)
        {
            PreviousState = previous;
            NextState = next;
            Cancel = false;
        }
    }

    public struct SimpleStateMachine<T>
    {
        public event PreviewStateChangeEventHandler<T> PreviewStateChange;
        public event StateChangeEventHandler<T> StateChange;
        public event RepeatingEventHandler StateUpdate;

        public T CurrentState { get; private set; }
        public T NextState { get; set; }

        public bool PendingStateChange => !Object.Equals(CurrentState, NextState);

        public void Update(GameTime  gameTime)
        {
            if (PendingStateChange) {
                var previewArgs = new PreviewStateChangeEventArgs<T>(CurrentState, NextState);
                PreviewStateChange?.Invoke(this, ref previewArgs);
                if (previewArgs.Cancel) {
                    NextState = CurrentState;
                    return;
                }

                CurrentState = NextState;

                var args = new StateChangeEventArgs<T>(previewArgs.PreviousState, previewArgs.NextState);
                StateChange?.Invoke(this, ref args);
            }
            StateUpdate?.Invoke(this, gameTime);
        }
    }

    public struct StateMachine
    {
        public class State
        {
            public string Name { get; }

            public event PreviewStateChangeEventHandler<State> PreviewEnter;
            public event PreviewStateChangeEventHandler<State> PreviewExit;
            public event StateChangeEventHandler<State> Enter;
            public event StateChangeEventHandler<State> Exit;
            public event RepeatingEventHandler Update;

            public State (string name = null) {
                Name = name;
            }

            internal void OnPreviewEnter(ref PreviewStateChangeEventArgs<State> args) => PreviewEnter?.Invoke(this, ref args);
            internal void OnPreviewExit(ref PreviewStateChangeEventArgs<State> args) => PreviewExit?.Invoke(this, ref args);
            internal void OnEnter(ref StateChangeEventArgs<State> args) => Enter?.Invoke(this, ref args);
            internal void OnExit(ref StateChangeEventArgs<State> args) => Exit?.Invoke(this, ref args);
            internal void OnUpdate(GameTime gameTime) => Update?.Invoke(this, gameTime);
        }

        public event PreviewStateChangeEventHandler<State> PreviewStateChange;
        public event StateChangeEventHandler<State> StateChange;
        public event RepeatingEventHandler StateUpdate;

        public State CurrentState { get; private set; }

        public bool PendingStateChange => CurrentState != NextState;

        public State Create(string name = null, StateChangeEventHandler<State> enter = null, StateChangeEventHandler<State> exit = null, PreviewStateChangeEventHandler<State> previewEnter = null, PreviewStateChangeEventHandler<State> previewExit = null, RepeatingEventHandler update = null)
        {
            var state = new State(name);

            if (previewEnter != null) {
                state.PreviewEnter += previewEnter;
            }
            if (previewExit != null) {
                state.PreviewExit += previewExit;
            }
            if (enter != null) {
                state.Enter += enter;
            }
            if (exit != null) {
                state.Exit += exit;
            }
            if (update != null) {
                state.Update += update;
            }

            if (NextState == null)
                NextState = state;

            return state;
        }

        public State NextState { get; set; }

        public void Update(GameTime gameTime)
        {
            if (PendingStateChange) {
                var previewArgs = new PreviewStateChangeEventArgs<State>(CurrentState, NextState);
                PreviewStateChange?.Invoke(this, ref previewArgs);
                if (previewArgs.Cancel) {
                    NextState = CurrentState;
                    return;
                }
                previewArgs.PreviousState?.OnPreviewExit(ref previewArgs);
                if (previewArgs.Cancel) {
                    NextState = CurrentState;
                    return;
                }
                previewArgs.NextState?.OnPreviewEnter(ref previewArgs);
                if (previewArgs.Cancel) {
                    NextState = CurrentState;
                    return;
                }

                CurrentState = NextState;

                var args = new StateChangeEventArgs<State>(previewArgs.PreviousState, previewArgs.NextState);
                args.PreviousState?.OnExit(ref args);
                args.NextState?.OnEnter(ref args);

                StateChange?.Invoke(this, ref args);
            }
            CurrentState?.OnUpdate(gameTime);
            StateUpdate?.Invoke(this, gameTime);
        }
    }
}