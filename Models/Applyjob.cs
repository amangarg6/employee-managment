using Employee_API_JWT_1035.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.Models
{
    public class Applyjob
    {
        public int Id { get; set; }
        public string? SenderId { get; set; } = string.Empty;
        [ForeignKey("SenderId")]
        public ApplicationUser? User1 { get; set; }
        public string? ReciverId { get; set; } = string.Empty;
        [ForeignKey("ReciverId")]
        public ApplicationUser? User2 { get; set; }
        public Status UserStatus { get; set; }
        public enum Status
        {
            Approved = 1,
            Reject = 2,
            Pending = 3
        };
        public Action UserActions { get; set; }
        public enum Action
        {
            Enable = 1,
            Disable = 2,
            Delete = 3
        };
    }
}
