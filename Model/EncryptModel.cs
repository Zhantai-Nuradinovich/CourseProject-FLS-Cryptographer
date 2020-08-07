using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CryptographerFLS.Model
{
    public class EncryptModel
    {
        public string Key { get; set; }
        public bool ToEncrypt { get; set; }
        public string Encrypted { get; set; }
        public string Decrypted { get; set; }

        static string alph = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        static string alphUp = alph.ToUpper();
        public EncryptModel(string originalVersion, bool toEncrypt, string key)
        {
            ToEncrypt = toEncrypt;

            if(key != "" && key != null)
            {
                Key = key;
            }
            else
            {
                Key = "скорпион";
            }

            if (ToEncrypt)
            {
                Decrypted = originalVersion;
                Encrypt();
            }
            else
            {
                Encrypted = originalVersion;
                Decrypt();
            }
        }

        public void Encrypt()
        {
            string result = "";
            int keyIndex = 0;
            Key = Key.ToLower();

            foreach (var item in Decrypted)
            {
                if (alph.IndexOf(item) != -1)
                {
                    int a = (alph.IndexOf(item) + alph.IndexOf(Key[keyIndex])) % alph.Length;
                    result += alph[a];
                    keyIndex++;
                    if (keyIndex == Key.Length)
                    {
                        keyIndex = 0;
                    }
                }
                else if (alphUp.IndexOf(item) != -1)
                {
                    int a = (alphUp.IndexOf(item) + alph.IndexOf(Key[keyIndex])) % alphUp.Length;
                    result += alphUp[a];
                    keyIndex++;

                    if (keyIndex == Key.Length)
                    {
                        keyIndex = 0;
                    }
                }
                else
                {
                    result += item;
                }
            }
            Encrypted = result;
        }

        public void Decrypt()
        {
            string result = "";
            int keyIndex = 0;
            Key = Key.ToLower();

            foreach (var item in Encrypted)
            {
                if (alph.IndexOf(item) != -1)
                {
                    int a = (alph.IndexOf(item) + alph.Length - alph.IndexOf(Key[keyIndex])) % alph.Length;
                    result += alph[a];
                    keyIndex++;

                    if (keyIndex == Key.Length)
                    {
                        keyIndex = 0;
                    }
                }
                else if (alphUp.IndexOf(item) != -1)
                {
                    int a = (alphUp.IndexOf(item) + alphUp.Length - alph.IndexOf(Key[keyIndex])) % alphUp.Length;
                    result += alphUp[a];
                    keyIndex++;

                    if (keyIndex == Key.Length)
                    {
                        keyIndex = 0;
                    }
                }
                else
                {
                    result += item;
                }
            }
            Decrypted = result;
        }
    }
}
