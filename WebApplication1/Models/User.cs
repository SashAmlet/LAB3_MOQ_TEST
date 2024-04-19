namespace WebApplication1.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FName { get; set; } = null!;
        public string? LName { get; set; }
        public string Email { get; set; } = null!;
    }
}
