using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SyslogSender
{
  internal interface ISender
  {
    public IPEndPoint EndPoint { get; set; }

    public Task StartSendingPackets();
  }
}
