using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.Configuration;
using LutherBackup.Models;

namespace LutherBackup.Logic;

public class ConfigurationValues
{
    public Dictionary<string, string> TargetDeviceDrives = new();

    public BackupConfig[] GetConfiguration(IConfiguration config)
    {
        List<BackupConfig> backupConfig = new();

        BackupDeviceInfo[] devicesInfo = config.GetSection("DeviceInfo").Get<BackupDeviceInfo[]>();

        foreach (var deviceInfo in devicesInfo)
        {
            deviceInfo.DriveLetter = GetDeviceDrive(deviceInfo.Device);

            BackupConfig bc = new()
            {
                RoboCopyArgs = config["Config:RoboCopyArgs"],
                RoboCopyExcludeFiles = config["Config:RoboCopyExcludeFiles"],
                RoboCopyExcludeFolders = config["Config:RoboCopyExcludeFolders"], 
                RoboCopyLogArg = config["Config:RoboCopyLogArg"], 
                DeviceInfo = deviceInfo,
            };

            foreach (var backupDevice in deviceInfo.TargetDevice)
            {
                if (! TargetDeviceDrives.ContainsKey(backupDevice))
                {
                    TargetDeviceDrives.Add(backupDevice, GetDeviceDrive(backupDevice));
                }
            }

            // Remove drive from SourceDirectory
            // string test2 = Regex.Replace(test, @"(.\:\\s*)", "");
            

            backupConfig.Add(bc);
        };

        return backupConfig.ToArray();
    }

    public string GetBackupDeviceDrive(string deviceName)
    {
        return GetDeviceDrive(deviceName);
    }

    public string GetDeviceDrive(string deviceName)
    { 
        DriveInfo[] drives = DriveInfo.GetDrives();

        foreach (var drive in drives)
        {
            var driveLetter = drive.RootDirectory.FullName;

            if (getDeviceIdFromFile(driveLetter, deviceName))
            {
                return driveLetter;
            }

            //var fileName = Path.Combine(driveLetter, Constants.DEVICE_ID_FILE_NAME);
            //if (File.Exists(fileName))
            //{
            //    string contents = File.ReadAllText(fileName).Trim();
            //    if (File.ReadAllText(fileName).Trim() == deviceName)
            //    {
            //        return driveLetter;
            //    }
            //}
        }

        return Constants.DRIVE_NOT_AVAILABLE;
    }

    private bool getDeviceIdFromFile(string driveLetter, string deviceName)
    {
        string fileName;

        // Check for device name defined in AppData folder. 
        if (driveLetter.ToLower().StartsWith("c"))
        {
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            fileName = Path.Combine(driveLetter, appDataFolder, Constants.DEVICE_ID_FILE_NAME);
            if (File.Exists(fileName) && File.ReadAllText(fileName).Trim() == deviceName) return true;
        }

        // Check for device named defined in root of drive.
        fileName = Path.Combine(driveLetter, Constants.DEVICE_ID_FILE_NAME);
        if (File.Exists(fileName) && File.ReadAllText(fileName).Trim() == deviceName) return true;

        return false;   
    }


}
