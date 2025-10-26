using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace iv.vfx.toast.editor
{
    public class FontAtlasGenerator
    {
        public int atlasSize = 4;
        public int characterSize = 16;
        public Font sourceFont;
        public FontStyle fontStyle;
        public Color textColor = Color.white;

        // Use GUI/Text Shader for proper font rendering with alpha
        public Shader textShader = Shader.Find("GUI/Text Shader");

        public byte[] GetData(string symbols) => GenerateFontAtlas(symbols).EncodeToPNG();

        public Texture2D GenerateFontAtlas(string symbols)
        {
            int textureSize = atlasSize * characterSize;
            Texture2D atlas = new Texture2D(textureSize, textureSize, TextureFormat.RGBA32, false);
            
            Color[] pixels = new Color[textureSize * textureSize];
            for (var i = 0; i < pixels.Length; ++i)
            {
                pixels[i] = Color.clear;
            }
            atlas.SetPixels(pixels);
            atlas.Apply();
            
            // Request characters from font (important for dynamic fonts)
            sourceFont.RequestCharactersInTexture(symbols, characterSize, fontStyle);
            
            // Render each character using GUI rendering
            for (int i = 0; i < symbols.Length && i < atlasSize * atlasSize; i++)
            {
                var charTexture = RenderCharacterToTexture(symbols[i]);
                var charPixels = charTexture.GetPixels();
                
                var x = i % atlasSize;
                var y = i / atlasSize; // Bottom to top, starting at 0
                atlas.SetPixels(x * characterSize, y * characterSize, characterSize, characterSize, charPixels);
                
                Object.DestroyImmediate(charTexture);
            }
            
            atlas.Apply();
            
            return atlas;
        }

        private Texture2D RenderCharacterToTexture(char character)
        {
            if (!sourceFont.GetCharacterInfo(character, out var charInfo, characterSize, fontStyle))
                throw new Exception($"Character {character} not found in font");

            var previousActive = RenderTexture.active;
            var rt = RenderTexture.GetTemporary(characterSize, characterSize, 24, RenderTextureFormat.ARGB32);
            
            RenderTexture.active = rt;
            
            RenderGlyphWithGL(charInfo);

            var texture = new Texture2D(characterSize, characterSize, TextureFormat.RGBA32, false);
            texture.ReadPixels(new Rect(0, 0, characterSize, characterSize), 0, 0);
            texture.Apply();
            
            RenderTexture.active = previousActive;
            RenderTexture.ReleaseTemporary(rt);
            
            return texture;
        }

        private void RenderGlyphWithGL(CharacterInfo charInfo)
        {
            GL.Clear(true, true, Color.clear);
            
            // Set up orthographic projection
            GL.PushMatrix();
            GL.LoadPixelMatrix(0, characterSize, characterSize, 0);
            
            // Get the font texture (works with dynamic fonts)
            var fontTexture = sourceFont.material.mainTexture;
                
            var fontMaterial = new Material(textShader)
            {
                mainTexture = fontTexture,
                color = textColor
            };
            fontMaterial.SetPass(0);
                
            // Calculate position to center the character
            var charWidth = charInfo.glyphWidth;
            var charHeight = charInfo.glyphHeight;
            var offsetX = (characterSize - charWidth) * 0.5f;// + charInfo.minX;
            var offsetY = (characterSize - charHeight) * 0.5f;// + charInfo.minY;
                
            // Draw the character quad
            GL.Begin(GL.QUADS);
                
            // Bottom-left
            GL.TexCoord2(charInfo.uvBottomLeft.x, charInfo.uvBottomLeft.y);
            GL.Vertex3(offsetX, offsetY + charHeight, 0);
                
            // Bottom-right
            GL.TexCoord2(charInfo.uvBottomRight.x, charInfo.uvBottomRight.y);
            GL.Vertex3(offsetX + charWidth, offsetY + charHeight, 0);
                
            // Top-right
            GL.TexCoord2(charInfo.uvTopRight.x, charInfo.uvTopRight.y);
            GL.Vertex3(offsetX + charWidth, offsetY, 0);
                
            // Top-left
            GL.TexCoord2(charInfo.uvTopLeft.x, charInfo.uvTopLeft.y);
            GL.Vertex3(offsetX, offsetY, 0);
                
            GL.End();
            GL.PopMatrix();
                
            Object.DestroyImmediate(fontMaterial);
        }
    }
}