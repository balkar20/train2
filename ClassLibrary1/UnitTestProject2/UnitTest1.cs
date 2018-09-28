using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject2
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        //[ExpectedException(typeof(DivideByZeroException))]
        public void TestMethod1()
        {
            
            Assert.ThrowsException<DivideByZeroException>(() => { Us(); });
        }

        public void Us()
        {
            throw new DivideByZeroException();
        }
    }
}
