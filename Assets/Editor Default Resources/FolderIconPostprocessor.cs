#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CrazyBirdLady.FolderIcons
{
    // Package initializer that populates the shared core and registers the draw callback.
    [InitializeOnLoad]
    public static class FreeFolderIconsManager
    {
        static FreeFolderIconsManager()
        {
            LoadIcons();
            FolderIconCore.Register();
        }

        public static void LoadIcons()
        {

            Texture2D LoadIcon(string name) => FolderIconCore.LoadEditorResource($"Handcrafted Pixel Folders/{name}.png");

            FolderIconCore.folderIcons["CharcoalGray"] = LoadIcon("charcoalGray");
            FolderIconCore.folderIcons["LightGray"] = LoadIcon("lightGray");
            FolderIconCore.folderIcons["Snow"] = LoadIcon("snow");
            FolderIconCore.folderIcons["Orange"] = LoadIcon("orange");
            FolderIconCore.folderIcons["Red"] = LoadIcon("red");
            FolderIconCore.folderIcons["Brown"] = LoadIcon("brown");
            FolderIconCore.folderIcons["Yellow"] = LoadIcon("yellow");
            FolderIconCore.folderIcons["Lemon"] = LoadIcon("lemon");
            FolderIconCore.folderIcons["Olive"] = LoadIcon("olive");
            FolderIconCore.folderIcons["GrassGreen"] = LoadIcon("grassGreen");
            FolderIconCore.folderIcons["Lime"] = LoadIcon("lime");
            FolderIconCore.folderIcons["Teal"] = LoadIcon("teal");
            FolderIconCore.folderIcons["Turquoise"] = LoadIcon("turquoise");
            FolderIconCore.folderIcons["NavyBlue"] = LoadIcon("navyBlue");
            FolderIconCore.folderIcons["SkyBlue"] = LoadIcon("skyBlue");
            FolderIconCore.folderIcons["Purple"] = LoadIcon("purple");
            FolderIconCore.folderIcons["Lavender"] = LoadIcon("lavender");
            FolderIconCore.folderIcons["Rose"] = LoadIcon("rose");
            FolderIconCore.folderIcons["Coral"] = LoadIcon("coral");
            FolderIconCore.folderIcons["Blush"] = LoadIcon("blush");
            FolderIconCore.folderIcons["Bubblegum"] = LoadIcon("bubblegum");
            FolderIconCore.folderIcons["Apricot"] = LoadIcon("apricot");
            FolderIconCore.folderIcons["Sand"] = LoadIcon("sand");

            // Preload optional list icons (if present)
            foreach (var key in new List<string>(FolderIconCore.folderIcons.Keys))
            {
                var listTex = LoadIcon(key.ToLowerInvariant() + "_list");
                if (listTex != null)
                    FolderIconCore.folderIcons[key + "_list"] = listTex;
            }
        }

        // Validation wrapper
        [MenuItem("Assets/Set Custom Folder/Vibrant/CharcoalGray", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/LightGray", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Snow", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Orange", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Red", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Brown", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Yellow", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lemon", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Olive", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/GrassGreen", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lime", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Teal", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Turquoise", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/NavyBlue", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/SkyBlue", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Purple", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lavender", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Rose", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Coral", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Blush", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Bubblegum", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Apricot", true)]
        [MenuItem("Assets/Set Custom Folder/Vibrant/Sand", true)]
        public static bool ValidateSetColor() => FolderIconCore.ValidateSetColor();

        // Menu item setters
        [MenuItem("Assets/Set Custom Folder/Vibrant/CharcoalGray")]
        public static void SetCharcoalGray() => FolderIconCore.SetColor("CharcoalGray");
        [MenuItem("Assets/Set Custom Folder/Vibrant/LightGray")]
        public static void SetLightGray() => FolderIconCore.SetColor("LightGray");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Snow")]
        public static void SetSnow() => FolderIconCore.SetColor("Snow");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Orange")]
        public static void SetOrange() => FolderIconCore.SetColor("Orange");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Red")]
        public static void SetRed() => FolderIconCore.SetColor("Red");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Brown")]
        public static void SetBrown() => FolderIconCore.SetColor("Brown");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Yellow")]
        public static void SetYellow() => FolderIconCore.SetColor("Yellow");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lemon")]
        public static void SetLemon() => FolderIconCore.SetColor("Lemon");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Olive")]
        public static void SetOlive() => FolderIconCore.SetColor("Olive");
        [MenuItem("Assets/Set Custom Folder/Vibrant/GrassGreen")]
        public static void SetGrassGreen() => FolderIconCore.SetColor("GrassGreen");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lime")]
        public static void SetLime() => FolderIconCore.SetColor("Lime");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Teal")]
        public static void SetTeal() => FolderIconCore.SetColor("Teal");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Turquoise")]
        public static void SetTurquoise() => FolderIconCore.SetColor("Turquoise");
        [MenuItem("Assets/Set Custom Folder/Vibrant/NavyBlue")]
        public static void SetNavyBlue() => FolderIconCore.SetColor("NavyBlue");
        [MenuItem("Assets/Set Custom Folder/Vibrant/SkyBlue")]
        public static void SetSkyBlue() => FolderIconCore.SetColor("SkyBlue");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Purple")]
        public static void SetPurple() => FolderIconCore.SetColor("Purple");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Lavender")]
        public static void SetLavender() => FolderIconCore.SetColor("Lavender");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Rose")]
        public static void SetRose() => FolderIconCore.SetColor("Rose");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Coral")]
        public static void SetCoral() => FolderIconCore.SetColor("Coral");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Blush")]
        public static void SetBlush() => FolderIconCore.SetColor("Blush");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Bubblegum")]
        public static void SetBubblegum() => FolderIconCore.SetColor("Bubblegum");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Apricot")]
        public static void SetApricot() => FolderIconCore.SetColor("Apricot");
        [MenuItem("Assets/Set Custom Folder/Vibrant/Sand")]
        public static void SetSand() => FolderIconCore.SetColor("Sand");
    }

    // AssetPostprocessor class to refresh icons when icon assets are imported
    public class FolderIconAssetPostprocessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string asset in importedAssets)
            {
                if (asset.Contains("Handcrafted Pixel Folders"))
                {
                    FreeFolderIconsManager.LoadIcons();
                    FolderIconCore.RepaintProjectWindow();
                    break;
                }
            }
        }
    }
}
#endif




                    
