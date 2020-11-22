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

namespace SK.Examples
{
    internal sealed class GameModelSetupInteractive : GameModelSetup
    {
        public bool Running => Libretro != null && Libretro.Running;

        public void Rewind(bool rewind) => Libretro.Rewind(rewind);

        public bool InputEnabled
        {
            get => Libretro != null && Libretro.InputEnabled;
            set
            {
                if (Libretro != null)
                    Libretro.InputEnabled = value;
            }
        }

        protected override int IndexInConfig => transform.GetSiblingIndex();

        protected override ConfigFileContentList GetConfigContent()
        {
            GameModelSetup[] gameModelSetups = transform.parent.GetComponentsInChildren<GameModelSetup>();

            ConfigFileContentList gameList = new ConfigFileContentList
            {
                Entries = new ConfigFileContent[gameModelSetups.Length]
            };

            for (int i = 0; i < gameModelSetups.Length; ++i)
            {
                GameModelSetup gameModelSetup = gameModelSetups[i];

                gameList.Entries[i] = new ConfigFileContent
                {
                    Core                      = gameModelSetup.CoreName,
                    Directory                 = gameModelSetup.GameDirectory,
                    Name                      = gameModelSetup.GameName,
                    AnalogDirectionsToDigital = gameModelSetup.AnalogDirectionsToDigitalToggle != null ? gameModelSetup.AnalogDirectionsToDigitalToggle.isOn : gameModelSetup.AnalogDirectionsToDigital
                };
            }

            return gameList;
        }
    }
}
