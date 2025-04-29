using AutoMapper;
using WebApiStudents.Models;
using WebApiStudents.Models.StudentDTO_s;

namespace WebApiStudents.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Student, StudentDto>()
            .ForMember(dest => dest.Facultet, opt => opt.MapFrom(src => src.Facultet))
            .ReverseMap();
        CreateMap<Student, CreateStudentDto>();
        CreateMap<Student, StudentSlimDto>();
        CreateMap<Student, StudentWithFacultetNameDto>();

        CreateMap<Facultet, FacultetDto>();
    }
}
