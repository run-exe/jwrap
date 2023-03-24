using System;
using System.IO;

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

    public static string GetGuidString()
    {
        Guid guid = Guid.NewGuid();
        return guid.ToString();
    }
}