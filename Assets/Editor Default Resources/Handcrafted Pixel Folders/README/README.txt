Handcrafted Pixel Folders for Unity

Make your Unity project easier to navigate—and a lot more cozy— with custom folder icons! 
This simple Editor extension lets you assign pixel art folder icons in just a couple of clicks. 


Features

23 handcrafted folder icons in a variety of colors 
Works in both grid view and list view in the Unity project window
Lightweight and editor-only system that does not affect builds
Automatically drawn over Unity’s default folders
Easy to extend with your own custom artwork


How to Install

Download from the Unity Asset Store and follow the prompts to import into your project
You’ll now see new context menu options when right-clicking folders in the project window
Right-click > Set Custom Folder > Vibrant 
Right-click > Reset Folder To Default 


Customization

Add your own PNG icons to the "Editor Default Resources/Handcrafted Pixel Folders" folder. (The custom folders included are 24x24.) 
Update the script dictionary and menu to recognize your new folder icons. 


Script Reference

This tool uses a shared core + package script: 

Assets/Editor Default Resources/FolderIconCore.cs- shared core that registers the Project 
window GUI callback (`EditorApplication.projectWindowItemOnGUI`) and draws icons from a 
central `folderIcons` dictionary. 

Assets/Editor Default Resources/FolderIconAssetPostprocessor.cs-  package-specific script that 
registers the folder icon textures with the core and uses `EditorPrefs` to store per-folder icon 
assignments; the icons are rendered over Unity's default folder icons in the Project window. 

License

This asset pack is licensed for use in both personal and commercial projects. By downloading 
this package, you are granted permission to use the included pixel art folders in your games, 
prototypes, tools, or other creative works. You can modify the assets to suit your needs. You 
may not redistribute or resell as assets. Attribution is not required, but always appreciated.

Created by CrazyBirdLady
https://crazybirdlady.net/

Tool concept and development with help from ChatGPT

Color palette- Resurrect 64 by Kerri Lake
https://lospec.com/palette-list/resurrect-64