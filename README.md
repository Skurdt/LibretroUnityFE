# LibretroUnityFE

This is a Unity sandboxed project to help create a [Unity/C#](https://unity.com/) based [libretro](https://www.libretro.com/) core "launcher" (or "frontend", in libretro's terms).

The goal is to have this fully integrated within the [3DArcade](http://3darcade.messageboard.nl/index.php) frontend, in order to have playable games on 3D models inside virtual arcade environments.

### To get started:
Download some cores from http://buildbot.libretro.com/nightly/windows/x86_64/latest/ and place them under **Assets/StreamingAssets/Data~/Cores**.

Unlike cores, roms can be setup with absolute paths. So it doesn't really matter where they are, as long as the path is setup in the configuration file (**Assets/StreamingAssets/Data~/Games.json**).

The **Assets/StreamingAssets/Data~/Roms~** folder is just there for convenience, if you prefer absolute paths.

### To start a game:
- Select the arcade cab model in the scene
- In the inspector, either type-in or click on the button to select one of the available cores
- In the inspector, either type-in a directory and game (without the extension), or click on the buttons to select a rom (the directory will be filled automatically, but you can try any combination of buttons and manual edits, and maybe find UI bugs for me :D)
- Hit save to save the entry to disk, or start to both save and start the app (will activate the play button)

- *Alternatively, you can edit the **Assets/StreamingAssets/Data~/Games.json** config file*

### I used either code or directions from:
- [The libretro header itself](https://github.com/libretro/RetroArch/blob/master/libretro-common/include/libretro.h)
- [The official libretro frontend, RetroArch](https://github.com/libretro/RetroArch)

For C#:
- https://github.com/Scorr/RetroUnity
- https://github.com/lurrrrr/RetroNUI
- https://github.com/timlump/oculus_arcade
- https://github.com/cas1993per/bizhawk
