namespace Common.Enums
{
    public enum UserStatus : byte
    {
        Active, //đang hoạt động (ví dụ đang dùng ứng dụng, di chuyển chuột, gõ bàn phím…)
        Idle //nhàn rỗi / không hoạt động (ví dụ không có tương tác với ứng dụng trong một khoảng thời gian)
    }
}
