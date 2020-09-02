namespace SMSMachine.Logic
{
    public interface ISmsMachine
    {
        string Message { get; set; }

        void SendMessage();

        void UseButton(string buttonPressed);

        void RemoveLastChar();

        void SaveText(string path);

        void CryptoSaveMessage(string OriginPath, string DestinationPath);
    }
}
