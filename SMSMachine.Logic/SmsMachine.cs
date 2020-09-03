using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;

namespace SMSMachine.Logic
{
    public class SmsMachine : ISmsMachine
    {
        public string Message { get; set; }

        private int _countingClicks;
        private DateTime _lastClick;
        private string _lastButton = "";

        void ISmsMachine.SendMessage()
        {
            this.Message = "";
        }

        void ISmsMachine.UseButton(string ButtonPressed)
        {
            if (ButtonPressed == _lastButton)
            {
                if (DateTime.Now.Subtract(_lastClick).Seconds > 0.5)
                {
                    Message += ButtonPressed[0];
                    _countingClicks = 0;
                }
                else
                {
                    _countingClicks++;
                    if (_countingClicks == ButtonPressed.Length)
                        _countingClicks = 0;
                    Message = Message.Substring(0, Message.Length - 1);
                    Message += ButtonPressed[_countingClicks];
                }
            }
            else
            {
                Message += ButtonPressed[0];
                _countingClicks = 0;
            }
            _lastButton = ButtonPressed;
            _lastClick = DateTime.Now;
        }

        void ISmsMachine.RemoveLastChar()
        {
            if (this.Message.Length > 0)
            {
                this.Message = Message.Substring(0, Message.Length - 1);
            }
        }

        void ISmsMachine.SaveText(string path)
        {
            try
            {
                var file = File.Create(path);
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.Write(Message);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving text", ex);
            }
        }

        void ISmsMachine.CryptoSaveMessage(string OriginPath, string DestinationPath)
        {
            const int BlockSize = 128;
            byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            byte[] Key = { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

            using (var sourceStream = File.OpenRead(OriginPath))
            using (var streamReader = new StreamReader(sourceStream))
            using (var destinationStream = File.Create(DestinationPath))
            {
                byte[] bytes = Encoding.Unicode.GetBytes(streamReader.ReadToEnd());
                SymmetricAlgorithm crypt = Aes.Create();
                crypt.BlockSize = BlockSize;
                crypt.Key = Key;
                crypt.IV = IV;
                using (var cryptoStream = new CryptoStream(destinationStream, crypt.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    sourceStream.CopyTo(cryptoStream);
                }
            }         
        }


        //{
        //    using (var sourceStream = File.OpenRead(OriginPath))
        //    using (var destinationStream = File.Create(DestinationPath))
        //    using (var encryptionProvider = new AesCryptoServiceProvider())
        //    using (var cryptoTransformer = encryptionProvider.CreateEncryptor())
        //    using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransformer, CryptoStreamMode.Write))
        //    {
        //        sourceStream.CopyTo(cryptoStream);
        //    }
        //}

        void ISmsMachine.DecryptSavedMessage(string FilePath)
        {
            byte[] Key = { 16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
            byte[] IV = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };

            using (var sourceStream = File.OpenRead(FilePath))
            { 
                using (var streamReader = new StreamReader(sourceStream))
                {
                    byte[] bytes = Encoding.Unicode.GetBytes(streamReader.ReadToEnd());
                    SymmetricAlgorithm decrypt = Aes.Create();
                    decrypt.Key = Key;
                    decrypt.IV = IV;
                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        using (var cryptoStream = new CryptoStream(memoryStream, decrypt.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            byte[] decryptedBytes = new byte[bytes.Length];
                            cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                            this.Message = Encoding.Unicode.GetString(decryptedBytes);
                        }
                    }
                }
            }
        }
    }
}
