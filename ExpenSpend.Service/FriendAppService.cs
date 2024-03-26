using AutoMapper;
using ExpenSpend.Domain.DTOs.Friends.Enums;
using ExpenSpend.Data.Context;
using ExpenSpend.Domain;
using ExpenSpend.Domain.Models.Friends;
using ExpenSpend.Repository.Contracts;
using ExpenSpend.Service.Contracts;
using ExpenSpend.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ExpenSpend.Domain.DTOs.Friends;

namespace ExpenSpend.Service
{
    public class FriendAppService : IFriendAppService
    {
        private readonly IRepository<Friendship> _friendRepository;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public FriendAppService(
            IRepository<Friendship> friendRepository, 
            ApplicationDbContext context, IMapper mapper,
            IHttpContextAccessor httpContext
        ){
            _friendRepository = friendRepository;
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<Response> GetAllFriendsAsync()
        {
            var friends = await _friendRepository.GetAllAsync();
            return new Response(_mapper.Map<GetFriendshipDto>(friends));
        }
        public async Task<Response> GetFriendshipRequestsAsync()
        {
            var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
            var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);
            var friendRequests = await _context.Friendships
            .Where(f => f.RecipientId == currUser!.Id && f.Status == FriendshipStatus.Pending)
            .Include(f => f.Initiator)
            .Include(f => f.Recipient)
            .ToListAsync();
            if (friendRequests.Count()==0)
            {
                return new Response("No friend requests pending.");
            }
            return new Response(_mapper.Map<List<GetFriendshipDto>>(friendRequests));
        }
        public async Task<Response> GetFriendshipsAsync()
        {
            var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
            var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);
            var friends = await _context.Friendships
            .Where(f => (f.InitiatorId == currUser!.Id || f.RecipientId == currUser.Id) && f.Status == FriendshipStatus.Accepted)
            .Include(f => f.Initiator)
            .Include(f => f.Recipient)
            .ToListAsync();
            if (friends.Count()==0)
            {
                return new Response("No friends found.");
            }
            return new Response(_mapper.Map<List<GetFriendshipDto>>(friends));
        }
        public async Task<Response> GetFriendByIdAsync(Guid id)
        {
            var friend = await _friendRepository.GetByIdAsync(id);
            if (friend == null)
            {
                return new Response("Friend not found");
            }
            return new Response(_mapper.Map<GetFriendshipDto>(friend));
        }
        public async Task<Response> CreateFriendAsync(Guid recipientId)
        {
            var currentUser = _httpContext.HttpContext?.User?.Identity?.Name;
            var currUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == currentUser);

            var friendshipExists = await _context.Friendships.FirstOrDefaultAsync(f => f.InitiatorId == currUser!.Id && f.RecipientId == recipientId || f.InitiatorId == recipientId && f.RecipientId == currUser.Id);
            if (friendshipExists != null)
            {
                return new Response("Friendship already exists.");
            }

            var friend = new Friendship(currUser!.Id, recipientId, FriendshipStatus.Pending, DateTime.Now, currUser.Id);
            await _friendRepository.InsertAsync(friend);

            return new Response(_mapper.Map<GetFriendshipDto>(friend));
        }
        public async Task<Response> UpdateFriendAsync(Friendship friendship)
        {
            var friendToUpdate = await _friendRepository.GetByIdAsync(friendship.Id);
            if (friendToUpdate == null)
            {
                return new Response("Friend not found");
            }
            var updatedFriend = _mapper.Map(friendship, friendToUpdate);
            await _friendRepository.UpdateAsync(updatedFriend);
            return new Response(updatedFriend);
        }
        public async Task<Response> SoftDeleteFriendAsync(Guid Id)
        {
            var friend = await _friendRepository.GetByIdAsync(Id);
            if (friend == null)
            {
                return new Response("Friend not found");
            }
            friend.IsDeleted = true;
            await _friendRepository.UpdateAsync(friend);
            return new Response(_mapper.Map<GetFriendshipDto>(friend));
        }
        public async Task<Response> DeleteFriendAsync(Guid id)
        {
            var friend = await _friendRepository.GetByIdAsync(id);
            if (friend == null)
            {
                return new Response("Friend not found");
            }
            await _friendRepository.DeleteAsync(friend);
            return new Response("Friend deleted successfully");
        }
        public async Task<Response> AcceptAsync(Guid friendshipId)
        {
            var friendship = await _friendRepository.GetByIdAsync(friendshipId);
            if (friendship == null)
            {
                return new Response("Friendship not found");
            }
            friendship.Status = FriendshipStatus.Accepted;
            await _friendRepository.UpdateAsync(friendship);
            return new Response(_mapper.Map<GetFriendshipDto>(friendship));
        }
        public async Task<Response> DeclineAsync(Guid friendshipId)
        {
            var friendship = await _friendRepository.GetByIdAsync(friendshipId);
            if (friendship == null)
            {
                return new Response("Friendship not found");
            }

            friendship.Status = FriendshipStatus.Declined;
            await _friendRepository.UpdateAsync(friendship);
            return new Response(_mapper.Map<GetFriendshipDto>(friendship));
        }
        public async Task<Response> BlockAsync(Guid friendshipId)
        {
            var friendship = await _friendRepository.GetByIdAsync(friendshipId);
            if (friendship == null)
            {
                return new Response("Friendship not found");
            }
            friendship.Status = FriendshipStatus.Blocked;
            await _friendRepository.UpdateAsync(friendship);
            return new Response(_mapper.Map<GetFriendshipDto>(friendship));
        }
        public async Task<Response> UnBlockAsync(Guid friendshipId)
        {
            var friendship = await _friendRepository.GetByIdAsync(friendshipId);
            if (friendship == null)
            {
                return new Response("Friendship not found");
            }
            friendship.Status = FriendshipStatus.Accepted;
            await _friendRepository.UpdateAsync(friendship);
            return new Response(_mapper.Map<GetFriendshipDto>(friendship));
        }
    }
}
