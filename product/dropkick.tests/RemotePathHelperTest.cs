namespace dropkick.tests
{
    using NUnit.Framework;

    [TestFixture]
    public class RemotePathHelperTest
    {
        [Test]
        public void NAME()
        {
            var server = "SrvTopeka01";
            var path = "E:\\bob\\bill";
            var expected = @"\\SrvTopeka01\E$\bob\bill";
            var actual = RemotePathHelper.Convert(server, path);
            Assert.AreEqual(expected, actual);

        }
    }
}