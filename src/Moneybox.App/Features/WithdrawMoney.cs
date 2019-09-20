using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class WithdrawMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public WithdrawMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, decimal amount)
        {
            var withdrawalAccount = this.accountRepository.GetAccountById(fromAccountId);
            
            if (!withdrawalAccount.HasSufficientBalance(amount))
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (withdrawalAccount.HasLowFunds())
            {
                this.notificationService.NotifyFundsLow(withdrawalAccount.User.Email);
            }

            withdrawalAccount.WithdrawFunds(amount);

            this.accountRepository.Update(withdrawalAccount);
            
        }
    }
}
