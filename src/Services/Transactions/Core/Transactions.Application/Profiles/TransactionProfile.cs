using AutoMapper;
using Transactions.Application.DTOs;
using Transactions.Domain.Entities;

namespace Transactions.Application.Profiles;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<AddTransactionDTO, Transaction>()
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
    }
}