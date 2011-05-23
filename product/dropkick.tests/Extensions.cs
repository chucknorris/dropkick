namespace dropkick.tests
{
    using NUnit.Framework;

    public static class Extensions
    {
        public static void ShouldBeTrue(this bool actual)
        {
            Assert.IsTrue(actual);
        }
        public static void ShouldBeFalse(this bool actual)
        {
            Assert.IsTrue(actual);
        }
        public static void ShouldBeEqualTo(this object actual,object expected)
        {
            Assert.AreEqual(expected,actual);
        }

        public static void ShouldBeOfType<T>(this object actual)
        {
            Assert.AreEqual(typeof(T), actual.GetType());
        }
    }
}