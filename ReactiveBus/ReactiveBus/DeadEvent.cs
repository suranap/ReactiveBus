using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveBus
{
    public class DeadEvent
    {
        public object Value { get; private set; }
        public DeadEvent(object v)
        {
            Value = v;
        }
    }
}
