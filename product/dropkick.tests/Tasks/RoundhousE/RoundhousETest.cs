// Copyright 2007-2010 The Apache Software Foundation.
// 
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace dropkick.tests.Tasks.RoundhousE
{
    using dropkick.DeploymentModel;
    using dropkick.Tasks.RoundhousE;
    using NUnit.Framework;

    [TestFixture]
    [Category("Integration")]
    public class RoundhousETest
    {
        [Test][Explicit]
        public void TestRoundhousE()
        {
            var connection = new DbConnectionInfo{
                Server = "(local)",
                DatabaseName = "TestRoundhouse"
            };
            var task = new RoundhousETask(connection,
                                          @"C:\Solutions\roundhouse\code_drop\sample\db\SQLServer\TestRoundhousE", "TEST",
                                          RoundhousEMode.DropCreate,DatabaseRecoveryMode.Simple,string.Empty,0,string.Empty,"git://somehwere","","",0,0, "", "", "", "", "", "", "", null);
            DeploymentResult results = task.Execute();

            System.Console.WriteLine(results);
            Assert.IsFalse(results.ContainsError());
        }
    }
}