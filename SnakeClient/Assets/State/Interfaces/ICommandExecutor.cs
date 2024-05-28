using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.State
{
    public interface ICommandExecutor
    {
        public bool TryExecute(Stream stream);
    }
}
