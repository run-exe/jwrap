using System;
using System.Linq;

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
#if true
                    if (!File.Exists(options.FilePath))
                    {
                        throw new Exception($"File not exist: {options.FilePath}");
                    }
#endif
                    string exePath = Regex.Replace(options.FilePath, "[.]jar$", ".exe");
                    Console.WriteLine(exePath);
                    File.Delete(exePath);
                    File.Copy(headPath, exePath);
                    if (options.main != null)
                    {
                        Console.WriteLine("[" + options.main + "]");
                        ProcessStartInfo psi = new ProcessStartInfo("rcedit-x64.exe", $"\"{exePath}\" --set-resource-string 1 {options.main}");
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

#if JWRAP_HEAD
    private static void SeparateMain(string[] args)
    {
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) argList += " ";
            argList += $"\"{args[i]}\"";
        }
        string jre = PrepareJre("zulu17-jre-17.40.19");
        //Console.WriteLine(jre);
        string java = $@"{jre}\bin\java.exe";
        //Console.WriteLine(java);
        //Console.WriteLine(File.Exists(java));
        string jarFile = Regex.Replace(Application.ExecutablePath, "[.][eE][xX][eE]$", ".jar");
        Process process = new Process();
        process.StartInfo.FileName = java;
        process.StartInfo.Arguments = $"-cp \"{jarFile}\" global.Main {argList}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.WaitForExit();
        Environment.Exit(process.ExitCode);
    }
#else
    private static void SeparateMain(string[] args)
    {
        string jarPath = args[0];
        ArraySegment<string> arySeg = new ArraySegment<string>(args, 1, args.Length - 1);
        args = arySeg.ToArray();
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            if (i > 0) argList += " ";
            argList += $"\"{args[i]}\"";
        }

        string jre = "xxx"; //PrepareJre("zulu17-jre-17.40.19");
        //Console.WriteLine(jre);
        string java = $@"{jre}\bin\java.exe";
        //Console.WriteLine(java);
        //Console.WriteLine(File.Exists(java));
        Process process = new Process();
        process.StartInfo.FileName = java;
        process.StartInfo.Arguments = $"-cp \"{jarPath}\" global.Main {argList}";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;
        process.Start();
        process.WaitForExit();
        Environment.Exit(process.ExitCode);
    }
#endif
    private static void DownloadBinaryFromUrl(string url, string destinationPath)
    {
        WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
        var objResponse = objRequest.GetResponse();
        byte[] buffer = new byte[32768];
        using (Stream input = objResponse.GetResponseStream())
        {
            using (FileStream output = new FileStream(destinationPath, FileMode.CreateNew))
            {
                int bytesRead;
                while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}
#endif