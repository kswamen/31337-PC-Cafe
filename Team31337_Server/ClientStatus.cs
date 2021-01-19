using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;


namespace Team31337_Server
{
    public class ClientStatus
    {
        public int myNum;
        public string myID;
        public Socket mySocket;
        public Boolean isNumSent = false;
    }
}
