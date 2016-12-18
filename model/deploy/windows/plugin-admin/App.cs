// Copyright 2012 Green Code LLC
// All rights reserved. 
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// Contributors:
//   James Domingo, Green Code LLC

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Landis.PlugIns.Admin
{
    public static class App
    {
        public static int Main(string[] args)
        {
            try {
                Assembly assembly = Assembly.GetExecutingAssembly();
                Version version = assembly.GetName().Version;
                Console.WriteLine("LANDIS-II TRANSITIONAL Plug-Ins Administration Tool {0}.{1}", version.Major, version.Minor);
                Console.WriteLine();

                // Run the command script with the same filename as the assembly
                System.Uri assemblyUri = new System.Uri(assembly.CodeBase);
                string assemblyPath = assemblyUri.LocalPath;
                string script = Path.ChangeExtension(assemblyPath, ".cmd");

                // Quote arguments with spaces
                StringBuilder scriptArgList = new StringBuilder();
                foreach (string arg in args) {
                    if (arg.Contains(" "))
                        scriptArgList.AppendFormat(" \"{0}\"", arg);
                    else
                        scriptArgList.AppendFormat(" {0}", arg);
                }
                string scriptArgs = scriptArgList.ToString();

                ProcessStartInfo processStartInfo = new ProcessStartInfo();
                processStartInfo.FileName = script;
                processStartInfo.Arguments = scriptArgs;
                processStartInfo.UseShellExecute = false;
                processStartInfo.WindowStyle = ProcessWindowStyle.Hidden;

                Process process = Process.Start(processStartInfo);
                if (process == null) {
                    Console.WriteLine("Error: Unable to run this script: {0}{1}", script, scriptArgs);
                    return 1;
                }
                Console.WriteLine("Started this script as new process (id #{0}): {1}{2}",  process.Id, script, scriptArgs);

                process.WaitForExit();
                Console.WriteLine("Process #{0} exited with code {1}", process.Id, process.ExitCode);
                return process.ExitCode;
            }
            catch (Exception exc) {
                Console.WriteLine("Internal error occurred within the program:");
                Console.WriteLine("  {0}", exc.Message);
                if (exc.InnerException != null) {
                    Console.WriteLine("  {0}", exc.InnerException.Message);
                }
                Console.WriteLine();
                Console.WriteLine("Stack trace:");
                Console.WriteLine(exc.StackTrace);
                return 1;
            }
        }
    }
}
