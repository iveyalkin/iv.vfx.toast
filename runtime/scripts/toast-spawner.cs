using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.VFX;

namespace iv.vfx.toast
{
    [ExecuteInEditMode]
    [AddComponentMenu("iv/vfx/ToastSpawner")]
    public class ToastSpawner : MonoBehaviour
    {
        private const uint maxMessageLength = 14u; // in case of 4x4 atlas, 4 bits is enough (x 2bit & y 2bit) to encode
                                                   // coordinates x & y of Vec2 (total of 64 bits) up to 16 characters.
                                                   // however, 8 bits are reserved to encode message length

        [SerializeField]
        private VisualEffect effect;

        private readonly SymbolsTextureData symbolsTextureData = SymbolsTextureData.Create();

        // message encoded as char coordinates within atlas
        private readonly Vector2Int[] messageSymbolsCoords = new Vector2Int[maxMessageLength];

        private int actualCount = 0;
        private readonly ToastParticleData[] particlesData = new ToastParticleData[16];
        // private readonly byte[] particlesData = new byte[8];
        
        private int BufferSize => particlesData.Length;
        // private int BufferSize => particlesDataBytes.Length;

        private GraphicsBuffer buffer;

        private static readonly int bufferPropertyID = Shader.PropertyToID("ParticlesBuffer");
        private static readonly int toastEventID = Shader.PropertyToID("OnToast");

        [ContextMenu("TestText")]
        public void TestText()
        {
            Show(transform.position, "+1", Color.red);
        }

        public void Show(Vector3 position, string toast, Color color)
        {
            var messageLenght = (ushort)Mathf.Min((int)maxMessageLength, toast.Length);

            for (var i = 0; i < messageLenght; i++)
            {
                messageSymbolsCoords[i] = symbolsTextureData[toast[i]];
            }

            particlesData[0].Set(messageSymbolsCoords, messageLenght);
            buffer.SetData(particlesData);
            
            // particlesDataBytes[0] = 1;
            // particlesDataBytes[1] = 0;
            
            effect.SendEvent(toastEventID);
        }

        private void Awake()
        {
            effect = GetComponent<VisualEffect>();
        }

        private void OnEnable()
        {
            ReallocateBuffer(BufferSize);
            effect.SetGraphicsBuffer(bufferPropertyID, buffer);
        }

        private void OnDisable() => ReleaseBuffer();
 
        private void OnDestroy() => ReleaseBuffer();
        
        private void ReallocateBuffer(int size)
        {
            buffer?.Release();
 
            buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, size, Marshal.SizeOf(typeof(ToastParticleData)));
            buffer.SetData(particlesData);
            // buffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, size, size);
            // buffer.SetData(particlesData);
        }
        
        private void ReleaseBuffer()
        {
            if (buffer == null) return;
 
            buffer.Release();
            buffer = null;
        }
    }
}