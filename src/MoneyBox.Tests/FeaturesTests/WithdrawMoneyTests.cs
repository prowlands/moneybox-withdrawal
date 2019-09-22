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
    public class WithdrawMoneyTests
    {
        private Mock<IAccountRepository> _mockAccountRepository = new Mock<IAccountRepository>();
        private Mock<INotificationService> _mockNotificationService = new Mock<INotificationService>();
        private WithdrawMoney withdrawMoney;

        [SetUp]
        public void Setup()
        {
            _mockAccountRepository = new Mock<IAccountRepository>();
            _mockNotificationService = new Mock<INotificationService>();
            withdrawMoney = new WithdrawMoney(_mockAccountRepository.Object, _mockNotificationService.Object);
        }


        [Test]
        public void ShouldExecuteUpdateIfWithdrawalSuccessful()
        {
            //ARRANGE
            var withdrawalAmount = 50m;
            var accountId = Guid.NewGuid();
            var userAcount = new Account
            {
                Id = accountId
            };
            userAcount.DepositFunds(500);
            _mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(userAcount);
            //ACT
            withdrawMoney.Execute(accountId, withdrawalAmount);

            //ASSERT
            _mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Once);
        }

        [Test]
        public void ShouldThrowExceptionIfInsufficientFunds()
        {
            //ARRANGE
            var withdrawalAmount = 50m;
            var accountId = Guid.NewGuid();
            var userAcount = new Account
            {
                Id = accountId
            };
            userAcount.DepositFunds(25);
            _mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(userAcount);
            //ACT            
            Assert.Throws<InvalidOperationException>(() => withdrawMoney.Execute(accountId, withdrawalAmount));

            //ASSERT
            _mockAccountRepository.Verify(x => x.Update(It.IsAny<Account>()), Times.Never);
        }

        [Test]
        public void ShouldSendNotificationIfLowFundsLimitReached()
        {
            //ARRANGE
            var withdrawalAmount = 50m;
            var accountId = Guid.NewGuid();
            var userAcount = new Account
            {
                Id = accountId,
                User = new User
                {
                    Email = "test@email.com"
                }
            };
            userAcount.DepositFunds(450);
            _mockAccountRepository.Setup(x => x.GetAccountById(It.IsAny<Guid>())).Returns(userAcount);
            //ACT
            withdrawMoney.Execute(accountId, withdrawalAmount);

            //ASSERT
            _mockNotificationService.Verify(x => x.NotifyFundsLow(It.IsAny<string>()), Times.Once);
        }
    }
}