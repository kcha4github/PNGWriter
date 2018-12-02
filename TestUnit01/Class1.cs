using System;
using System.Collections.Generic;
using System.Text;

namespace TestUnit01
{
    using NUnit.Framework;

    [TestFixture]
    public class Class1
    {
        [Test]
        public void TestGetBigEndian32(){
            byte[] result = new byte[4]{0x12, 0x34, 0x56, 0x78};
            byte[] buf = new byte[4];
            PNGEncoder.PNGUtil.GetBigEndian32(buf, 0x12345678);
            Assert.AreEqual(result, buf);
        }

    }
}
