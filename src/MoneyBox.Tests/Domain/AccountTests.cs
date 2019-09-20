using Moneybox.App;
using NUnit.Framework;
using Shouldly;

namespace MoneyBox.Tests.Domain
{
    public class AccountTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(100, 10, true)]
        [TestCase(100, 25, true)]
        [TestCase(5, 10, false)]
        [TestCase(15, 25, false)]
        public void HasSufficientBalanceShouldReturnCorrectStatus(decimal balance, decimal withdrawalAmount, bool expectedResult)
        {
            //ARRANGE
            var account = new Account
            {
                Balance = balance
            };
            //ACT
            bool result = account.HasSufficientBalance(withdrawalAmount);

            //ASSERT
            expectedResult.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(600, true)]
        [TestCase(100, false)]
        public void HasLowFundsShouldReturnCorrectStatus(decimal balance, bool expectedResult)
        {
            //ARRANGE
            var account = new Account
            {
                Balance = balance
            };
            //ACT
            bool result = account.HasLowFunds();

            //ASSERT
            expectedResult.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(150, 0, 20, 130, -20)]
        [TestCase(150, 50, 20, 130, 30)]
        [TestCase(0, 0, 20, -20, -20)]
        [TestCase(0, 50, 20, -20, 30)]
        public void WithdrawFundsShouldUpdateCorrectly(decimal startingBalance, decimal startingWithdrawn, decimal withdrawalAmount, decimal expectedBalance, decimal expectedWithdrawn)
        {
            //ARRANGE
            var account = new Account
            {
                Balance = startingBalance,
                Withdrawn = startingWithdrawn
            };
            //ACT
            account.WithdrawFunds(withdrawalAmount);

            //ASSERT
            account.Balance.ShouldBe(expectedBalance);
            account.Withdrawn.ShouldBe(expectedWithdrawn);
        }

        [Test]
        [TestCase(150, 0, 20, 170, 20)]
        [TestCase(150, 50, 20, 170, 70)]
        [TestCase(0, 0, 20, 20, 20)]
        [TestCase(0, 50, 20, 20, 70)]
        public void DepositFundsShouldUpdateCorrectly(decimal startingBalance, decimal startingPaidIn, decimal depositAmount, decimal expectedBalance, decimal expectedPaidIn)
        {
            //ARRANGE
            var account = new Account
            {
                Balance = startingBalance,
                PaidIn = startingPaidIn
            };
            //ACT
            account.DepositFunds(depositAmount);

            //ASSERT
            account.Balance.ShouldBe(expectedBalance);
            account.PaidIn.ShouldBe(expectedPaidIn);
        }
    }
}
