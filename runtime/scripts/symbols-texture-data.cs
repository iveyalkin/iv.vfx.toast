using System;
using UnityEngine;

namespace iv.vfx.toast
{
    /// <summary>
    /// Character texture encoding data Данные кодирования символов в текстуре
    /// Supports arabian digits, space and symbols +->.k
    /// </summary>
    [Serializable]
    public struct SymbolsTextureData
    {
        private static readonly Vector2Int AtlasDimension = new (4, 4);

        private const int plusIndex = 10;
        private const int minusIndex = 11;
        private const int spaceIndex = 12;
        private const int periodIndex = 13;
        private const int bracketIndex = 14;
        private const int kCharIndex = 15;
        private const int atlasLength = 16;
 
        // character coordinates [rows columns]
        private Vector2Int[] charsDict;

        private Vector2Int this[int index] => new (index % AtlasDimension.x, index / AtlasDimension.y);

        public Vector2Int this[char c]
        {
            get
            {
                int atlasIndex;
                switch (c)
                {
                    case ' ':
                        atlasIndex = spaceIndex;
                        break;
                    case '+':
                        atlasIndex = plusIndex;
                        break;
                    case '-':
                        atlasIndex = minusIndex;
                        break;
                    case '>':
                        atlasIndex = bracketIndex;
                        break;
                    case '.':
                        atlasIndex = periodIndex;
                        break;
                    case 'k':
                        atlasIndex = kCharIndex;
                        break;
                    // digits ASCII code dec: 48 - 57
                    case >= '0' and <= '9':
                        atlasIndex = c - '0';
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unsupported symbol: {c}");
                }

                return charsDict[atlasIndex];
            }
        }

        public static SymbolsTextureData Create()
        {
            var textureData = new SymbolsTextureData
            {
                charsDict = new Vector2Int[atlasLength]
            };

            for (var i = 0; i < 10; i++)
            {
                // converts char index to row and column
                var uv = textureData[i];
                textureData.charsDict[i] = uv;
            }

            textureData.charsDict[plusIndex] = textureData[plusIndex];
            textureData.charsDict[minusIndex] = textureData[minusIndex];
            textureData.charsDict[spaceIndex] = textureData[spaceIndex];
            textureData.charsDict[periodIndex] = textureData[periodIndex];
            textureData.charsDict[bracketIndex] = textureData[bracketIndex];
            textureData.charsDict[kCharIndex] = textureData[kCharIndex];

            return textureData;
        }
    }
}