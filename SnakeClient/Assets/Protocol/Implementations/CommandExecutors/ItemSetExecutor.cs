using Assets.State;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.InputSystem.XR;

namespace Assets.Protocol.Implementations.CommandExecutors
{
    public abstract class ItemSetExecutor : ICommandExecutor
    {
        public bool TryExecute(Stream stream)
        {
            if (stream.ReadByte() != SignatureByte)
            {
                stream.Position -= 1;
                return false;
            }
            var count = stream.ReadByte();
            for (int i = 0; i < count; i++)
            {
                ParseItem(stream);
            }

            return true;
        }

        public abstract byte SignatureByte { get; }

        public abstract void ParseItem(Stream stream);
    }
}
