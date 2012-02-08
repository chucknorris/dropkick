using NUnit.Framework;

namespace dropkick.tests
{
    [TestFixture]
    public class HexToByteExtensionTest
    {
        [TestCase(new byte[] { 177, 118, 143, 52, 108, 228, 20, 26, 202, 185, 189, 217, 132, 126, 50, 217, 143, 19, 115, 66 }, "b1768f346ce4141acab9bdd9847e32d98f137342")]
        [TestCase(new byte[] { 0x1e, 0x45, 0xdf, 0x5f, 0x00 }, "1e45df5f00")]
        public void TestBytesToHex(byte[] byteValues, string expectedHex)
        {
            Assert.AreEqual(expectedHex, byteValues.ToHex());
        }

        [TestCase("e8", new byte[] { 0xe8 })]
        [TestCase("e833d4", new byte[] { 0xe8, 0x33, 0xd4 })]
        public void TestHexToBytes(string hexString, byte[] expectedBytes)
        {
            Assert.AreEqual(expectedBytes, hexString.FromHexToBytes());
        }
    }
}