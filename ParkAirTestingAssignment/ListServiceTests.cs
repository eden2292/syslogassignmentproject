using SyslogAssignmentProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkAirTestingAssignment
{
  [TestClass]
  public class ListServiceTests
  {
    [TestMethod]
    public void Test_If_ListServicer_IP_Filter_Works()
    {
      string expectedIP = "192.168.1.4";

      ListServicer messageList = new ListServicer();

      SyslogMessage message1 = new SyslogMessage("127.0.0.1", 514, expectedIP, 50000, DateTimeOffset.Now, "<>1 2023-11-16T14:01:58.015Z - - - - Message1", "UDP");
      messageList.SyslogMessageList.Add(message1);
      SyslogMessage message2 = new SyslogMessage("127.0.0.1", 514, "192.168.1.5", 50000, DateTimeOffset.Now, "<3>1 2023-11-16T14:01:58.015Z - - - - Message2", "UDP");
      messageList.SyslogMessageList.Add(message2);
      SyslogMessage message3 = new SyslogMessage("127.0.0.1", 514, "192.168.1.5", 50000, DateTimeOffset.Now, "<>1 2023-11-16T14:01:58.015Z - - - - Message3", "UDP");
      messageList.SyslogMessageList.Add(message3);
      SyslogMessage message4 = new SyslogMessage("127.0.0.1", 514, expectedIP, 50000, DateTimeOffset.Now, "<4>1 2023-11-16T14:01:58.015Z - - - - Message4", "UDP");
      messageList.SyslogMessageList.Add(message4);
      SyslogMessage message5 = new SyslogMessage("127.0.0.1", 514, expectedIP, 50000, DateTimeOffset.Now, "<5>1 2023-11-16T14:01:58.015Z - - - - Message5", "UDP");
      messageList.SyslogMessageList.Add(message5);

      List<SyslogMessage> filteredList = messageList.FilterList(expectedIP, "None");
      foreach (SyslogMessage msg in filteredList)
      {
        Assert.AreEqual(msg.SenderIP, expectedIP);
      }
    }

    [TestMethod]
    public void Test_If_ListServicer_Severity_Filter_Works()
    {
      string expectedSeverity = "4";

      ListServicer messageList = new ListServicer();

      SyslogMessage message1 = new SyslogMessage("127.0.0.1", 514, "192.168.1.4", 50000, DateTimeOffset.Now, "<>1 2023-11-16T14:01:58.015Z - - - - Message1", "UDP");
      messageList.SyslogMessageList.Add(message1);
      SyslogMessage message2 = new SyslogMessage("127.0.0.1", 514, "192.168.1.5", 50000, DateTimeOffset.Now, $"<{expectedSeverity}>1 2023-11-16T14:01:58.015Z - - - - Message2", "UDP");
      messageList.SyslogMessageList.Add(message2);
      SyslogMessage message3 = new SyslogMessage("127.0.0.1", 514, "192.168.1.5", 50000, DateTimeOffset.Now, "<>1 2023-11-16T14:01:58.015Z - - - - Message3", "UDP");
      messageList.SyslogMessageList.Add(message3);
      SyslogMessage message4 = new SyslogMessage("127.0.0.1", 514, "192.168.1.4", 50000, DateTimeOffset.Now, $"<{expectedSeverity}>1 2023-11-16T14:01:58.015Z - - - - Message4", "UDP");
      messageList.SyslogMessageList.Add(message4);
      SyslogMessage message5 = new SyslogMessage("127.0.0.1", 514, "192.168.1.4", 50000, DateTimeOffset.Now, $"<{expectedSeverity}>1 2023-11-16T14:01:58.015Z - - - - Message5", "UDP");
      messageList.SyslogMessageList.Add(message5);

      List<SyslogMessage> filteredList = messageList.FilterList("None", expectedSeverity);
      foreach (SyslogMessage msg in filteredList)
      {
        Assert.AreEqual(msg.Severity, expectedSeverity);
      }
    }
  }
}
