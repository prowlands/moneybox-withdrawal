using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using System;

namespace Moneybox.App.Features
{
    public class TransferMoney
    {
        private IAccountRepository accountRepository;
        private INotificationService notificationService;

        public TransferMoney(IAccountRepository accountRepository, INotificationService notificationService)
        {
            this.accountRepository = accountRepository;
            this.notificationService = notificationService;
        }

        public void Execute(Guid fromAccountId, Guid toAccountId, decimal amount)
        {
            var fromAccount = this.accountRepository.GetAccountById(fromAccountId);
            var toAccount = this.accountRepository.GetAccountById(toAccountId);

            if (!fromAccount.HasSufficientBalance(amount))
            {
                throw new InvalidOperationException("Insufficient funds to make transfer");
            }

            if (fromAccount.HasLowFunds())
            {
                this.notificationService.NotifyFundsLow(fromAccount.User.Email);
            }

            
            if (!toAccount.HasSufficientPayInCapacity(amount))
            {
                throw new InvalidOperationException("Account pay in limit reached");
            }

            if (toAccount.IsNearPayInLimit(amount))
            {
                this.notificationService.NotifyApproachingPayInLimit(toAccount.User.Email);
            }

            fromAccount.WithdrawFunds(amount);
            toAccount.DepositFunds(amount);

            this.accountRepository.Update(fromAccount);
            this.accountRepository.Update(toAccount);
        }
    }
}
