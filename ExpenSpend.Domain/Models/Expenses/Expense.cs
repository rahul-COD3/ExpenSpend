using ExpenSpend.Domain.Models.Groups;
using ExpenSpend.Domain.Models.Payments;
using ExpenSpend.Domain.Models.Users;

namespace ExpenSpend.Domain.Models.Expenses;

public class Expense : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }
    public Guid PaidById { get; set; }
    public ApplicationUser? PaidBy { get; set; }
    public double Amount { get; set; }
    public SplitAs SplitAs { get; set; } = SplitAs.Equally;
    public bool IsSettled { get; set; }
    public List<Payment>? Payments { get; set; }
}
