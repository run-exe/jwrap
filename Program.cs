using System.Xml.XPath;

#if !JWRAP_GEN
namespace jwrap;

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;

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

        string xml = Encoding.UTF8.GetString(Win32Res.ReadResourceData(Application.ExecutablePath, "JWRAP", "XML"));
        XDocument doc = XDocument.Parse(xml);
        //Console.WriteLine(doc.ToString());
        var root = doc.Root;
        Console.WriteLine(root.XPathSelectElement("./main"));
        Console.WriteLine(root.XPathSelectElement("./guid"));
        Console.WriteLine(root.XPathSelectElement("./sha512"));
        //byte[] jarData = Win32Res.ReadResourceData(Application.ExecutablePath, "JWRAP", "JAR");
        byte[] jarData = Convert.FromBase64String(root.XPathSelectElement("./jar").Value);
        Console.WriteLine($"jarData={jarData.Length}");
        //string guid = Encoding.UTF8.GetString(Win32Res.ReadResourceData(Application.ExecutablePath, "JWRAP", "GUID"));
        string guid = root.XPathSelectElement("./guid").Value;
        //string sha512 = Encoding.UTF8.GetString(Win32Res.ReadResourceData(Application.ExecutablePath, "JWRAP", "SHA512"));
        string sha512 = root.XPathSelectElement("./sha512").Value;
        Console.WriteLine($"guid={guid}");
        Console.WriteLine($"sha512={sha512}");
        //string mainClass = Encoding.UTF8.GetString(Win32Res.ReadResourceData(Application.ExecutablePath, "JWRAP", "MAIN"));
        string mainClass = root.XPathSelectElement("./main").Value;
        var profilePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        //Console.WriteLine(profilePath);
        var rootPath = $"{profilePath}\\.jwap\\.jar";
        Directory.CreateDirectory(rootPath);
        string jarPath = $"{rootPath}\\{Path.GetFileNameWithoutExtension(Application.ExecutablePath)}+{guid}+{sha512}.jar";
        Console.WriteLine(jarPath);
        if (!File.Exists(jarPath))
        {
            string timestamp = GetTimeStampString();
            Misc.WriteBinaryFile($"{jarPath}.{timestamp}", jarData);
            File.Move($"{jarPath}.{timestamp}", jarPath);
        }
        //string jre = PrepareJre("zulu17-jre-17.40.19");
        string jre = PrepareJre("zulu8-jre-8.68.0.21");
        Console.WriteLine(jre);
        string java = $@"{jre}\bin\java.exe";
        Console.WriteLine(java);
        Console.WriteLine(File.Exists(java));
        Console.WriteLine(mainClass);
        //string jarFile = Regex.Replace(Application.ExecutablePath, "[.][eE][xX][eE]$", ".jar");
        ProcessStartInfo psi = new ProcessStartInfo(java, $"-cp \"{jarPath}\" {mainClass} {argList}");
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
        process.OutputDataReceived += (sender, e) =>
        {
            Console.WriteLine(e.Data);
            try
            {
                using (StreamWriter writer = File.AppendText(Application.ExecutablePath+".log")) {
                    writer.WriteLine(e.Data);
                }
            }
            catch (Exception exception)
            {
            }
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            Console.WriteLine(e.Data);
            try
            {
                using (StreamWriter writer = File.AppendText(Application.ExecutablePath+".log")) {
                    writer.WriteLine(e.Data);
                }
            }
            catch (Exception exception)
            {
            }
        };
        try
        {
            File.Delete(Application.ExecutablePath+".log");
        }
        catch (Exception e)
        {
        }
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        Environment.Exit(process.ExitCode);
    }
#else
    private static void SeparateMain(string[] args)
    {
        Console.WriteLine("(1)");
        string jarPath = new FileInfo(args[0]).FullName;
        jarPath = Convert.ToBase64String(Encoding.UTF8.GetBytes(jarPath));
        ArraySegment<string> arySeg = new ArraySegment<string>(args, 1, args.Length - 1);
        args = arySeg.ToArray();
        string argList = "";
        for (int i = 0; i < args.Length; i++)
        {
            //if (i > 0) argList += " ";
            //argList += $"\"{args[i]}\"";
            if (i > 0) argList += ",";
            argList += Convert.ToBase64String(Encoding.UTF8.GetBytes(args[i]));
        }
        Console.WriteLine("["+argList+"]");
        //string jre = PrepareJre("zulu17-jre-17.40.19");
        string jre = PrepareJre("zulu8-jre-8.68.0.21");
        //https://github.com/run-exe/jwrap/releases/download/jre/zulu8-jre-8.68.0.21.zip
        //Console.WriteLine(jre);
        string java = $@"{jre}\bin\java.exe";
        //Console.WriteLine(java);
        //Console.WriteLine(File.Exists(java));
        string exeDir = Directory.GetParent(Application.ExecutablePath).FullName;
        string bootJar = $"{exeDir}\\jwrap.boot.jar";
        string mainClass = "global.Main";
        mainClass = Convert.ToBase64String(Encoding.UTF8.GetBytes(mainClass));
        
        Console.WriteLine("(2)");
        ProcessStartInfo psi = new ProcessStartInfo(java, $"-cp \"{bootJar}\" jwrap.boot.App --jar {jarPath} --main {mainClass} --args \"{argList}\"");
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;
        Process process = new Process();
        process.StartInfo = psi;

        process.OutputDataReceived += (sender, e) =>
        {
            Console.WriteLine(e.Data);
        };
        process.ErrorDataReceived += (sender, e) =>
        {
            Console.WriteLine(e.Data);
        };

        Console.WriteLine("(3)");
        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();
        Console.WriteLine("(4)");
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
