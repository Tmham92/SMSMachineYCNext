using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

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
            using (var sourceStream = File.OpenRead(OriginPath))
            using (var destinationStream = File.Create(DestinationPath))
            using (var encryptionProvider = new AesCryptoServiceProvider())
            using (var cryptoTransformer = encryptionProvider.CreateEncryptor())
            using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransformer, CryptoStreamMode.Write))
            {
                sourceStream.CopyTo(cryptoStream);
            }
        }
    }
}
