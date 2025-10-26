using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

    namespace iv.vfx.toast.editor
{
    /// <summary>
    /// Editor window for generating font atlas textures for VFX Graph toast system
    /// </summary>
    public class FontAtlasGeneratorWindow : EditorWindow
    {
        private string symbols = "0123456789+- >.k";
        
        private bool antiAliasing = true;
        private TextAnchor textAnchor = TextAnchor.MiddleCenter;
        private string outputPath = "Assets/";
        private string outputFileName = "font-atlas";

        public FontAtlasGenerator fontGenerator;
        public FontAtlassPreviewer previewer;
        
        private Vector2 scrollPosition;

        [MenuItem("Tools/iv.vfx.toast/Font Atlas Generator")]
        public static void ShowWindow()
        {
            var window = GetWindow<FontAtlasGeneratorWindow>("Font Atlas Generator");
            window.minSize = new Vector2(400, 600);

            window.fontGenerator = new FontAtlasGenerator();
            window.previewer = new FontAtlassPreviewer
            {
                parentWindow = window
            };
            window.Show();
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Font Atlas Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Generate square atlas textures for VFX Graph toast system. " +
                "The atlas will be arranged in a grid where each cell contains one symbol.",
                MessageType.Info);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Font Settings", EditorStyles.boldLabel);
            fontGenerator.sourceFont = EditorGUILayout.ObjectField("Source Font", fontGenerator.sourceFont, typeof(Font), false) as Font;
            fontGenerator.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Font Style", fontGenerator.fontStyle);
            fontGenerator.characterSize = EditorGUILayout.IntSlider("Character Size", fontGenerator.characterSize, 16, 256);
            antiAliasing = EditorGUILayout.Toggle("Anti-Aliasing", antiAliasing);
            textAnchor = (TextAnchor)EditorGUILayout.EnumPopup("Text Anchor", textAnchor);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Atlas Settings", EditorStyles.boldLabel);
            symbols = EditorGUILayout.TextField("Symbols", symbols);
            
            var symbolCount = string.IsNullOrEmpty(symbols) ? 0 : symbols.Length;
            var minAtlasSize = Mathf.CeilToInt(Mathf.Sqrt(symbolCount));
            
            EditorGUILayout.LabelField($"Symbol Count: {symbolCount}");
            EditorGUILayout.LabelField($"Minimum Atlas Size: {minAtlasSize}x{minAtlasSize}");
            
            fontGenerator.atlasSize = EditorGUILayout.IntSlider("Atlas Grid Size", fontGenerator.atlasSize, minAtlasSize, 16);
            EditorGUILayout.LabelField($"Total Cells: {fontGenerator.atlasSize * fontGenerator.atlasSize}");
            EditorGUILayout.LabelField($"Texture Size: {fontGenerator.atlasSize * fontGenerator.characterSize}x{fontGenerator.atlasSize * fontGenerator.characterSize}");
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Color Settings", EditorStyles.boldLabel);
            fontGenerator.textColor = EditorGUILayout.ColorField("Text Color", fontGenerator.textColor);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Output Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            outputPath = EditorGUILayout.TextField("Output Path", outputPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                var path = EditorUtility.OpenFolderPanel("Select Output Folder", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                    if (path.StartsWith(Application.dataPath))
                        outputPath = Path.Combine("Assets", path.Substring(Application.dataPath.Length));
                    else
                        EditorUtility.DisplayDialog("Invalid Path", 
                                                    "Please select a folder inside the Assets directory.", "OK");
            }
            
            EditorGUILayout.EndHorizontal();
            
            outputFileName = EditorGUILayout.TextField("File Name", outputFileName);
            
            EditorGUILayout.Space(10);
            
            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = fontGenerator.sourceFont != null && !string.IsNullOrEmpty(symbols);
            if (GUILayout.Button("Generate Preview", GUILayout.Height(30)))
            {
                var previewTexture = fontGenerator.GenerateFontAtlas(symbols);
                previewer.SetPreview(previewTexture);
            }
            
            if (GUILayout.Button("Save Atlas Texture", GUILayout.Height(30)))
            {
                var data = fontGenerator.GetData(symbols);
                SaveAtlasTexture(data, outputFileName, outputPath);
            }
            
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            previewer.DrawPreview(symbols, fontGenerator.atlasSize);
            
            EditorGUILayout.EndScrollView();
        }

        private static void SaveAtlasTexture(byte[] data, string fileName, string dirName)
        {
            if (!Directory.Exists(dirName))
                Directory.CreateDirectory(dirName);

            var fullPath = Path.Combine(dirName, $"{fileName}.png");
            File.WriteAllBytes(fullPath, data);
            
            AssetDatabase.Refresh();
            
            var importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Multiple;
            importer.alphaSource = TextureImporterAlphaSource.None;
            importer.alphaIsTransparency = true;
            importer.sRGBTexture = false;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.SaveAndReimport();
            
            EditorUtility.DisplayDialog("Success", 
                                        $"Font atlas saved to {fullPath}",
                                        "OK");
        }

        private void OnDestroy() => previewer.Dispose();
    }
}
