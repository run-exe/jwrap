#if !JWRAP_GEN
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace jwrap;

using System;
using System.Linq;
using System.Reflection;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            SeparateMain(args);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    private static string GetTimeStampString()
    {
        var now = DateTime.Now;
        string formatted = now.ToString("yyyy-MMdd-HHmmss-fff");
        return formatted;
    }

    private static string PrepareJre(string name)
    {
        string url = $"https://github.com/run-exe/jwrap/releases/download/jre/{name}.zip";
        //Console.WriteLine(url);
        var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //Console.WriteLine(profilePath);
        var rootPath = $"{profilePath}\\.jwap\\.jre";
        //Console.WriteLine(rootPath);
        Directory.CreateDirectory(rootPath);
        string timestamp = GetTimeStampString();
        string downloadPath = $"{rootPath}\\{name}+{timestamp}.zip";
        //Console.WriteLine(downloadPath);
        string installPath = $"{rootPath}\\{name}";
        //Console.WriteLine(installPath);
        if (!Directory.Exists(installPath))
        {
            DownloadBinaryFromUrl(url, downloadPath);
            ZipFile.ExtractToDirectory(downloadPath, $"{installPath}+{timestamp}");
            Directory.Move($"{installPath}+{timestamp}", installPath);
        }

        return installPath;
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
        Console.WriteLine(jre);
        string java = $@"{jre}\bin\java.exe";
        Console.WriteLine(java);
        Console.WriteLine(File.Exists(java));
        string mainClass = Win32Api.LoadString(1);
        if (mainClass == "") mainClass = "global.Main";
        Console.WriteLine(mainClass);
        string jarFile = Regex.Replace(Application.ExecutablePath, "[.][eE][xX][eE]$", ".jar");
        Console.WriteLine(jarFile);
        ProcessStartInfo psi = new ProcessStartInfo(java, $"-cp \"{jarFile}\" {mainClass} {argList}");
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        Process process = new Process();
        process.StartInfo = psi;
        //Process process = new Process();
        //process.StartInfo.FileName = java;
        //process.StartInfo.Arguments = $"-cp \"{jarFile}\" global.Main {argList}";
        //process.StartInfo.UseShellExecute = false;
        process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
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
        string jre = PrepareJre("zulu17-jre-17.40.19");
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
