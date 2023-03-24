using System;
using System.Linq;
using System.Text;

#if JWRAP_GEN
namespace jwrap;

using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using CommandLine;

class Options
{
    [Option('w', "window", Required = false, HelpText = "Run in Windoe Mode")]
    public bool window { get; set; }

    [Option('m', "main", Required = false, HelpText = "Main Class Name")]
    public string main { get; set; }

    [Value(0, MetaName = "file", Required = true, HelpText = "Jar Path")]
    public string FilePath { get; set; }
}

public class JwrapGen
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            string exeDir = Directory.GetParent(Application.ExecutablePath).FullName;
            Console.WriteLine(Directory.GetParent(Application.ExecutablePath));
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options =>
                {
                    string windowSuffix = options.window ? "w" : "";
                    string headPath = exeDir + $"\\jwrap{windowSuffix}-head.exe";
                    Console.WriteLine(headPath);
                    Console.WriteLine(options.FilePath);
                    if (!options.FilePath.EndsWith(".jar"))
                    {
                        throw new Exception($"File is not jar: {options.FilePath}");
                    }
                    if (!File.Exists(options.FilePath))
                    {
                        throw new Exception($"File not exist: {options.FilePath}");
                    }
                    byte[] jarData = Misc.ReadBinaryFile(options.FilePath);
                    string exePath = Regex.Replace(options.FilePath, "[.]jar$", ".exe");
                    Console.WriteLine(exePath);
                    File.Delete(exePath);
                    File.Copy(headPath, exePath);
                    Win32Res.WriteResourceData(exePath, "JWRAP", "JAR", jarData);
                    Win32Res.WriteResourceData(exePath, "JWRAP", "GUID", Encoding.UTF8.GetBytes(Misc.GetGuidString()));
                    if (options.main != null)
                    {
                        Console.WriteLine("[" + options.main + "]");
                        ProcessStartInfo psi = new ProcessStartInfo("rcedit-x64.exe",
                            $"\"{exePath}\" --set-resource-string 1 {options.main}");
                        psi.RedirectStandardOutput = true;
                        psi.RedirectStandardError = true;
                        psi.UseShellExecute = false;
                        psi.CreateNoWindow = true;
                        Process process = new Process();
                        process.StartInfo = psi;
                        //var proc = Process.Start("rcedit-x64.exe", $"\"{exePath}\" --set-resource-string 1 {options.main}");
                        process.Start();
                        process.WaitForExit();
                        if (process.ExitCode != 0)
                        {
                            //File.Delete(exePath);
                            throw new Exception("rcedit-x64.exe failed");
                        }
                    }
                });
            //SeparateMain(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

}
#endif