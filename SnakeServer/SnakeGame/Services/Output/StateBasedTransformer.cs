using ServerEngine.Interfaces.Output;
using SnakeGame.Models.Output.External;
using SnakeGame.Models.Output.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MessageSchemes;
using FlatSharp;
using SnakeCore.Collections;

namespace SnakeGame.Services.Output
{
    internal class StateBasedTransformer : IOutputCollector<Group>, IOutputProvider<StateBasedBinaryOutput>
    {
        private readonly List<Group> _groups = [];
        public StateBasedBinaryOutput Get()
        {
            var message = new Message()
            {
                Groups = _groups
            };
            var size = Message.Serializer.GetMaxSize(message);
            var buffer = new byte[size + 4];
            var lenghtBytes = BitConverter.GetBytes(buffer.Length);
            lenghtBytes.CopyTo(buffer, 0);
            Message.Serializer.Write(new SpanWriter(), buffer.AsSpan(4), message);
            return new StateBasedBinaryOutput()
            {
                Data = buffer
            };
        }

        public void Pass(Group data)
        {
            _groups.Add(data);
        }
    }
}
