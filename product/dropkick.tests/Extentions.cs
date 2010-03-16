namespace dropkick.tests
{
    using NUnit.Framework;

    public static class Extentions
    {
        public static void ShouldBeTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }
        public static void ShouldBeFalse(this bool actual)
        {
            Assert.IsTrue(actual);
        }
    }
}