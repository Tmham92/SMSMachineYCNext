using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSMachine.Logic
{
    public class StoreNameAndNumber : IStoreNameAndNumber
    {
        [Bindable(true)]
        public string Name { get; set; }

        [Bindable(true)]
        public string Number { get; set; }
        public void StoreData(string path)
        {

            try
            {
                var file = File.Create(path);
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine("Name = " + Name);
                    sw.WriteLine("Number = " + Number);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving text", ex);
            }
        }
    }
}
