#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace CrazyBirdLady.FolderIcons
{
    // Shared core for folder icon handling. Packages populate `folderIcons` and call Register().
    public static class FolderIconCore
    {
        internal static Dictionary<string, Texture2D> folderIcons = new Dictionary<string, Texture2D>();
        private static readonly string iconKeyPrefix = "FolderColor_";
        private static bool registered = false;

        public static void Register()
        {
            if (registered)
                return;
            registered = true;
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        }

        public static void Unregister()
        {
            if (!registered)
                return;
            registered = false;
            EditorApplication.projectWindowItemOnGUI -= OnProjectWindowItemGUI;
        }

        private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(path))
                return;

            string iconName = EditorPrefs.GetString(iconKeyPrefix + path, "");
            if (string.IsNullOrEmpty(iconName))
                return;

            if (!folderIcons.TryGetValue(iconName, out Texture2D baseIcon) || baseIcon == null)
                return;

            Texture2D iconToUse = baseIcon;
            bool isListView = selectionRect.height < 25f;

            if (isListView)
            {
                if (folderIcons.TryGetValue(iconName + "_list", out Texture2D listIcon) && listIcon != null)
                    iconToUse = listIcon;
            }

            Rect iconRect = isListView
                ? new Rect(selectionRect.x, selectionRect.y + 1f, 16f, 16f)
                : new Rect(
                    selectionRect.x + (selectionRect.width - Mathf.Min(selectionRect.width, selectionRect.height - 12f)) / 2f,
                    selectionRect.y,
                    Mathf.Min(selectionRect.width, selectionRect.height - 12f),
                    Mathf.Min(selectionRect.width, selectionRect.height - 12f)
                  );

            GUI.DrawTexture(iconRect, iconToUse, ScaleMode.ScaleToFit);
        }

        public static void ClearIcons()
        {
            folderIcons.Clear();
        }

        public static Texture2D LoadEditorResource(string resourcePath)
        {
            return EditorGUIUtility.Load(resourcePath) as Texture2D;
        }

        public static void SetColor(string color)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(path))
            {
                EditorPrefs.SetString(iconKeyPrefix + path, color);
                RepaintProjectWindow();
            }
        }

        public static void ResetFolderColor()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (AssetDatabase.IsValidFolder(path))
            {
                EditorPrefs.DeleteKey(iconKeyPrefix + path);
                RepaintProjectWindow();
            }
        }

        public static bool ValidateSetColor()
        {
            return Selection.activeObject != null &&
                   AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(Selection.activeObject));
        }

        public static bool ValidateResetFolderColor()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            return AssetDatabase.IsValidFolder(path) && EditorPrefs.HasKey(iconKeyPrefix + path);
        }

        [MenuItem("Assets/Reset Folder To Default Icon")]
        public static void ResetFolderToDefaultMenu() => ResetFolderColor();

        [MenuItem("Assets/Reset Folder To Default Icon", true)]
        public static bool ValidateResetFolderToDefaultMenu() => ValidateResetFolderColor();

        public static void RepaintProjectWindow()
        {
            EditorApplication.RepaintProjectWindow();
        }
    }
}
#endif
