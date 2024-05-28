using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.State
{
    public class CommandReader : IMessageReader
    {
        private IEnumerable<ICommandExecutor> Executors;

        [Inject]
        public void Construct(List<ICommandExecutor> executors)
        {
            Executors = executors;
        }

        public void Read(byte[] buffer)
        {
            using var stream = new MemoryStream(buffer);
            while (stream.Position < stream.Length - 1)
            {
                foreach (var executor in Executors)
                {
                    if (executor.TryExecute(stream))
                    {
                        break;
                    }
                }
            }
        }
    }
}
