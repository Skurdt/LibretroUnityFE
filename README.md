# LibretroUnityFE

[Libretro](https://www.libretro.com/) + [Unity](https://unity.com/) + [SK.Libretro](https://github.com/Skurdt/SK.Libretro).

This is not meant to be a features rich frontend by itself, but rather example scene(s) showing how to use the SK.Libretro package.

The official/main application that uses these implemented features can be found here: [3DArcade](https://github.com/Skurdt/3DArcade)

It mainly depends on the [SK.Libretro](https://github.com/Skurdt/SK.Libretro) package.
Other dependencies include:
- [SK.Utilities](https://github.com/Skurdt/SK.Utilities)
- [SK.Utilities.Unity](https://github.com/Skurdt/SK.Utilities.Unity)
- Unity Universal Render Pipeline
- Unity Burst
- Unity Input System
- Unity TextMesh Pro

Currently supports most non-gl cores, including mame, fbneo, mednafen, others...

Support for OpenGL based cores is WIP and uses a native plugin ([LibretroUnityPlugin](https://github.com/Skurdt/LibretroUnityPlugin)).
mupen64plus-next is currently playable, using the default core options, but still has some crash issues as well as graphical glitches in some games.

- Super Mario 64 running inside of a windows build: https://youtu.be/euec6832wNA
