﻿using System;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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
            Misc.Log(Directory.GetParent(Application.ExecutablePath));
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(options =>
                {
                    string windowSuffix = options.window ? "w" : "";
                    string headPath = exeDir + $"\\jwrap{windowSuffix}-head.exe";
                    Misc.Log(headPath);
                    Misc.Log(options.FilePath);
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
                    Misc.Log(exePath);
                    File.Delete(exePath);
                    File.Copy(headPath, exePath);
                    //Win32Res.WriteResourceData(exePath, "JWRAP", "JAR", jarData);
                    //Win32Res.WriteResourceData(exePath, "JWRAP", "SHA512",
                    //    Encoding.UTF8.GetBytes(Misc.GetSha512String(jarData)));
                    //Win32Res.WriteResourceData(exePath, "JWRAP", "GUID", Encoding.UTF8.GetBytes(Misc.GetGuidString()));
                    string mainClass = options.main;
                    if (mainClass == null) mainClass = "global.Main";
                    //Win32Res.WriteResourceData(exePath, "JWRAP", "MAIN", Encoding.UTF8.GetBytes(mainClass));
                    XElement root = new XElement("xml",
                        new XElement("main", mainClass),
                        new XElement("guid", Misc.GetGuidString()),
                        new XElement("sha512", Misc.GetSha512String(jarData)),
                        new XElement("jar", Convert.ToBase64String(jarData))
                    );
                    XDocument doc = new XDocument(root);
                    Win32Res.WriteResourceData(exePath, "JWRAP", "XML", Encoding.UTF8.GetBytes(doc.ToString()));
                });
            //SeparateMain(args);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.ToString());
        }
    }
}
#endif