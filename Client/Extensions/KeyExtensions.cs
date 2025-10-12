namespace Client.Extensions
{
    public static class KeyExtensions
    {
        public static bool ContainsModifierKeys(this List<Keys> pressedKeys)
        {
            return pressedKeys.Any(x => x.IsModifierKey());
        }

        //Kiểm tra có phải phím phụ không ? 
        //Nhưng tại sao lại ko có shift ?
        public static bool IsModifierKey(this Keys key)
        {
            return (key == Keys.LControlKey
                    || key == Keys.RControlKey
                    || key == Keys.LMenu
                    || key == Keys.RMenu
                    || key == Keys.LWin
                    || key == Keys.RWin
                    || key == Keys.Control
                    || key == Keys.Alt);
        }
        //trong System.Windows.Forms.Keys, các phím chữ được định nghĩa là in hoa-> cần Upper
        //kiểm tra xem trong danh sách các phím đang được nhấn (pressedKeys) có phím tương ứng với ký tự đó không.
        public static bool ContainsKeyChar(this List<Keys> pressedKeys, char c)
        {
            return pressedKeys.Contains((Keys)char.ToUpper(c));
        }


        //Kiểm tra có phải các phím giữa các phím nhập liệu (text).
        public static bool IsExcludedKey(this Keys k)
        {
            return (k >= Keys.A && k <= Keys.Z
                    || k >= Keys.NumPad0 && k <= Keys.Divide
                    || k >= Keys.D0 && k <= Keys.D9
                    || k >= Keys.Oem1 && k <= Keys.OemClear
                    || k >= Keys.LShiftKey && k <= Keys.RShiftKey
                    || k == Keys.CapsLock
                    || k == Keys.Space);
        }

        public static string GetDisplayName(this Keys key)
        {
            string name = key.ToString();
            if (name.Contains("ControlKey"))
                return "Control";
            else if (name.Contains("Menu"))
                return "Alt";
            else if (name.Contains("Win"))
                return "Win";
            else if (name.Contains("Shift"))
                return "Shift";
            return name;
        }
    }
}
