using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LutherBackup.Models
{
    public class BackupConfig
    {
        public string Device;
        public string RoboCopyArgs;
        public string RoboCopyLogArg;
        public string RoboCopyExcludeFiles;
        public string RoboCopyExcludeFolders;

        public BackupDeviceInfo DeviceInfo;
    }
}
