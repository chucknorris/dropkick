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
using dropkick.Tasks.RoundhousE;

namespace dropkick.Configuration.Dsl.RoundhousE
{
    public interface RoundhousEOptions
    {
        // RoundhousEOptions ForDatabaseType(string type);
        RoundhousEOptions OnInstance(string name);
        RoundhousEOptions OnDatabase(string name);
        RoundhousEOptions WithUserName(string userName);
        RoundhousEOptions WithPassword(string password);
        RoundhousEOptions WithRoundhousEMode(RoundhousEMode roundhouseMode);
        RoundhousEOptions WithScriptsFolder(string scriptsLocation);
        RoundhousEOptions ForEnvironment(string environment);
        RoundhousEOptions WithDatabaseRecoveryMode(DatabaseRecoveryMode recoveryMode);
        RoundhousEOptions WithRestorePath(string restorePath);
        RoundhousEOptions WithRestoreCustomOptions(string options);
        RoundhousEOptions WithRestoreTimeout(int timeout);
        RoundhousEOptions WithRepositoryPath(string repositoryPath);
        RoundhousEOptions WithVersionFile(string versionFile);
        RoundhousEOptions WithVersionXPath(string versionXPath);
        RoundhousEOptions WithCommandTimeout(int timeout);
        RoundhousEOptions WithCommandTimeoutAdmin(int timeout);
        RoundhousEOptions WithAlterDatabaseFolder(string alterDatabaseFolderName);
        RoundhousEOptions WithRunAfterCreateDatabaseFolder(string runAfterCreateDatabaseFolderName);
        RoundhousEOptions WithRunBeforeUpFolder(string runBeforeUpFolderName);
        RoundhousEOptions WithUpFolder(string upFolderName);
        RoundhousEOptions WithRunFirstAfterUpFolder(string runFirstAfterUpFolderName);
        RoundhousEOptions WithFunctionsFolder(string functionsFolderName);
        RoundhousEOptions WithViewsFolder(string viewsFolderName);
        RoundhousEOptions WithSprocsFolder(string sprocsFolderName);
        RoundhousEOptions WithIndexesFolder(string indexesFolderName);
        RoundhousEOptions WithRunAfterOtherAnyTimeScriptsFolder(string runAfterAnyOtherTimeScriptsFolderName);
        RoundhousEOptions WithPermissionsFolder(string permissionsFolderName);
        RoundhousEOptions WithVersionTable(string versionTable);
        RoundhousEOptions WithScriptsRunTable(string scriptsRunTable);
        RoundhousEOptions WithScriptsRunErrorTable(string scriptsRunErrorTable);
        RoundhousEOptions WithOutputPath(string outputPath);
        RoundhousEOptions WarnAndContinueOnOneTimeScriptChanges();
        RoundhousEOptions ErrorOnOneTimeScriptChanges();
    }

}