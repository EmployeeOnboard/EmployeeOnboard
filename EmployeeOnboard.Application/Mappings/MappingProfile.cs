using AutoMapper;
using EmployeeOnboard.Application.DTOs;
using EmployeeOnboard.Domain.Entities;
using EmployeeOnboard.Domain.Enums;

namespace EmployeeOnboard.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterEmployeeDTO, Employee>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(_ => Guid.NewGuid()))
            .ForMember(dest => dest.Password, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.Now))
            .ForMember(dest => dest.EmployeeNumber, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => EmployeeStatus.Active));

        CreateMap<Employee, EmployeeResponseDTO>()
            .ForMember(dest => dest.FullName, opt =>
                opt.MapFrom(src => string.Join(" ", new[] { src.FirstName, src.MiddleName, src.LastName }
                                                 .Where(name => !string.IsNullOrWhiteSpace(name)))))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
