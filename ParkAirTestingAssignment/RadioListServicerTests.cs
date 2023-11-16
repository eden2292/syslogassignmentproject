using Microsoft.AspNetCore.Mvc.Formatters;
using SyslogAssignmentProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkAirTestingAssignment
{
  [TestClass]
  public class RadioListServicerTests
  {
    [TestMethod]
    public void Test_If_RadioListServicer_IP_Filter_Works()
    {
      RadioListServicer radioList = new RadioListServicer();
      Radio radioToAdd = new Radio("T6S3", "192.168.1.4", 50000, "TCP");

      radioList.UpdateList(radioToAdd);

      Assert.AreEqual("T6S3", radioList.RadioStore[0].Name);
      Assert.AreEqual("192.168.1.4", radioList.RadioStore[0].IPAddress);
      Assert.AreEqual(50000, radioList.RadioStore[0].PortNumber);
      Assert.AreEqual("TCP", radioList.RadioStore[0].TransportProtocol);
    }

    [TestMethod]
    public void Test_If_RadioListServicer_Unique_IP_Sort_Works()
    {
      RadioListServicer radioList = new RadioListServicer();
      int counter = 0;
      string temp = "";

      List<string> IPs = new List<string>();

      Radio radio1 = new Radio("T6S3", "192.168.1.4", 50000, "TCP");
      radioList.UpdateList(radio1);
      IPs.Add("192.168.1.4");
      Radio radio2 = new Radio("T6S3", "192.168.1.5", 50000, "TCP");
      radioList.UpdateList(radio2);
      IPs.Add("192.168.1.5");
      Radio radio3 = new Radio("T6S3", "192.168.1.6", 50000, "TCP");
      radioList.UpdateList(radio3);
      IPs.Add("192.168.1.6");
      Radio radio4 = new Radio("T6S3", "192.168.1.4", 50000, "TCP");
      radioList.UpdateList(radio4);
      IPs.Add("192.168.1.4");
      Radio radio5 = new Radio("T6S3", "192.168.1.5", 50000, "TCP");
      radioList.UpdateList(radio5);
      IPs.Add("192.168.1.5");

      List<string> uniqueIPsReturned = radioList.UniqueIpAddresses();
      foreach (string ip in uniqueIPsReturned)
      {
        temp = ip;
        foreach(string ips in uniqueIPsReturned)
        {
          if (temp == ips)
          {
            counter++;
          }
          if (counter > 1)
          {
            Assert.Fail($"Unexpected repeated occurrence of {ips}");
          }
        }
        counter = 0;
      }
    }
  }
}
