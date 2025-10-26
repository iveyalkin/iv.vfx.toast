using System;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace iv.vfx.toast.editor
{
    public class FontAtlassPreviewer : IDisposable
    {
        public Texture2D previewTexture;
        public bool HasPreview => previewTexture != null;
        
        public EditorWindow parentWindow;

        public void SetPreview(Texture2D previewTexture)
        {
            try
            {
                // Clean up old preview
                if (this.previewTexture != null)
                {
                    Object.DestroyImmediate(this.previewTexture);
                    this.previewTexture = null;
                }
                
                this.previewTexture = previewTexture;
                
                if (previewTexture == null)
                {
                    EditorUtility.DisplayDialog("Error", "Failed to generate atlas texture.", "OK");
                    return;
                }
                
                parentWindow.Repaint();
            }
            catch (Exception e)
            {
                EditorUtility.DisplayDialog("Error", 
                    $"Failed to generate preview: {e.Message}\n\nSee console for details.", 
                    "OK");
                Debug.LogError($"Font atlas generation failed: {e}\nStack trace:\n{e.StackTrace}");
            }
        }

        public void DrawPreview(string symbols, int atlasSize)
        {
            if (!HasPreview) return;
            
            EditorGUILayout.Space(10);
                
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
                
            var previewSize = Mathf.Min(parentWindow.position.width - 40, 400);
            var previewRect = GUILayoutUtility.GetRect(previewSize, previewSize);
            EditorGUI.DrawPreviewTexture(previewRect, previewTexture, null, ScaleMode.ScaleToFit);
                
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Symbol Grid Layout:", EditorStyles.boldLabel);
                
            DisplaySymbolGrid(symbols, atlasSize);
        }
        
        private void DisplaySymbolGrid(string symbols, int atlasSize)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            
            for (var row = atlasSize - 1; row >= 0; row--)
            {
                EditorGUILayout.BeginHorizontal();
                
                for (var col = 0; col < atlasSize; col++)
                {
                    var index = row * atlasSize + col;
                    var displayChar = index < symbols.Length ? symbols[index].ToString() : "[ ]";
                    
                    if (displayChar == " ")
                        displayChar = "[SP]";
                    
                    GUILayout.Label($"({col},{row}): {displayChar}", GUILayout.Width(80));
                }
                
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.EndVertical();
        }

        public void Dispose()
        {
            if (previewTexture != null)
                Object.DestroyImmediate(previewTexture);
        }
    }
}