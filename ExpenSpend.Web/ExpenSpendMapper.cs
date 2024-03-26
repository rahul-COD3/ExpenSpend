using AutoMapper;
using ExpenSpend.Domain.DTOs.Friends;
using ExpenSpend.Domain.DTOs.GroupMembers;
using ExpenSpend.Domain.DTOs.Groups;
using ExpenSpend.Domain.DTOs.Users;
using ExpenSpend.Domain.Models.Friends;
using ExpenSpend.Domain.Models.GroupMembers;
using ExpenSpend.Domain.Models.Groups;
using ExpenSpend.Domain.Models.Users;

namespace ExpenSpend.Web
{
    public class ExpenSpendMapper : Profile
    {
        public ExpenSpendMapper()
        {
            CreateMap<ApplicationUser, CreateUserDto>().ForMember(dest => dest.Password, opt => opt.Ignore()).ReverseMap();
            CreateMap<ApplicationUser, UpdateUserDto>().ReverseMap();
            CreateMap<ApplicationUser, GetUserDto>();
            CreateMap<Friendship, GetFriendshipDto>();
            CreateMap<CreateFriendshipDto, Friendship>();
            CreateMap<Group, GetGroupDto>();
            CreateMap<GroupMember, GetGroupMemberDto>();
        }
    }
}
