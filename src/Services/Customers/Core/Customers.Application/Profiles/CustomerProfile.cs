using AutoMapper;
using Customers.Application.DTOs;
using Customers.Domain.Entities;

namespace Customers.Application.Profiles;

public class CustomerProfile : Profile
{
    public CustomerProfile()
    {
        CreateMap<AddCustomerDTO, Customer>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}