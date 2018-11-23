using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Diagnostics;

namespace BuildUtils
{
    class Program
    {
        [Serializable]
        public class Settings
        {
            public string rimworldExecPath = "";
            public string rimalongOutputDirPath = "";
        }

        static Settings settings = new Settings();

        static string modDesc = @"<?xml version={0} encoding={1}?>
<ModMetaData>
  <name>RimAlong</name>
  <author>6opoDuJIo</author>
  <url>http://rimworldgame.com</url>
  <targetVersion>1.0.0</targetVersion>
  <description>Map cooperative mod. Build {2} </description>
</ModMetaData>";

        static void WriteSettings(string filename)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var fs = File.OpenWrite(filename))
            {
                bf.Serialize(fs, settings);
            }
        }

        static void ReadSettings(string filename)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var fs = File.OpenRead(filename))
            {
                settings = (Settings)bf.Deserialize(fs);
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            string buildToolSettingsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RimAlongBuildTool");

            if (!Directory.Exists(buildToolSettingsDir))
            {
                Directory.CreateDirectory(buildToolSettingsDir);
            }

            string settingsPath = Path.Combine(buildToolSettingsDir, "Settings.bin");

            if (!System.IO.File.Exists(settingsPath))
            {
                System.IO.File.Create(settingsPath).Dispose();
                WriteSettings(settingsPath);
            }

            ReadSettings(settingsPath);

            while (string.IsNullOrEmpty(settings.rimworldExecPath) || !File.Exists(settings.rimworldExecPath))
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.AddExtension = true;
                dialog.Filter = "Rimworld executable (*.exe)|*.exe";
                dialog.ShowDialog();
                settings.rimworldExecPath = dialog.FileName;
            }

            WriteSettings(settingsPath);
            Console.WriteLine(args[0]);

            switch (args[0])
            {
                case "--Pre-RimAlong":
                    {
                        string assemblyPath = Path.Combine(Path.GetDirectoryName(settings.rimworldExecPath), "RimWorldWin64_Data", "Managed", "Assembly-CSharp.dll");

                        Console.WriteLine("Copying assembly from " + assemblyPath + " to dependencies folder");
                        File.Copy
                        (
                            assemblyPath
                            , "../../../dependencies/Assembly-CSharp.dll", true
                        );
                    }
                    break;

                case "--Post-RimAlong":
                    {
                        string modDir = Path.Combine(Path.GetDirectoryName(settings.rimworldExecPath), "Mods", "RimAlong");
                        string rimalongPath = "../../../ModFolder/RimAlong/Assemblies/CooperateRim.dll";

                        if (!Directory.Exists(modDir))
                        {
                            Console.WriteLine("Creating girectory " + modDir);
                            Directory.CreateDirectory(modDir);
                        }
                        
                        AssemblyName updatedAssemblyName = AssemblyName.GetAssemblyName(rimalongPath);
                        Console.WriteLine("RimAlong version " + updatedAssemblyName.Version + ", writing to xml");
                        File.WriteAllText("../../../ModFolder/RimAlong/About/About.xml", string.Format(modDesc, "\"1.0\"", "\"utf-8\"", updatedAssemblyName.Version));
                        Process proc = new Process();
                        proc.StartInfo.UseShellExecute = true;
                        proc.StartInfo.FileName = Path.Combine(Environment.SystemDirectory, "xcopy.exe");
                        string copyDirSrc = "../../../ModFolder/RimAlong";
                        proc.StartInfo.Arguments = "\"" + copyDirSrc + "\"" + " " + "\"" + modDir + "\"" + " /E /I /Y";
                        Console.WriteLine("Executing xcopy with " + proc.StartInfo.Arguments);
                        proc.Start();
                        proc.WaitForExit();
                        int eCode = proc.ExitCode;

                        if (eCode != 0)
                        {
                            throw new TargetInvocationException("XCopy returned " + eCode, null);
                        }
                    }
                    break;

                default:
                    throw new Exception("not implemented");
            }
        }
    }
}
