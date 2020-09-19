/* MIT License

 * Copyright (c) 2020 Skurdt
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:

 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.

 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE. */

using System.Collections.Generic;

namespace SK.Examples.Player
{
    public class StateController
    {
        public State CurrentState { get; private set; }

        private readonly Controls _controls;
        private readonly Interactions _interactions;

        private readonly List<State> _allStates = new List<State>();

        public StateController(Controls controls, Interactions interactions)
        {
            _controls     = controls;
            _interactions = interactions;
        }

        public void Update(float dt) => CurrentState?.OnUpdate(dt);

        public void FixedUpdate(float dt) => CurrentState?.OnFixedUpdate(dt);

        public void TransitionTo<T>() where T : State
        {
            State newState = _allStates.Find(x => x.GetType() == typeof(T));
            if (newState != null)
            {
                if (CurrentState != newState)
                {
                    if (CurrentState != null)
                    {
                        CurrentState.OnExit();
                    }
                    CurrentState = newState;
                    CurrentState.OnEnter();
                }
            }
            else
            {
                _allStates.Add(System.Activator.CreateInstance(typeof(T), this, _controls, _interactions) as State);
                TransitionTo<T>();
            }
        }
    }
}
