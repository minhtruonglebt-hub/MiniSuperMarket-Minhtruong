namespace MiniSuperMarket.API.Models
{
    // Thực thể Vai trò nhân viên (bài tập mở rộng Buổi 1 - dữ liệu In-Memory)
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
