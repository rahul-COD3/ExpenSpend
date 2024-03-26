using ExpenSpend.Domain.Models.Groups;
using ExpenSpend.Domain.Models.Users;

namespace ExpenSpend.Domain.Models.GroupMembers
{
    public class GroupMember : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Group? Group { get; set; }
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; }
        public bool IsAdmin { get; set; }
    }
}
