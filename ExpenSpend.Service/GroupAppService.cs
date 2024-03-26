using AutoMapper;
using ExpenSpend.Domain.DTOs.Groups;
using ExpenSpend.Data.Context;
using ExpenSpend.Domain.Models.GroupMembers;
using ExpenSpend.Domain.Models.Groups;
using ExpenSpend.Repository.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ExpenSpend.Service.Models;
using ExpenSpend.Service.Contracts;

namespace ExpenSpend.Service;

public class GroupAppService : IGroupAppService
{
    private readonly IRepository<Group> _groupRepository;
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContext;

    public GroupAppService(IRepository<Group> groupRepository, ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
    {
        _groupRepository = groupRepository;
        _context = context;
        _mapper = mapper;
        _httpContext = httpContextAccessor;
    }

    public async Task<Response> GetAllGroupsAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        return new Response(_mapper.Map<List<GetGroupDto>>(groups));
    }
    public async Task<Response> GetGroupByIdAsync(Guid id)
    {
        var group = await _context.Groups.Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == id);
        if (group != null)
        {
            return new Response(_mapper.Map<GetGroupDto>(group));
        }
        return null;
    }
    public async Task<Response> CreateGroupAsync(CreateGroupDto input)
    {
        var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
        var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);
        var group = new Group
        {
            Name = input.Name,
            About = input.About,
            CreatedBy = currUser?.Id,
            CreatedAt = DateTime.Now,
        };

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                await _groupRepository.InsertAsync(group);
                var groupMember = new GroupMember
                {
                    GroupId = group.Id,
                    UserId = currUser!.Id,
                    IsAdmin = true,
                    CreatedAt = DateTime.Now,
                    CreatedBy = currUser.Id
                };
                _context.GroupMembers.Add(groupMember);
                await _context.SaveChangesAsync();
                transaction.Commit();
                return new Response(_mapper.Map<GetGroupDto>(group));
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    public async Task<Response> CreateGroupWithMembers(CreateGroupWithMembersDto input)
    {
        var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
        var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);
        var group = new Group
        {
            Name = input.Name,
            About = input.About,
            CreatedBy = currUser?.Id,
            CreatedAt = DateTime.Now,
        };
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                await _groupRepository.InsertAsync(group);
                var groupMembers = new List<GroupMember>();
                foreach (var memberId in input.MemberIds)
                {
                    groupMembers.Add(new GroupMember
                    {
                        GroupId = group.Id,
                        UserId = memberId
                    });
                }
                _context.GroupMembers.AddRange(groupMembers);
                await _context.SaveChangesAsync();
                return new Response(_mapper.Map<GetGroupDto>(group));
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }
    }
    public async Task<Response> UpdateGroupAsync(Guid id, UpdateGroupDto group)
    {
        var existingGroup = await _groupRepository.GetByIdAsync(id);
        if (existingGroup == null)
        {
            return new Response("Group not found");
        }

        var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
        var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);
        existingGroup.Name = group.Name;
        existingGroup.About = group.About;
        existingGroup.ModifiedAt = DateTime.Now;
        existingGroup.ModifiedBy = currUser!.Id;

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                await _groupRepository.UpdateAsync(existingGroup);
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                return new Response("An error occurred while updating the group");
            }
        }
        return new Response(_mapper.Map<GetGroupDto>(existingGroup));
    }
    public async Task<Response> SoftDeleteAsync(Guid id)
    {
        var existingGroup = await _groupRepository.GetByIdAsync(id);
        if (existingGroup == null)
        {
            new Response("Group not found");
        }
        existingGroup!.IsDeleted= true;
        await _groupRepository.UpdateAsync(existingGroup);
        return new Response(_mapper.Map<GetGroupDto>(existingGroup));
    }
    public async Task<Response> DeleteGroupAsync(Guid id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        if (group == null)
        {
            return new Response("Group not found");
        }
        await _groupRepository.DeleteAsync(group);
        return new Response(_mapper.Map<GetGroupDto>(group));
    }
    public async Task<Response> GetGroupsByUserId(Guid userId)
    {
        var groups = await _context.Groups.Where(x => x.Members!.Any(x => x.UserId == userId)).ToListAsync();
        return new Response(_mapper.Map<List<GetGroupDto>>(groups));
    }
}
