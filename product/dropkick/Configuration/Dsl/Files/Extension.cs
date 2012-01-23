// Copyright 2007-2008 The Apache Software Foundation.
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
namespace dropkick.Configuration.Dsl.Files
{
    using System;
    using System.Text;
    using FileSystem;

    public static class Extension
    {

        public static FileCopyOptions CopyFile(this ProtoServer protoServer, string from)
        {
            var proto = new ProtoCopyFileTask(new DotNetPath(), from);
            protoServer.RegisterProtoTask(proto);
            return proto;
        }
        public static CopyOptions CopyDirectory(this ProtoServer protoServer, string from)
        {
            return CopyDirectory(protoServer, o => o.Include(from));
        }
        
        public static CopyOptions CopyDirectory(this ProtoServer protoServer, Action<FromOptions> a)
        {
            var proto = new ProtoCopyDirectoryTask(new DotNetPath());
            a(proto);
            protoServer.RegisterProtoTask(proto);
            return proto;
        }

        public static void CreateEmptyFolder(this ProtoServer protoServer, string folderName)
        {
            var task = new ProtoEmptyFolderTask(new DotNetPath(), folderName);
            protoServer.RegisterProtoTask(task);
        }

        public static void EncryptWebConfig(this ProtoServer protoServer, string file)
        {
            var proto = new ProtoEncryptWebConfigTask(new DotNetPath(), file);
            protoServer.RegisterProtoTask(proto);
        }

        public static void EncryptAppConfig(this ProtoServer protoServer, string file)
        {
            var proto = new ProtoEncryptAppConfigTask(new DotNetPath(), file);
            protoServer.RegisterProtoTask(proto);
        }

        public static RenameOptions RenameFile(this ProtoServer protoServer, string file)
        {
            var proto = new ProtoRenameTask(new DotNetPath(), file);
            protoServer.RegisterProtoTask(proto);
            return proto;
        }

		public static UnzipOptions UnzipArchive(this ProtoServer protoServer, string archiveFilename)
		{
			var proto = new ProtoUnzipArchiveTask(new DotNetPath(), archiveFilename);
			protoServer.RegisterProtoTask(proto);
			return proto;
		}

        public static FilePokeOptions FilePoke(this ProtoServer protoServer, string filePath)
        {
            return protoServer.FilePoke(filePath, Encoding.Default);
        }

        public static FilePokeOptions FilePoke(this ProtoServer protoServer, string filePath, Encoding encoding)
        {
            var proto = new ProtoFilePokeTask(filePath, encoding);
            protoServer.RegisterProtoTask(proto);
            return proto;
        }
    }
}