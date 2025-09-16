namespace Client
{
    public static class Settings
    {
        public static Environment.SpecialFolder SPECIALFOLDER = Environment.SpecialFolder.ApplicationData;
        public static string DIRECTORY = Environment.GetFolderPath(SPECIALFOLDER);
        public static string SUBDIRECTORY = "PBL4_HHB";
        public static string MUTEX = "PBL4_HHB";
        public static string LOGDIRECTORYNAME = "Logs";
        public static string LOGSPATH = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), LOGDIRECTORYNAME);
    }
}
