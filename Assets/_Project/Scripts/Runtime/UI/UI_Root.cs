/* MIT License

 * Copyright (c) 2021-2022 Skurdt
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

using SK.Libretro.Unity;
using UnityEngine;

namespace SK.Libretro.Examples
{
    [DisallowMultipleComponent, DefaultExecutionOrder(1000)]
    public sealed class UI_Root : MonoBehaviour
    {
        [field: SerializeField] public LibretroInstanceVariable Libretro { get; private set; }
        [field: SerializeField] public UI_Toolbar Toolbar                { get; private set; }
        [field: SerializeField] public UI_Button GameButton              { get; private set; }
        [field: SerializeField] public UI_ToolbarMenu GameMenu           { get; private set; }
        [field: SerializeField] public UI_Button GameStartButton         { get; private set; }
        [field: SerializeField] public UI_Button GameResetButton         { get; private set; }
        [field: SerializeField] public UI_Button GameStopButton          { get; private set; }
        [field: SerializeField] public UI_Button StateButton             { get; private set; }
        [field: SerializeField] public UI_ToolbarMenu StateMenu          { get; private set; }
        [field: SerializeField] public UI_Button StateSaveButton         { get; private set; }
        [field: SerializeField] public UI_Button StateLoadButton         { get; private set; }

        private void OnEnable()
        {
            Libretro.OnInstanceChanged += LibretroInstanceChangedCallback;
            LibretroInstanceChangedCallback(Libretro.Current);

            Toolbar.SetVisible(true);

            GameButton.Construct(true, true, () => GameMenu.SetVisible(true));
            GameMenu.SetVisible(false);
            GameStartButton.Construct(true, true, () => Libretro.StartContent());
            GameResetButton.Construct(true, false, () => Libretro.ResetContent());
            GameStopButton.Construct(true, false, () => Libretro.StopContent());

            StateButton.Construct(true, false, () => StateMenu.SetVisible(true));
            StateMenu.SetVisible(false);
            StateSaveButton.Construct(true, true, null);
            StateLoadButton.Construct(true, true, null);
        }

        private void OnDisable()
        {
            Libretro.OnInstanceChanged -= LibretroInstanceChangedCallback;
            if (Libretro.Current != null)
            {
                Libretro.Current.OnInstanceStarted -= LibretronInstanceStartedCallback;
                Libretro.Current.OnInstanceStopped -= LibretronInstanceStoppedCallback;
            }
        }

        private void LibretronInstanceStartedCallback()
        {
            GameStartButton.SetText("Pause");
            GameStartButton.SetCallback(() =>
            {
                Libretro.PauseContent();
                GameStartButton.SetText("Resume");
                GameStartButton.SetCallback(() =>
                {
                    Libretro.ResumeContent();
                    GameStartButton.SetText("Pause");
                });
            });

            GameStopButton.SetInteractable(true);

            StateButton.SetInteractable(true);
        }

        private void LibretronInstanceStoppedCallback()
        {
            GameStartButton.SetText("Start");
            GameStartButton.SetCallback(() => Libretro.StartContent());

            GameStopButton.SetInteractable(false);

            StateButton.SetInteractable(false);
        }

        private void LibretroInstanceChangedCallback(LibretroInstance libretroInstance)
        {
            if (libretroInstance == null)
                return;

            libretroInstance.OnInstanceStarted -= LibretronInstanceStartedCallback;
            libretroInstance.OnInstanceStarted += LibretronInstanceStartedCallback;
            libretroInstance.OnInstanceStopped -= LibretronInstanceStoppedCallback;
            libretroInstance.OnInstanceStopped += LibretronInstanceStoppedCallback;
        }
    }
}
