using SyslogAssignmentProject;
using SyslogAssignmentProject.Classes;

namespace ParkAirTestingAssignment
{
  [TestClass]
  public class UnitTest1
  {
    [TestMethod]
    public void Test_If_UDPSyslogReceiver_Sets_Port_Number()
    {
      GlobalInjection globalInjection = new GlobalInjection();
      UdpSyslogReceiver udpReceiver = new UdpSyslogReceiver(
        globalInjection, globalInjection.S_RadioList, globalInjection.S_LiveFeedMessages);

      Assert.AreEqual(udpReceiver.ListeningPort, globalInjection.S_ReceivingPortNumber);
    }

    [TestMethod]
    public void Test_If_TCPSyslogReceiver_Sets_Port_Number()
    {
      GlobalInjection globalInjection = new GlobalInjection();
      TcpSyslogReceiver tcpReceiver = new TcpSyslogReceiver(
        globalInjection, globalInjection.S_RadioList, globalInjection.S_LiveFeedMessages);

      Assert.AreEqual(tcpReceiver.ListeningPort, globalInjection.S_ReceivingPortNumber);
    }

    [TestMethod]
    public void Test_If_TCPSyslogReceiver_Sets_IP_Address()
    {
      GlobalInjection globalInjection = new GlobalInjection();
      TcpSyslogReceiver tcpReceiver = new TcpSyslogReceiver(
        globalInjection, globalInjection.S_RadioList, globalInjection.S_LiveFeedMessages);

      Assert.AreEqual(tcpReceiver.ListeningIP, globalInjection.S_ReceivingIpAddress);
    }

    [TestMethod]
    public void Test_If_TCP_And_UDP_Receivers_Listen_Without_Exceptioning()
    {
      try
      {
        GlobalInjection globalInjection = new GlobalInjection();
        UdpSyslogReceiver udpReceiver = new UdpSyslogReceiver(
          globalInjection, globalInjection.S_RadioList, globalInjection.S_LiveFeedMessages);
        TcpSyslogReceiver tcpReceiver = new TcpSyslogReceiver(
        globalInjection, globalInjection.S_RadioList, globalInjection.S_LiveFeedMessages);

        udpReceiver.StartListening();
        tcpReceiver.StartListening();

        udpReceiver.TokenToStopSource.Cancel();
        tcpReceiver.TokenToStopSource.Cancel();
      }
      catch (Exception ex)
      {
        Assert.Fail($"Expected no exception, but got {ex.Message}");
      }
    }
  }
}