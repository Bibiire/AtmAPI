using Application.DTOs.Transaction;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class DtoToDomainProfile : Profile
    {
        public DtoToDomainProfile()
        {

            CreateMap<IntraTransferRequestDTO, Transaction>();
        }
    }
}
