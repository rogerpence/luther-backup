using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LutherBackup
{
    public class Constants
    {
        public const string DRIVE_NOT_AVAILABLE = "NA";
        public const string DEVICE_ID_FILE_NAME = "luther-drive-id.txt";
        public const string BACKUP_ROOT_FOLDER = "luther-backup-2.0";

        // Status code 4 is mysteriously not documented.
        // https://docs.microsoft.com/en-us/windows-server/administration/windows-commands/robocopy
        // I found wording for #4 in other docs--but it looks suspiciously out of date.
        // I think 1 through 4 are considered successful copies.

        public static Dictionary<int, string> RoboCopyStatusCodes = new Dictionary<int, string>()
        {
            {0, "No files were copied"},
            {1, "All files were copied succesfully"},
            {2, "There are some files in the destination directory that are not present in the source directory. No files were copied."},
            {3, "Some files were copied. Additional files were present. No failure was encountered."},
            {4, "Some Mismatched files or directories were detected. "},
            {5, "Some files were copied. Some files were mismatched. No failure was encountered."},
            {6, "Additional files and mismatched files exist in the destination directory. No files were copied."},
            {7, "Files were copied, a file mismatch was present, and additional files were present."},
            {8, "Several files did not copy."}
        };
    }
}
