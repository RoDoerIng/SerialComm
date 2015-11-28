namespace SerialComm.Model
{
    public class FileExportSettingsModel : SingletonBase<FileExportSettingsModel>
    {
        private FileExportSettingsModel()
        {
        }

        public string[] getFileExtensions = { ".TXT", ".CSV" };
    }
}
