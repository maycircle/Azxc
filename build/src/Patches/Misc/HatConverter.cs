using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using Harmony;
using DuckGame;

namespace Azxc.Hacks.Misc
{
    internal static class HatConverter
    {
        public static byte[] ConvertFromPNG(byte[] fileData, long identifier, string teamName)
        {
            BitBuffer rawHat = new BitBuffer();
            // Write base section of hat
            rawHat.Write(BitConverter.GetBytes(identifier));

            // BitBuffer's Write with string argument does not work perfectly
            rawHat.Write((byte)teamName.Length);
            rawHat.Write(Encoding.UTF8.GetBytes(teamName));

            rawHat.Write(fileData.Length);
            rawHat.Write(fileData);
            
            RijndaelManaged RMCrypto = new RijndaelManaged();
            RMCrypto.Key = new byte[]
            {
                243, 22, 152, 32, 1, 244, 122, 111,
                97,  42, 13,  2,  19, 15, 45,  230
            };
            RMCrypto.GenerateIV();

            byte[] encrypted = BitConverter.GetBytes(RMCrypto.IV.Length).AddRangeToArray(RMCrypto.IV);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                    RMCrypto.CreateEncryptor(RMCrypto.Key, RMCrypto.IV), CryptoStreamMode.Write))
                {
                    using (BinaryWriter binaryWriter = new BinaryWriter(cryptoStream))
                        binaryWriter.Write(rawHat.GetBytes());
                    encrypted = encrypted.AddRangeToArray(memoryStream.ToArray());
                }
            }
            return encrypted;
        }

        public static byte[] ConvertFromPNG(string filename, string teamName)
        {
            if (File.Exists(filename))
            {
                Image image = Image.FromFile(filename);
                return ConvertFromPNG(File.ReadAllBytes(filename), GetIdentifier(image), teamName);
            }
            return null;
        }

        public static void ExportFromPNG(string filename, string teamName,
            string outputPath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(outputPath, FileMode.Create)))
                writer.Write(ConvertFromPNG(filename, teamName));
        }

        public static long GetIdentifier(Image hatImage)
        {
            // Taken from QuackHead. 630430777029345 is for hats that have cape; 402965919293045
            // is for hats that don't have cape
            return hatImage.Width == 96 ? 630430777029345L : 402965919293045L;
        }
    }
}
