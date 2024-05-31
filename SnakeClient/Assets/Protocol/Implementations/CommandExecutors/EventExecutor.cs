using FlatBuffers;
using MessageSchemes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace Assets.State.Executors
{
    internal class EventExecutor : ICommandExecutor
    {
        private FrameDisplay Display;

        [Inject]
        public void Construct(FrameDisplay display)
        {
            Display = display;
        }

        public bool TryExecute(Stream stream)
        {
            if (stream.ReadByte() != 0)
            {
                stream.Position -= 1;
                return false;
            }
            var lengthBuffer = new byte[4];
            stream.Read(lengthBuffer);
            var messageLength = BitConverter.ToUInt32(lengthBuffer);
            
            var messageBuffer = new byte[messageLength];
            stream.Read(messageBuffer);
            var loader = new ByteBuffer(messageBuffer);
            var message = EventMessage.GetRootAsEventMessage(loader);
            Display.Synchronize(message);
            return true;
        }
    }
}
