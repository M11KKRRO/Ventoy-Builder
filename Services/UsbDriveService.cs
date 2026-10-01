using System;
using System.Collections.Generic;
using System.IO;
using Ventoy_Builder.Models;

namespace Ventoy_Builder.Services
{
    public class UsbDriveService
    {
        public List<UsbDriveInfo> GetUsbDrives()
        {
            List<UsbDriveInfo> drives = new();
            
            // Get the root of the operating system drive (e.g., "C:\") to make sure we don't include it
            string systemDrive = Path.GetPathRoot(Environment.SystemDirectory);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                try
                {
                    // 1. Verify the drive is spun up and ready to be read
                    if (!drive.IsReady) continue;

                    // 2. Skip the internal main system drive entirely for user safety
                    if (string.Equals(drive.Name, systemDrive, StringComparison.OrdinalIgnoreCase)) continue;

                    // 3. Catch traditional USB flash drives (Removable) AND external hard drives (Fixed)
                    if (drive.DriveType == DriveType.Removable || drive.DriveType == DriveType.Fixed)
                    {
                        // Calculate display values safely
                        string sizeText = $"{drive.TotalSize / (1024 * 1024 * 1024)} GB";
                        string freeSpaceText = $"{drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB";

                        drives.Add(new UsbDriveInfo
                        {
                            DriveLetter = drive.Name,
                            Label = string.IsNullOrEmpty(drive.VolumeLabel) ? "Local Disk" : drive.VolumeLabel,
                            SizeText = sizeText,
                            FreeSpaceText = freeSpaceText,
                            FileSystem = drive.DriveFormat,
                            FullPath = drive.RootDirectory.FullName,
                            TotalSizeBytes = drive.TotalSize,
                            FreeSpaceBytes = drive.AvailableFreeSpace,
                            DriveType = drive.DriveType,
                            IsRemovable = (drive.DriveType == DriveType.Removable),
                            IsReady = drive.IsReady
                        });
                    }
                }
                catch (Exception)
                {
                    // Skip any drives that throw an error during access (unformatted, locked, etc.)
                    continue;
                }
            }

            return drives;
        }
    }
}
