using System;
using System.Linq;
using System.Net.Mail;
using System.Security.Permissions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Assert = NUnit.Framework.Assert;
using Moq;
using ClassLibrary4;


namespace UnitTestProject1
{
    [TestFixture]
    public class UnitTest2
    {
        [Test]
        //[ExpectedException(typeof(Exception), "My ex")]
        public void TestMethod1()
        {
            var mock = new Mock<ILastUsernameProvider>(MockBehavior.Default);

            mock.Setup(lp => lp.ReadLastUserName()).Returns("Balkarov");
            mock.Setup(lp => lp.SaveLastUserName(It.IsAny<string>()));

            ILastUsernameProvider lastUsernameProvider = mock.Object;

            
            Assert.AreEqual(lastUsernameProvider.ReadLastUserName(), "Balkarov");
            var ex = Assert.Throws<Exception>(() => { Console.WriteLine("Hi man");});
            //lastUsernameProvider.SaveLastUserName("lll");
            Assert.That(ex.Message == "Hi man");
            mock.Verify();
        }

        [Test]
        [ExpectedException(typeof(DivideByZeroException))]
        public void DivideTest()
        {
            //int numerator = 4;
            //int denominator = 0;
            //int actual = numerator / denominator;
            
            throw new DivideByZeroException();
        }
    }
}
