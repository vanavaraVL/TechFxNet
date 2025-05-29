using AutoMapper;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Application.Mappers;

public class JournalMapperProfile : Profile
{
    public JournalMapperProfile()
    {
        CreateMap<JournalEntity, JournalDto>();
        CreateMap<JournalEntity, JournalInfoDto>();
    }
}