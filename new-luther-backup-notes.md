DriveInfo[] allDrives = DriveInfo.GetDrives();

/*

Yaml file that defines source device ids and their source_directory and target_device(s)
---s
config:
   - device: rpwin10git
     source_directory: users\roger\documents
     target_device: 
       - seagate-4tb-desktop
       - seagate-4tb-little    
   - device: wing10rp-1809
     source_directory: users\roger\documents
     target_device:
      - seagate-4tb-desktop
      - seagate-4tb-little
      - little-thumb
   - delray:
     source_directory: users\thumb\documents
     target_device:
     - seagate-4tb-desktop
     - seagate-4tb-little
  
*/


//traverse drives looking for luther-drive-id@xx.txt in root.
//this file's drive and its xx defines: 
// target_device, target_drive, target_directory 
// source_drive, source_device, and source_directory. 
 

string target_drive = "d:";
string target_directory = "luther-backup";

string source_drive = "c:";
string source_device = "rpwin10git";
string source_directory = @"users\roger\documents";

string robo_exclude_folders = @"/xd node_modules __pycache__ AppData dat env site-packages .git\";
string robo_exclude_files = @"/xf NTUSER.DAT* ntuser.ini *.gm2 *.gbp *.pst";
string robo_other_args = @"/mt:64 /mir /tee";

string source = $@"{source_drive}\{source_directory}";
source.Dump();

string target = $@"{target_drive}\{target_directory}\{source_device}\{source_directory}";
target.Dump();

string robo_logfile = $@"/log+:{target_drive}\{target_directory}\{source_device}-{source_directory.Replace('\\', '-')}.log";
robo_logfile.Dump();

string robo_cmd = $@"robocopy {source} {target} {robo_other_args} {robo_exclude_folders} {robo_exclude_files} {robo_logfile}";
robo_cmd.Dump();

