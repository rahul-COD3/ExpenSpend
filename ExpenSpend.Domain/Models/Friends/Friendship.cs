using ExpenSpend.Domain.DTOs.Friends.Enums;
using ExpenSpend.Domain.Models.Users;

namespace ExpenSpend.Domain.Models.Friends;

public class Friendship : BaseEntity
{
    public Guid InitiatorId { get; set; }
    public ApplicationUser? Initiator { get; set; }
    public Guid RecipientId { get; set; }
    public ApplicationUser? Recipient { get; set; }
    public FriendshipStatus Status { get; set; } = FriendshipStatus.Pending;

    public Friendship(Guid initiatorId, Guid recipientId, FriendshipStatus status, DateTime createdAt, Guid? createdBy = null, Guid? modifiedBy = null, bool isDeleted = false) : base()
    {
        InitiatorId = initiatorId;
        RecipientId = recipientId;
        Status = status;
        CreatedAt = createdAt; 
        CreatedBy = createdBy;
        ModifiedBy = modifiedBy;
        IsDeleted = isDeleted;
    }
}