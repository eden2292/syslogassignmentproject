using Microsoft.Extensions.Logging;
using SyslogAssignmentProject.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkAirTestingAssignment
{
  [TestClass]
  public class SyslogExportTest
  {
    private string projectDirectory = AppDomain.CurrentDomain.BaseDirectory;

    [TestMethod]
    public void Test_If_Zip_Folder_Exists()
    {
      string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
      int charactersToRemove = 43;
      string trimmedDirectory = baseDirectory.Remove(baseDirectory.Length - charactersToRemove);
      trimmedDirectory += "\\SyslogAssignmentProject\\bin\\Debug\\net7.0";
      string filePath = Path.Combine(trimmedDirectory, "Logs.zip");
      Assert.IsTrue(File.Exists(filePath), "File has to be in the project directory");
    }
  }
}