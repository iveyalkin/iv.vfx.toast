using System;
using UnityEngine;

namespace iv.vfx.toast.editor
{
    /// <summary>
    /// Example: Custom Symbols Texture Data for your own font atlas
    /// This is a template showing how to create your own symbol mapping.
    /// 
    /// To create a custom version:
    /// 1. Use the Font Atlas Generator tool (Tools → iv.vfx.toast → Font Atlas Generator)
    /// 2. Generate your custom atlas texture
    /// 3. Copy the generated code snippet into a new class
    /// 4. Update your ToastSpawner to use your custom SymbolsTextureData
    /// </summary>
    [Serializable]
    public struct CustomSymbolsTextureData
    {
        // CONFIGURATION - Update these based on your atlas
        private static readonly Vector2Int AtlasDimension = new Vector2Int(4, 4);
        private const int atlasLength = 16;
        
        // SYMBOL INDICES - Define indices for each symbol in your atlas
        // The index represents the position in the grid (left to right, bottom to top)
        // Example for digits 0-9 and special characters: "0123456789+- >.k"
        
        private const int char_0_Index = 0;
        private const int char_1_Index = 1;
        private const int char_2_Index = 2;
        private const int char_3_Index = 3;
        private const int char_4_Index = 4;
        private const int char_5_Index = 5;
        private const int char_6_Index = 6;
        private const int char_7_Index = 7;
        private const int char_8_Index = 8;
        private const int char_9_Index = 9;
        private const int plusIndex = 10;
        private const int minusIndex = 11;
        private const int spaceIndex = 12;
        private const int periodIndex = 13;
        private const int bracketIndex = 14;
        private const int kCharIndex = 15;
        
        // Internal dictionary mapping indices to UV coordinates
        private Vector2Int[] charsDict;
        
        /// <summary>
        /// Converts a linear index to 2D grid coordinates
        /// </summary>
        private Vector2Int this[int index] => new Vector2Int(
            index % AtlasDimension.x, 
            index / AtlasDimension.x
        );
        
        /// <summary>
        /// Gets the UV coordinates for a character in the atlas
        /// </summary>
        public Vector2Int this[char c]
        {
            get
            {
                int atlasIndex = GetAtlasIndex(c);
                return charsDict[atlasIndex];
            }
        }
        
        /// <summary>
        /// Maps a character to its atlas index
        /// UPDATE THIS METHOD with your custom character mapping
        /// </summary>
        private int GetAtlasIndex(char c)
        {
            switch (c)
            {
                // Digits (0-9)
                case >= '0' and <= '9':
                    return c - '0'; // Maps '0' to index 0, '1' to index 1, etc.
                
                // Special characters
                case ' ':
                    return spaceIndex;
                case '+':
                    return plusIndex;
                case '-':
                    return minusIndex;
                case '.':
                    return periodIndex;
                case '>':
                    return bracketIndex;
                case 'k':
                    return kCharIndex;
                
                // Add more cases here for your custom symbols
                // Example:
                // case 'A':
                //     return char_A_Index;
                // case '!':
                //     return exclamationIndex;
                
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Unsupported symbol for texture: '{c}' (code: {(int)c})"
                    );
            }
        }
        
        /// <summary>
        /// Creates and initializes a new CustomSymbolsTextureData instance
        /// Call this once to set up the UV coordinate dictionary
        /// </summary>
        public static CustomSymbolsTextureData Create()
        {
            var textureData = new CustomSymbolsTextureData
            {
                charsDict = new Vector2Int[atlasLength]
            };
            
            // Initialize all possible positions in the atlas
            for (var i = 0; i < atlasLength; i++)
            {
                textureData.charsDict[i] = textureData[i];
            }
            
            return textureData;
        }
        
        /// <summary>
        /// Validates that a string contains only supported characters
        /// </summary>
        public bool CanDisplay(string text)
        {
            foreach (char c in text)
            {
                try
                {
                    GetAtlasIndex(c);
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
            }
            return true;
        }
        
        /// <summary>
        /// Gets the list of all supported characters
        /// Useful for debugging and validation
        /// </summary>
        public static string GetSupportedCharacters()
        {
            return "0123456789+- >.k";
        }
    }
}
