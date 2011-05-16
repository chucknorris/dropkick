using dropkick.Configuration.Dsl.RoundhousE;
using NUnit.Framework;

namespace dropkick.tests.Configuration.Dsl.roundhouse
{
    [TestFixture]
    public class RoundhouseProtoTest
    {
        RoundhousEProtoTask proto = new RoundhousEProtoTask();

        [Test]
        public void WhenBuildingTheConnectionStringWithNoUserNameAndPasswordShouldReturnIntegratedSecurity()
        {
            string serverName = "(local)";
            string databaseName = "timmy";
            var expected = "data source={0};initial catalog={1};integrated security=sspi;".FormatWith(serverName,databaseName);
            var cs = proto.BuildConnectionString(serverName, databaseName, null, null);

            Assert.AreEqual(expected,cs);
        }

        [Test]
        public void WhenBuildingTheConnectionStringWithUserNameAndPasswordShouldReturnUserAndPassword()
        {
            string serverName = "(local)";
            string databaseName = "timmy";
            string userName = "bob";
            string password = "asdbob1234";
            var expected = "data source={0};initial catalog={1};user id={2};password={3};".FormatWith(serverName, databaseName,userName,password);
            var cs = proto.BuildConnectionString(serverName, databaseName, userName,password);

            Assert.AreEqual(expected, cs);
        }
    }
}