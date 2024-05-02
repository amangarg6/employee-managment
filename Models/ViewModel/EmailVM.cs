using System.ComponentModel.DataAnnotations.Schema;

namespace Login_Register.Models.ViewModel
{
    public class EmailVM
    {
        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? company { get; set; }
        public int? employeeId { get; set; }
        [ForeignKey("employeeId")]
        public Employee? employee { get; set; }                                                                                 
    }
}
