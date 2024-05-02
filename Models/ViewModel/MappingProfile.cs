using AutoMapper;
using Employee_API_JWT_1035.Identity;
using Login_Register.DTO_s;



namespace Login_Register.Models.ViewModel
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Register,ApplicationUser>().ReverseMap();
            CreateMap<Employeedto,Employee>().ReverseMap();
            CreateMap<Designationdto, Designation>().ReverseMap();
            CreateMap<Companydto,Company>().ReverseMap();
        }
    }
}
