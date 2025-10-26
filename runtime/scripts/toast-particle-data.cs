using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.VFX;

namespace iv.vfx.toast
{
    [VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
    public struct ToastParticleData
    {
        public Vector2 customData;
        public uint length;

        public void Set(Vector2Int[] message, ushort length)
        {
            this.customData = CreateCustomData(message, length);
            this.length = length;
        }

        // =============================================================================
        // 

        private const int vectorLength = 2; // количество компонент в одном векторе customData
        private const int coordsPerFloat = 8; // количество координат в одном float

        private static float PackFloat(Vector2Int[] texCoords, ushort messageLength, int offset)
        {
            var packedValue = 0u;

            for (var i = 0; i < coordsPerFloat; i++)
            {
                packedValue <<= 4; // сдвигаем чтобы освободить место для новых данных
                var coordIndex = offset + i;

                if (coordIndex < messageLength)
                {
                    var coord = texCoords[coordIndex];

                    packedValue ^= ((uint)coord.x & 0b11) << 2; // кодируем координату x в старшие 2 бита
                    packedValue ^= (uint)coord.y & 0b11; // кодируем координату y в младшие 2 бита
                }
            }

            return ConvertToFloat(packedValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe float ConvertToFloat(uint value) => *(float*)&value;

        /// <summary>
        /// упаковка для потока CustomData
        /// </summary>
        private static Vector2 CreateCustomData(Vector2Int[] texCoords, ushort messageLength)
        {
            var data = Vector2.zero;
            for (var i = 0; i < vectorLength; i++)
            {
                data[i] = PackFloat(texCoords, messageLength, i * coordsPerFloat);
            }

            // PackToastLength(messageLength, data);

            return data;
        }

        private static void PackToastLength(ushort messageLength, Vector2 data)
        {
            var lastIndex = vectorLength - 1;
            var last = (uint)BitConverter.SingleToInt32Bits(data[lastIndex]);
            uint mask = 0xFF; // 8 бит для длины сообщения

            // длина сообщения в младших 8 битах последнеего элемента вектора
            data[lastIndex] = BitConverter.Int32BitsToSingle((int)(last & ~mask ^ messageLength));
        }
    }
}