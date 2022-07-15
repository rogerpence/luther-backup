using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using LutherBackup.Models;
using CliWrap;
using static LutherBackup.ConsoleHelpers;
using Serilog;

namespace LutherBackup.Logic;
public class BackupManager
{
    IConfiguration config;

    public BackupManager(IConfiguration config)
    {
        this.config = config;
    }

    public async Task Run(string sourceDevice)
    {
        ConfigurationValues cv = new();
        BackupConfig[] backupConfigs = cv.GetConfiguration(config);

        foreach (var backupConfig in backupConfigs)
        {
            if (sourceDevice.Equals(backupConfig.DeviceInfo.Device, StringComparison.OrdinalIgnoreCase) ||
                sourceDevice.Equals("*all", StringComparison.OrdinalIgnoreCase))
            {
                await RunBackupForConfig(backupConfig);
            }
        }
    }

    public async Task RunBackupForConfig(BackupConfig backupConfig)
    {
        Log.Information($" *****  ");
        Log.Information($" *****  ");
        Log.Information($"Starting backup for device: {backupConfig.DeviceInfo.Device}");

        ConfigurationValues cv = new();
         
        WriteMessage($"Initiating backup for source device: {backupConfig.DeviceInfo.Device}");
                
        foreach (var targetDevice in backupConfig.DeviceInfo.TargetDevice)
        {
            string targetDrive = cv.GetBackupDeviceDrive(targetDevice);

            foreach (var sourceDir in backupConfig.DeviceInfo.SourceDirectory)
            {
                string sourceDevice = backupConfig.DeviceInfo.Device;

                if (backupConfig.DeviceInfo.DriveLetter == Constants.DRIVE_NOT_AVAILABLE)
                {
                    Log.Warning($"No backup performed for: {sourceDevice}. Source device drive not found.");
                    WriteError($"  No backup performed for: {sourceDevice}. Source device drive not found.");
                    return;
                }

                await BackUpForTargetDevice(backupConfig, targetDevice, targetDrive, sourceDir, sourceDevice);
            }
        }
    }
    private async Task BackUpForTargetDevice(BackupConfig backupConfig, string targetDevice, string targetDrive, string sourceDir, string sourceDevice)
    {
        string args = backupConfig.RoboCopyArgs;
        string excludeFiles = backupConfig.RoboCopyExcludeFiles;
        string excludeFolders = backupConfig.RoboCopyExcludeFolders;
        string logarg = backupConfig.RoboCopyLogArg;

        string targetDir = sourceDir.Replace("\\", "-");

        string fullSourceDir = Path.Join(backupConfig.DeviceInfo.DriveLetter, sourceDir);
        if (!CheckFolderExists(sourceDevice, fullSourceDir)) return;

        string fullTargetDir = Path.Join(targetDrive, Constants.BACKUP_ROOT_FOLDER, sourceDevice, targetDir);
       
        var stdOutBuffer = new StringBuilder();
        var stdErrBuffer = new StringBuilder();

        if (targetDrive == Constants.DRIVE_NOT_AVAILABLE)
        {
            Log.Warning($"No backup performed for: {sourceDevice} on target device {targetDevice}. Target device drive not found.");
            WriteError($"  No backup performed for: {sourceDevice} on target device {targetDevice}. Target device drive not found.");
            return;
        }
        else
        {
            Log.Information($"Backing up {targetDir} on {sourceDevice} to {targetDevice}");
            WriteSuccess($"  Backing up {targetDir} on {sourceDevice} to {targetDevice}");
            WriteWarning($"  Backup started at {DateTime.Now.ToString("hh:mm:ss")}");
        }

        string logFile = Path.Join(targetDrive, Constants.BACKUP_ROOT_FOLDER, $"{sourceDevice}-{targetDir}.log");
        string RoboLogFile = $@"{logarg}{logFile}";
        string RoboArgs= $@" ""{fullSourceDir}"" ""{fullTargetDir}"" {args} {excludeFiles} {excludeFolders} {RoboLogFile}";
        Log.Information($"Using: robocopy {RoboArgs}");

        //WriteMessage($" with command RoboCopy {RoboArgs} ");

        try        
        {
            var result = await Cli.Wrap("robocopy")
            .WithArguments(RoboArgs)
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdOutBuffer))
            .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
            .WithValidation(CommandResultValidation.None)
            .ExecuteAsync();

            string statusMessage;
            if (Constants.RoboCopyStatusCodes.ContainsKey(result.ExitCode))
            {
                statusMessage = Constants.RoboCopyStatusCodes[result.ExitCode];
                Log.Information($"Finished with status code: {result.ExitCode}.\n  {statusMessage}");
                WriteSuccess($"  Finished with status code: {result.ExitCode}.\n  {statusMessage}");
            }
            else
            {
                statusMessage = "At least one failure occurred--check the RoboCopy error log.";
                Log.Error($"A failure occurred: {result.ExitCode}.\n  Check log--this is probably not an issue.\n  {statusMessage}");
                WriteWarning($"  A failure occurred: {result.ExitCode}.\n  Check log--this is probably not an issue.\n  {statusMessage}");
            }
            Log.Information($"Backup finished at {DateTime.Now.ToString("hh:mm:ss")}");
            WriteWarning($"  Backup finished at {DateTime.Now.ToString("hh:mm:ss")}");

        }
        catch (Exception ex)
        {
            WriteError(ex.Message);
            WriteError(stdOutBuffer.ToString());
            WriteError(stdErrBuffer.ToString());
        }
    }

    public bool CheckFolderExists(string sourceDevice, string folder)
    {
        if (! Directory.Exists(folder))
        {
            WriteError($"Folder {folder} does not exist on source device {sourceDevice}");
            return false;
        }

        return true;
    }

    public void ShowConfig(string sourceDevice)
    {
        ConfigurationValues cv = new();

        BackupConfig[] backupConfigs = cv.GetConfiguration(config);

        WriteSuccess($"RoboCopy arguments:");
        WriteMessage($"  args: {backupConfigs[0].RoboCopyArgs}");
        WriteMessage($"  excludeFiles: {backupConfigs[0].RoboCopyExcludeFiles}");
        WriteMessage($"  excludeFolders: {backupConfigs[0].RoboCopyExcludeFolders}");
        WriteMessage($"  logarg: {backupConfigs[0].RoboCopyLogArg}");
        WriteEmptyLine();

        foreach (var backupConfig in backupConfigs)
        {
            if (sourceDevice.Equals(backupConfig.DeviceInfo.Device, StringComparison.OrdinalIgnoreCase) ||
                sourceDevice.Equals("*all", StringComparison.OrdinalIgnoreCase))
            {
                WriteSuccess("Source device:");
                WriteMessage($"  {backupConfig.DeviceInfo.Device} at drive: {backupConfig.DeviceInfo.DriveLetter}");

                WriteSuccess("Source directories");
                foreach (var item in backupConfig.DeviceInfo.SourceDirectory)
                {
                    WriteMessage($"  -{item}");
                }
                
                WriteSuccess("Target devices");
                foreach (var item in backupConfig.DeviceInfo.TargetDevice)
                {
                    string driveId = cv.GetBackupDeviceDrive(item);
                    WriteMessage($"  -{driveId} | {item}");
                }

                WriteSuccess("--------------------");
            }
        }

    }

}




