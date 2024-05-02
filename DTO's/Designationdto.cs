using Login_Register.Models;

namespace Login_Register.DTO_s
{
    public class Designationdto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? EmployeeId { get; set; }
        public Employee? employee { get; set; }

    }
}
