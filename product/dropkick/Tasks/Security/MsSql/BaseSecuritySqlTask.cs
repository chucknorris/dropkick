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
namespace dropkick.Tasks.Security.MsSql
{
    using System;
    using Tasks.MsSql;

    public abstract class BaseSecuritySqlTask :
        BaseSqlTask
    {
        protected BaseSecuritySqlTask(string serverName, string databaseName) : base(serverName, databaseName)
        {
        }

        public void GrantSelect(string objectName, string role)
        {
            Grant("SELECT", objectName, role);
        }

        public void GrantInsert(string objectName, string role)
        {
            Grant("INSERT", objectName, role);
        }

        public void GrantUpdate(string objectName, string role)
        {
            Grant("UPDATE", objectName, role);
        }

        public void GrantDelete(string objectName, string role)
        {
            Grant("DELETE", objectName, role);
        }

        public void Grant(string permission, string objectName, string role)
        {
            ExecuteSqlWithNoReturn("GRANT {0} ON [{1}] TO [{2}]".FormatWith(permission, objectName, role));
        }

        public bool DoesRoleExist(string role)
        {
            var sql = @"SELECT '1' from sys.database_principals where name = N'{0}' AND type = 'R'";
            var o = ExecuteSqlWithOneReturn<bool>(sql);
            return o;
        }
        public bool DoesUserExist(string user)
        {
            var sql = @"SELECT '1' from sys.database_principals where name = N'{0}' AND type = 'U'";
            var o = ExecuteSqlWithOneReturn<bool>(sql);
            return o;
        }
    }
}