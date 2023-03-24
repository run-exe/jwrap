﻿using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace jwrap;

public class Misc
{
    public static byte[] ReadBinaryFile(string filePath)
    {
        // ファイルを開く
        using (FileStream fs = new FileStream(filePath, FileMode.Open))
        {
            // バイナリデータを読み取る
            using (BinaryReader br = new BinaryReader(fs))
            {
                // ファイルのサイズを取得
                int fileSize = (int)fs.Length;
                // バイナリデータをbyte[]に読み込む
                byte[] data = br.ReadBytes(fileSize);
                // 読み込んだデータを表示
                //Console.WriteLine(BitConverter.ToString(data));
                return data;
            }
        }
    }

    public static void WriteBinaryFile(string filePath, byte[] data)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create))
        {
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(data);
            }
        }
    }

    public static string GetGuidString()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
    }

    public static string GetSha512String(byte[] data)
    {
        using (SHA512 shaM = new SHA512Managed())
        {
            byte[] hashBytes = shaM.ComputeHash(data);
            string hashString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            return hashString;
        }
    }
}