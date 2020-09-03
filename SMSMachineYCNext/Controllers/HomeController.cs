using SMSMachine.Logic;
using SMSMachineYCNext.Database;
using SMSMachineYCNext.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace SMSMachineYCNext.Controllers
{
    public class HomeController : Controller
    {

        [Route("Home/Index")]
        public ActionResult Index()
        {
            return View();
        }

        [Route("Home/About")]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ViewResult TextingPage()
        {
            ISmsMachine smsMachine = new SmsMachine();
            smsMachine.Message = "";
            // Een soort 'BAG' waar je van alles kan opslaan. Session = per gebruiker
            Session["SmsMachineObject"] = smsMachine;
            ViewBag.Users = CreateListOfUsers();

            return View("TextingPage", smsMachine);

        }

        public ActionResult PressedButton(int? id)
        {
            ISmsMachine mySmsMachine = (ISmsMachine)Session["SmsMachineObject"];

            switch (id)
            {
                case 1:
                    mySmsMachine.UseButton("1?!/%$#");
                    break;
                case 2:
                    mySmsMachine.UseButton("abc2ABC");
                    break;
                case 3:
                    mySmsMachine.UseButton("def3DEF");
                    break;
                case 4:
                    mySmsMachine.UseButton("ghi4GHI");
                    break;
                case 5:
                    mySmsMachine.UseButton("jkl5JKL");
                    break;
                case 6:
                    mySmsMachine.UseButton("mno6MNO");
                    break;
                case 7:
                    mySmsMachine.UseButton("pqrs7PQRS");
                    break;
                case 8:
                    mySmsMachine.UseButton("tuv8TUV");
                    break;
                case 9:
                    mySmsMachine.UseButton("wxyz9WXYZ");
                    break;
                case 10:
                    mySmsMachine.UseButton("*+");
                    break;
                case 11:
                    mySmsMachine.UseButton(" 0");
                    break;
                case 12:
                    mySmsMachine.RemoveLastChar();
                    break;
            }
            return View("TextingPage", mySmsMachine);
        }

        public ActionResult SendMessage()
        {
            ISmsMachine smsMachine = (ISmsMachine)Session["SmsMachineObject"];
            smsMachine.SendMessage();
            ViewBag.Message = "Text succesfully sent";
            return View("TextingPage", smsMachine);
        }

        public ActionResult SaveMessage()
        {
            ISmsMachine smsMachine = (ISmsMachine)Session["SmsMachineObject"];

            using (var context = new DatabaseModelContainer())
            {
                var newMessage = new Message()
                {
                    TextMessage = smsMachine.Message,
                    User = context.UserSet.Find(1)
                };
                context.MessageSet.Add(newMessage);
                context.SaveChanges();
            }

            // Save to text file
            //try
            //{


            //    smsMachine.SaveText(@"C:\temp\Test.txt");
            //    ViewBag.Message = "Text saved";
            //    smsMachine.SendMessage();
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.Message = "Error while saving txt" + ex;
            //}
            smsMachine.SendMessage();
            ViewBag.Message = "Text is Saved";
                return View("TextingPage", smsMachine);
        }

        public ActionResult CryptoSaveMessage()
        {
            ISmsMachine smsMachine = (ISmsMachine)Session["SmsMachineObject"];
            try
            {
                smsMachine.CryptoSaveMessage(@"C:\temp\Test.txt", @"C:\temp\CryptoTest.txt");
                smsMachine.SendMessage();
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error while saving encrypted text." + ex;
            }

            // To clear message container
            smsMachine.SendMessage();
            ViewBag.Message = "Message Saved";
            return View("TextingPage", smsMachine);
        }

        public ActionResult DecryptSavedMessage()
        {
            ISmsMachine smsMachine = (ISmsMachine)Session["SmsMachineObject"];

            try
            {
                smsMachine.DecryptSavedMessage(@"C:\temp\CryptoTest.txt");
                ViewBag.Message = smsMachine.Message;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error while decrypting" + ex;
            }
            return View("TextingPage", smsMachine);
        }

        public ActionResult SearchForName()
        {
            return View(new RequestedData());
        }

        [HttpPost]
        public ActionResult SearchForName(RequestedData dataObject)
        {
            string path = @"C:\temp\Names.txt";
            string line;
            List<string> results = new List<string>();
            using (StreamReader sr = new StreamReader(path))
            {
                try
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Contains(dataObject.Phonenumber))
                        {
                            results.Add(line);
                            ViewBag.ifResults += line;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error occured" + ex);
                }
            }
            ViewBag.Result = results.ToArray();
            return View();
        }

        public string CreateListOfUsers()
        {
            var context = new DatabaseModelContainer();
            List<string> userList = new List<string>();
            foreach (var user in context.UserSet)
            {
                userList.Add(user.FirstName + " " + user.LastName);
            }
            while (userList.Count > 0)
            {
                ViewBag.Users += userList[0] + " ";
                userList.RemoveAt(0);
            }
            return ViewBag.Users;
        }

    }
}