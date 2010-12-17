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
namespace dropkick.remote
{
    using System;
    using System.IO;
    using System.Messaging;
    using Configuration.Dsl.Msmq;


    internal class Program
    {
        //dropkick.remote create_queue msmq://servername/dk_remote
        static void Main(string[] args)
        {
            try
            {
                if (args[0] == "create_queue")
                {
                    var queuename = args[1];
                    var queueAddress = new QueueAddress(queuename);
                    var formattedName = queueAddress.LocalName;
                    MessageQueue.Create(formattedName);
                    
                    Environment.Exit(0);
                }
                else if (args[0] == "verify_queue")
                {
                    var queuename = args[1];
                    var queueAddress = new QueueAddress(queuename);
                    var formattedName = queueAddress.LocalName;
                    var result = MessageQueue.Exists(formattedName);
                    Console.WriteLine("exists");
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText("error.txt", ex.ToString());
                Console.WriteLine(ex);
                Environment.Exit(21);
            }
        }
    }
}