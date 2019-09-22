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
            var account = new Account();
            account.DepositFunds(balance);
            //ACT
            bool result = account.HasSufficientBalance(withdrawalAmount);

            //ASSERT
            result.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(600, false)]
        [TestCase(100, true)]
        public void HasLowFundsShouldReturnCorrectStatus(decimal balance, bool expectedResult)
        {
            //ARRANGE
            var account = new Account();
            account.DepositFunds(balance);
            //ACT
            bool result = account.HasLowFunds();

            //ASSERT
            result.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(100, 10, true)]
        [TestCase(100, 25, true)]
        [TestCase(4000, 100, false)]
        [TestCase(3800, 250, false)]
        public void HasSufficientPayInCapacityShouldReturnCorrectStatus(decimal paidInAmount, decimal depositAmount, bool expectedResult)
        {
            //ARRANGE
            var account = new Account();
            account.DepositFunds(paidInAmount);           
            
            //ACT
            bool result = account.HasSufficientPayInCapacity(depositAmount);

            //ASSERT
            result.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(100, 10, false)]
        [TestCase(100, 25, false)]
        [TestCase(3550, 100, true)]
        [TestCase(3400, 250, true)]
        public void IsNearPayInLimitShouldReturnCorrectStatus(decimal paidInAmount, decimal depositAmount, bool expectedResult)
        {
            //ARRANGE
            var account = new Account();
            account.DepositFunds(paidInAmount);
            //ACT
            bool result = account.IsNearPayInLimit(depositAmount);

            //ASSERT
            result.ShouldBe(expectedResult);
        }

        [Test]
        [TestCase(150, 20, 130, -20)]
        [TestCase(0, 20, -20, -20)]
        public void WithdrawFundsShouldUpdateCorrectly(decimal startingBalance, decimal withdrawalAmount, decimal expectedBalance, decimal expectedWithdrawn)
        {
            //ARRANGE
            var account = new Account();
            account.DepositFunds(startingBalance);
            //ACT
            account.WithdrawFunds(withdrawalAmount);

            //ASSERT
            account.GetBalance().ShouldBe(expectedBalance);
            account.GetWithdrawn().ShouldBe(expectedWithdrawn);
        }

        [Test]
        [TestCase(150, 150, 150)]
        [TestCase(20, 20, 20)]
        public void DepositFundsShouldUpdateCorrectly(decimal depositAmount, decimal expectedBalance, decimal expectedPaidIn)
        {
            //ARRANGE
            var account = new Account();
            //ACT
            account.DepositFunds(depositAmount);

            //ASSERT
            account.GetBalance().ShouldBe(expectedBalance);
            account.GetPaidIn().ShouldBe(expectedPaidIn);
        }
    }
}
