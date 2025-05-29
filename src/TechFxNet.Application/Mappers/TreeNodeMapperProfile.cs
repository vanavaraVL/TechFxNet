using AutoMapper;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Entities;

namespace TechFxNet.Application.Mappers;

public class TreeNodeMapperProfile: Profile
{
    public TreeNodeMapperProfile()
    {
        CreateMap<TreeEntity, TreeNodeDto>()
            .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.TreeName))
            .ForMember(dst => dst.Children, opt => opt.Ignore());
    }
}