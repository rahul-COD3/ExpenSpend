using AutoMapper;
using ExpenSpend.Domain.DTOs.GroupMembers;
using ExpenSpend.Data.Context;
using ExpenSpend.Domain.Models.GroupMembers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ExpenSpend.Repository.Contracts;
using ExpenSpend.Service.Contracts;
using ExpenSpend.Service.Models;

namespace ExpenSpend.Service
{
    public class GroupMemberAppService : IGroupMemberAppService
    {
        private readonly IRepository<GroupMember> _groupMemberRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public GroupMemberAppService(IRepository<GroupMember> groupMemberRepository, ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _groupMemberRepository = groupMemberRepository;
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<Response> GetAllGroupMembersAsync()
        {
            var groupMembers = await _groupMemberRepository.GetAllAsync();
            if (groupMembers == null)
            {
                return new Response("No group members found.");
            }
            return new Response(_mapper.Map<List<GetGroupMemberDto>>(groupMembers));
        }

        public async Task<Response> GetGroupMemberByIdAsync(Guid id)
        {
            var groupMember = await _groupMemberRepository.GetByIdAsync(id);
            if (groupMember == null)
            {
                return new Response("Group member not found.");
            }
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));

        }

        public async Task<Response> CreateGroupMemberAsync(CreateGroupMemberDto input)
        {
            var checkIfGroupMemberExists = await _context.GroupMembers.FirstOrDefaultAsync(x => x.GroupId == input.GroupId && x.UserId == input.UserId);
            if (checkIfGroupMemberExists != null)
            {
                return new Response("Group member already exists.");
            }
            var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
            var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);

            var groupMember = _mapper.Map<GroupMember>(input);
            groupMember.CreatedAt = DateTime.Now;
            groupMember.CreatedBy = input.UserId;
            await _groupMemberRepository.InsertAsync(groupMember);
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));
        }

        public async Task<Response> CreateGroupMembersAsync(List<CreateGroupMemberDto> input)
        {
            var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
            var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);

            var groupMembers = _mapper.Map<List<GroupMember>>(input);

            foreach (var member in groupMembers)
            {
                member.CreatedAt = DateTime.Now;
                member.CreatedBy = currUser?.Id;
            }

            await _context.GroupMembers.AddRangeAsync(groupMembers);
            var createdGroupMembers = await _context.SaveChangesAsync();
            if (createdGroupMembers == 0)
            {
                return new Response("Bad Request");
            }
            return new Response(_mapper.Map<List<GetGroupMemberDto>>(groupMembers));
        }

        public async Task<Response> SoftDeleteGroupMemberAsync(Guid id)
        {
            var groupMember = await _groupMemberRepository.GetByIdAsync(id);
            groupMember.IsDeleted = true;
            await _groupMemberRepository.UpdateAsync(groupMember);
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));
        }

        public async Task<Response> DeleteGroupMemberAsync(Guid id)
        {
            var groupMember = await _groupMemberRepository.GetByIdAsync(id);
            await _groupMemberRepository.DeleteAsync(groupMember);
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));
        }

        public async Task<Response> MakeGroupAdminAsync(Guid id)
        {
            var groupMember = await _groupMemberRepository.GetByIdAsync(id);
            if (groupMember == null)
            {
                return new Response("Group member not found");
            }
            groupMember.IsAdmin = true;
            await _groupMemberRepository.UpdateAsync(groupMember);
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));
        }

        public async Task<Response> RemoveGroupAdminAsync(Guid id)
        {
            var groupMember = await _groupMemberRepository.GetByIdAsync(id);
            if (groupMember == null)
            {
                return new Response("Group member not found");
            }
            groupMember.IsAdmin = false;
            await _groupMemberRepository.UpdateAsync(groupMember);
            return new Response(_mapper.Map<GetGroupMemberDto>(groupMember));
        }
    }
}
