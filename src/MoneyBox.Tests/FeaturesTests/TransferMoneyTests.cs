using Moneybox.App;
using Moneybox.App.DataAccess;
using Moneybox.App.Domain.Services;
using Moneybox.App.Features;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;

namespace MoneyBox.Tests.FeaturesTests
{
    public class TransferMoneyTests
    {
        private Mock<IAccountRepository> _mockAccountRepository = new Mock<IAccountRepository>();
        private Mock<INotificationService> _mockNotificationService = new Mock<INotificationService>();
        private TransferMoney transferMoney;

        [SetUp]
        public void Setup()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            transferMoney = new TransferMoney(_mockAccountRepository.Object, _mockNotificationService.Object);
        }

        [Test]
        public void ShouldExecuteUpdateIfTransferSuccessful()
        {
            //ARRANGE
            var transferAmount = 50m;
            var fromAccountId = Guid.NewGuid();
            var fromUserAccount = new Account
            {
                Id = fromAccountId,
            };
            fromUserAccount.DepositFunds(1000);
            var toAccountId = Guid.NewGuid();
            var toUserAccount = new Account
            {
                Id = toAccountId
            };
            _mockAccountRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromUserAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toUserAccount);
            //ACT
            transferMoney.Execute(fromAccountId, toAccountId, transferAmount);

            //ASSERT
            _mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        }

        [Test]
        public void ShouldThrowExceptionIfInsufficientFundsInWithdrawalAccount()
        {
            //ARRANGE
            var transferAmount = 50m;
            var fromAccountId = Guid.NewGuid();
            var fromUserAccount = new Account
            {
                Id = fromAccountId,
            };
            fromUserAccount.DepositFunds(50);
            var toAccountId = Guid.NewGuid();
            var toUserAccount = new Account
            {
                Id = toAccountId,
            };
            _mockAccountRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromUserAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toUserAccount);
            //ACT
            Assert.Throws<InvalidOperationException>(() => transferMoney.Execute(fromAccountId, toAccountId, transferAmount));

            //ASSERT
            _mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Test]
        public void ShouldThrowExceptionIfPayInLimitReached()
        {
            //ARRANGE
            var transferAmount = 50m;
            var fromAccountId = Guid.NewGuid();
            var fromUserAccount = new Account
            {
                Id = fromAccountId,
            };
            fromUserAccount.DepositFunds(1000);
            var toAccountId = Guid.NewGuid();
            var toUserAccount = new Account
            {
                Id = toAccountId,
            };
            toUserAccount.DepositFunds(3960);
            _mockAccountRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromUserAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toUserAccount);
            //ACT
            Assert.Throws<InvalidOperationException>(() => transferMoney.Execute(fromAccountId, toAccountId, transferAmount));

            //ASSERT
            _mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Test]
        public void ShouldSendNotificationIfLowFundsLimitReached()
        {
            //ARRANGE
            var transferAmount = 50m;
            var fromAccountId = Guid.NewGuid();
            var fromUserAccount = new Account
            {
                Id = fromAccountId,
                User = new User
                {
                    Email = "test@email.com"
                }
            };
            fromUserAccount.DepositFunds(400);

            var toAccountId = Guid.NewGuid();
            var toUserAccount = new Account
            {
                Id = toAccountId
            };
            toUserAccount.DepositFunds(1000);
            _mockAccountRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromUserAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toUserAccount);
            //ACT
            transferMoney.Execute(fromAccountId, toAccountId, transferAmount);

            //ASSERT
            _mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
        }

        [Test]
        [TestCase(3550, 50)]
        [TestCase(3475, 50)]
        public void ShouldSendNotificationIfPayInWarningReached(decimal startingPaidInAmount, decimal transferAmount)
        {
            //ARRANGE
            
            var fromAccountId = Guid.NewGuid();
            var fromUserAccount = new Account
            {
                Id = fromAccountId,
            };
            fromUserAccount.DepositFunds(1000);
            var toAccountId = Guid.NewGuid();
            var toUserAccount = new Account
            {
                Id = toAccountId,
                User = new User
                {
                    Email = "test@email.com"
                }
            };
            toUserAccount.DepositFunds(startingPaidInAmount);
            _mockAccountRepository.Setup(x => x.GetAccountById(fromAccountId)).Returns(fromUserAccount);
            _mockAccountRepository.Setup(x => x.GetAccountById(toAccountId)).Returns(toUserAccount);
            //ACT
            transferMoney.Execute(fromAccountId, toAccountId, transferAmount);

            //ASSERT
            _mockNotificationService.Verify(x => x.NotifyApproachingPayInLimit(It.IsAny<string>()), Times.Once);
        }


    }
}