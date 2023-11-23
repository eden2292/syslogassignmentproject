using System;
using SyslogAssignmentProject.Classes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkAirTestingAssignment
{
  [TestClass]
  public class SyslogMessageTests
  {
    [TestMethod]
    public void Test_If_Incorrectly_Formatted_Message_Returns_Expected_ParseFailures()
    {
      SyslogMessage newMessage = new SyslogMessage("192.168.0.3", 514, "127.0.0.1", 500, DateTimeOffset.Now, "123456  aaaaaa", "udp");
      SyslogMessage.ParseFailure failLevel = newMessage.ParseMessage();

      if ((failLevel & SyslogMessage.ParseFailure.Priority) == 0)
      {
        Assert.Fail($"Syslog severity parse reported successful, expected unsuccessful");
      }
    }

    [TestMethod]
    public void Test_If_Correctly_Formatted_Message_Returns_Expected_ParseFailures()
    {
      SyslogMessage newMessage = new SyslogMessage("192.168.0.3", 514, "127.0.0.1", 500, DateTimeOffset.Now, "<1>1 2003-10-11T22:14:15.003Z - - - - hello", "udp");
      SyslogMessage.ParseFailure failLevel = newMessage.ParseMessage();

      if (failLevel == 0)
      {
        Assert.Fail($"Syslog severity parse reported unsuccessful, expected successful");
      }
    }
  }
}
