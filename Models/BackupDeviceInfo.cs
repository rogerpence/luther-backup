using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LutherBackup.Models
{
 
    public class BackupDeviceInfo
    {
        public string Device { get; set; }
        public string DriveLetter { get; set; }
        public string[] SourceDirectory { get; set; }
        public string[] TargetDevice { get; set; }
    }
}
