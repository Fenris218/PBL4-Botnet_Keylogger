namespace Client.Extensions
{
    public static class DriveTypeExtensions
    {
        public static string ToFriendlyString(this DriveType type)
        {
            switch (type)
            {
                // ổ cố định
                case DriveType.Fixed:
                    return "Local Disk";
                //một thư mục/ổ đĩa được chia sẻ từ máy khác trong mạng LAN hoặc từ server.
                case DriveType.Network:
                    return "Network Drive";
                //ổ đĩa di động có thể tháo rời: USB, thẻ nhớ SD
                case DriveType.Removable:
                    return "Removable Drive";
                default:
                    return type.ToString();
            }
        }
    }
}
