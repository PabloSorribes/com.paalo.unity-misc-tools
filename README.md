# Pablo's Unity Misc Tools
Add this package to get more functionality for Unity's Editor.

# Adding the package to your project
Add the package to your project by adding this in your `manifest.json`-file, located in the `Packages`-folder of your Unity Project:
```
{
  "dependencies": {
    "com.paalo.unity-misc-tools": "https://github.com/PabloSorribes/com.paalo.unity-misc-tools.git#0.2.5",
    ...
  }
}
```

# Stuff included:

## Bulk Rename Utility
Rename, find and replace, add prefix/suffix and enumerate the names of the GameObjects in your Hierarchy!

![Bulk Rename Utility 3 0](https://user-images.githubusercontent.com/10932943/74588978-8da7df80-5001-11ea-8e53-37f7256ec635.gif)

## Create scripts from own template
Use the default template or use a custom script template. Good tool to use if you've got a namespace which you want all your new scripts to end up in.

[GIF will come here]

## Set Audio Clips Utility
A tool which lets you select all audio clips in a folder and then apply them to the GameObjects with AudioSources on that you have selected in the hierearchy. Saves you the time of having to manually drag and drop an audio clip for each AudioSource. Very useful when working with [MasterAudio's SoundGroup Variations](https://www.dtdevtools.com/docs/masteraudio/SoundGroupVariations.htm).

[GIF will come here]
