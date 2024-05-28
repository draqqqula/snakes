using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.State
{
    public interface IMessageReader
    {
        public void Read(byte[] buffer);
    }
}
