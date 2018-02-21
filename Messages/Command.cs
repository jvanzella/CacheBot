using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public abstract class Command
    {
        public Environment Environment { get; set; } 
        public CommandType CommandType { get; set; }

    }
}
