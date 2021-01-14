# LibretroUnityFE

A [libretro](https://www.libretro.com/) frontend written in C# using the [Unity game engine](https://unity.com/)

This repository only contains example scenes as well as the code for the c++ native plugin powering the opengl implementation.
The main code for the wrapper is available here: https://github.com/Skurdt/SK.Libretro
Which depends on these libraries:
- https://github.com/Skurdt/SK.Utilities
- https://github.com/Skurdt/SK.Utilities.Unity

The goal is to have this fully integrated within the [3DArcade](http://3darcade.messageboard.nl/viewforum.php?f=15) frontend ([GitHub](https://github.com/3DArcade/3DArcade/tree/develop)), in order to have playable games on 3D models inside virtual environments.

Currently supports most non-gl cores, including mame, fbneo, mednafen, others...

Support for OpenGL based cores is WIP. mupen64plus-next is currently playable, using the default core options, but still has some crash issues as well as graphical glitches in some games.

- Super Mario 64 running inside of a windows build: https://youtu.be/euec6832wNA
