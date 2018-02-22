using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Messages
{
    public class ResponseMessage
    {
        public Status Status { get; set; }
        public String Error { get; set; }
        public String Data { get; set; }
    }
}
