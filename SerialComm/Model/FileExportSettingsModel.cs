using System;
namespace SerialComm.Model
{
    [Serializable]
    public class FileExportSettingsModel : SingletonBase<FileExportSettingsModel>
    {
        public FileExportSettingsModel()
        {

        }

        public string[] getFileExtensions = { ".TXT", ".CSV" };
    }
}
