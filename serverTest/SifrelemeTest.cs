using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using serverTest;

namespace clientTest
{
    [TestFixture]
    public  class SifrelemeTest
    {
        Form1 m = new Form1();

         [Test]
        public void BinaryToString()
        {
            string sonuc = m.BinaryToString("011001000110010101101110011001010110110101100101");
            Assert.AreEqual("deneme", sonuc);
        }
        [Test]
        public void StringToBinary()
        {
            string sonuc = m.StringToBinary("deneme");
            Assert.AreEqual("011001000110010101101110011001010110110101100101", sonuc);
        }
        [Test]
       // 0001000100011000
        public void Substition()
        {
            string sonuc = m.Substition("0001111000000000");
            Assert.AreEqual("0001000100011000", sonuc);
        }
        [Test]
        public void ReSubstiton()
        {
            string sonuc = m.ReSubstition("0001000100011000");
            Assert.AreEqual("0001111000000000", sonuc);
        }
        [Test]
        public void ComputeSha()
        {
            string sonuc = m.ComputeStringToSha256Hash("deneme");
            Assert.AreEqual("25b80b3556ca3a15353dd2fd312062fad27adcf5a1de51b75bdadea1fa8214ab", sonuc);
        }
    }
}
