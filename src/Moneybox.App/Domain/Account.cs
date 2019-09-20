using System;

namespace Moneybox.App
{
    public class Account
    {
        public const decimal PayInLimit = 4000m;

        private const decimal PayInLimitThreshold = 500m;

        private const decimal LowFundsThreshold = 500m;

        public Guid Id { get; set; }

        public User User { get; set; }

        public decimal Balance { get; set; }

        public decimal Withdrawn { get; set; }

        public decimal PaidIn { get; set; }

        public bool HasSufficientBalance(decimal withdrawalAmount)
        {
            return Balance > withdrawalAmount;
        }

        public bool HasLowFunds()
        {
            return Balance < LowFundsThreshold;
        }

        public bool HasSufficientPayInCapacity(decimal depositAmount)
        {
            return PaidIn + depositAmount < PayInLimit;
        }

        public bool IsNearPayInLimit(decimal depositAmount)
        {
            return PaidIn + depositAmount > PayInLimit - PayInLimitThreshold;
        }

        public void WithdrawFunds(decimal amount)
        {
            Balance = Balance - amount;
            Withdrawn = Withdrawn - amount;
        }

        public void DepositFunds(decimal amount)
        {
            Balance = Balance + amount;
            PaidIn = PaidIn + amount;
        }
    }
}
