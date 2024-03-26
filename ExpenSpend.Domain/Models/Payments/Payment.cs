using ExpenSpend.Domain.Models.Expenses;
using ExpenSpend.Domain.Models.Users;

namespace ExpenSpend.Domain.Models.Payments
{
    public class Payment : BaseEntity
    {
        public Guid OwenedById { get; set; }
        public ApplicationUser? OwenedBy { get; set; }
        public Guid ExpenseId { get; set; }
        public Expense? Expense { get; set; }
        public double Amount { get; set; }
        public bool IsSettled { get; set; }

    }
}
