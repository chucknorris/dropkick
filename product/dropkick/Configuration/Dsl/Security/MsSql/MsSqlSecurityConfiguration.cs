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
namespace dropkick.Configuration.Dsl.Security.MsSql
{
    public class MsSqlSecurityConfiguration :
        MsSqlSecurity
    {
        readonly string _database;
        readonly ProtoServer _server;

        public MsSqlSecurityConfiguration(ProtoServer server, string database)
        {
            _server = server;
            _database = database;
        }

        public MsSqlUserOptions CreateUserFor(string account)
        {
            var proto = new ProtoCreateUserTask(_database, account);
            _server.RegisterProtoTask(proto);
            return proto;
        }

        public void GrantReadToAllTables(string role)
        {
            var proto = new ProtoGrantReadToAllTablesTask(_database, role);
            _server.RegisterProtoTask(proto);
        }

        public void GrantWriteToAllTables(string role)
        {
            var proto = new ProtoGrantWriteToAllTablesTask(_database, role);
            _server.RegisterProtoTask(proto);
        }

        public void CreateALoginFor(string account)
        {
            var proto = new ProtoCreateLoginTask(_database, account);
            _server.RegisterProtoTask(proto);
        }

        public void GrantDataReader(string role)
        {
            var proto = new ProtoGrantDataReaderTask(_database, role);
            _server.RegisterProtoTask(proto);
        }

        public void GrantDataWriter(string role)
        {
            var proto = new ProtoGrantDataWriterTask(_database, role);
            _server.RegisterProtoTask(proto);
        }
    }
}