using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace TestCoreSuite
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void SimpleTest()
        {
            Assert.AreEqual(1,1);
        }
    }
}
