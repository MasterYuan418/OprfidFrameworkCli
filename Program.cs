using System;
using liboprfid;
using liboprfid.Operations;
using librcic;
using static liboprfid.CardBasicOperation;

namespace OpRFIDInitCard
{
    class Program
    {
        static Random r = new Random();
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while(true)
            {
                Console.Clear();
                Console.WriteLine("Welcome to use OpRFID Debug Client");
                Console.WriteLine("=== Which operation is you choose?");
                Console.WriteLine("- r. Read Card Info");
                Console.WriteLine("- w. Write Card Info");
                Console.WriteLine("- a. About OpRFID Simple GUI Client");
                Console.WriteLine("- p. Pay");
                Console.WriteLine("- x. Exit");
                Console.Write("?>>");
                string choose = Console.ReadLine();
                if (choose == "r" || choose == "R")
                {
                    Read();
                }
                else if (choose == "x" || choose == "X") Environment.Exit(0);
                else if (choose == "a" || choose == "A") About();
                else if (choose == "w" || choose == "W") Write();
                else if (choose == "p" || choose == "P") Pay();
            }
        }
        static void Read()
        {
            Console.WriteLine("debug Password: (0) for default,(1) for v2");
            Console.Write("?>>");
            string p = Console.ReadLine();
            byte[] pwd = new byte[6];
            if (p == "0") pwd = // password here;
            else if (p == "1") pwd = // password here;
            liboprfid.CardBasicOperation.Init(pwd, 0);
            liboprfid.CardBasicOperation.Init(pwd, 1);
            liboprfid.CardBasicOperation.Read(pwd);
            BasicInfo bi = CardParse.ParseBasicData(CardBasicOperation.rpk[0].retData);
            RCNameExtraInfo ei = CardParse.ParseRCNameData(CardBasicOperation.rpk[1].retData);
            Console.WriteLine("========= Info =========");
            Console.WriteLine("SN: {0}", bi.serialnumber);
            Console.WriteLine("Name: {0}", ei.name);
            Console.WriteLine("Group ID: {0}", bi.groupId);
            Console.WriteLine("Permission: " + bi.mainPid.ToString() + "-" + bi.secondPid.ToString());
            Console.WriteLine("Balance: " + bi.bal);
            Console.WriteLine("========================");
            Console.WriteLine("CTRLW Balance: " + bi.balCtrlWord);
            Console.WriteLine("CTRLW Card: " + bi.ctrlWord);
            Console.WriteLine("FEATURE RC Name Extra: " + ei.Supported);
            Console.WriteLine("========================");
            Console.WriteLine("-> Press any key to return.");
            Console.ReadKey();
        }
        static void Write()
        {
            //        public static void Init(CardControlWord ctrlword, string SN, CardBalanceControlWord balCtrlWord,
            //uint balance, uint mainPermission, uint secondPermission, long groupId,
            //string name, bool isEmbedded,byte[] password = null)
            Console.WriteLine("========= Info =========");
            Console.Write("|> Please enter the card control word: ");
            CardControlWord cctrlwrd = CardControlWord.NO_REGISTER;
            string _a = Console.ReadLine();
            if (_a == "NO_REGISTER") cctrlwrd = CardControlWord.NO_REGISTER;
            else if (_a == "STANDARD") cctrlwrd = CardControlWord.STANDARD;
            else if (_a == "GIFTCARD") cctrlwrd = CardControlWord.GIFTCARD;
            else if (_a == "EMBEDDED") cctrlwrd = CardControlWord.EMBEDDED;
            Console.Write("|> Please enter the balance control word: ");
            CardBalanceControlWord bctrlwrd = CardBalanceControlWord.INVAILD;
            string _b = Console.ReadLine();
            if (_a == "INVAILD") bctrlwrd = CardBalanceControlWord.INVAILD;
            else if (_a == "STANDARD") bctrlwrd = CardBalanceControlWord.STANDARD;
            else if (_a == "ONLY_PAY") bctrlwrd = CardBalanceControlWord.ONLY_PAY;
            else if (_a == "FREEZE_RECEIVE") bctrlwrd = CardBalanceControlWord.FREEZE_RECEIVE;
            else if (_a == "FREEZE_PAY") bctrlwrd = CardBalanceControlWord.FREEZE_PAY;
            else if (_a == "FREEZE_ALL") bctrlwrd = CardBalanceControlWord.FREEZE_ALL;
            Console.Write("|> Please enter the SN that card have one: ");
            string SN = Console.ReadLine();
            Console.Write("|> Please enter the balance that card have(at least 0): ");
            uint bal = (uint)Convert.ToInt32(Console.ReadLine());
            Console.Write("|> Please enter the permission word(e.g. xx-xxx): ");
            string per_ = Console.ReadLine();
            Console.Write("|> Please enter the groupId(at least 7 characters): ");
            long groupId = Convert.ToInt64(Console.ReadLine());
            Console.Write("|> Is the card is embedded?(True for true,False for false): ");
            bool isEmbedded = Convert.ToBoolean(Console.ReadLine());
            Console.Write("|> === RCNE: Please enter name: ");
            string name = Console.ReadLine();
            Console.WriteLine("==============================");
            Console.WriteLine("☐ Checking vaild ...");
            if (per_.Split("-").Length != 2) chkFailed();
            uint mainPermission = (uint)Convert.ToInt32(per_.Split("-")[0]);
            byte[] pwd = new byte[6];
            uint secondPermission = (uint)Convert.ToInt32(per_.Split("-")[1]);
            if (isEmbedded) pwd = // password here;
            else pwd = // password here;
            if (SN.Length > 8 || bal < 0 || groupId == 0 || groupId < 1000000) chkFailed();
            chkSuccess();
            Console.WriteLine("☐ Creating rand ...");
            uint rand = (uint)r.Next(0, 255);
            chkSuccess("☑ Creating rand done.");
            Console.WriteLine("☐ Finally ...");
            byte[] newpwd = pwd;
            newpwd[5] = (byte)(newpwd[5] + rand);
            string[] rx = new string[6];
            if (!isEmbedded)
            {
                ChangePassword.ChangePwd(pwd, newpwd, 0);
                ChangePassword.ChangePwd(pwd, newpwd, 1);
                for (int i = 0; i < 6; i++)
                {
                    rx[i] = newpwd[i].ToString("X");
                }
            }
            else
            {
                newpwd = // password here;
                ChangePassword.ChangePwd(pwd, newpwd, 0);
                ChangePassword.ChangePwd(pwd, newpwd, 1);
                for (int i = 0; i < 6; i++)
                {
                    rx[i] = "FF";
                }
            }
            liboprfid.Operations.InitCard.Init(cctrlwrd, SN, bctrlwrd, bal, mainPermission, secondPermission, groupId, name, isEmbedded, newpwd);
            Console.WriteLine("☑ DONE.\n Password: ");
            for (int i = 0; i < 6; i++)
            {
                Console.Write(rx[i] + "  ");
            }
            Console.WriteLine("-> Press any key to return.");
            Console.ReadKey();
        }
        static void chkFailed()
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("☒ Checking vaild failed.");
            Console.ForegroundColor = ConsoleColor.White;
            Environment.Exit(0);
        }
        static void chkSuccess(string txt = null)
        {
            Console.Clear();
            Console.WriteLine("==============================");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("☑ Checking vaild success");
            if (txt != null) Console.WriteLine(txt);
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void Pay()
        {
            Console.Write("Value: >");
            uint v = Convert.ToUInt32(Console.ReadLine());
            Console.Write("\nPassword profile");
            Console.WriteLine("debug Password: (0) for default,(1) for v2");
            Console.Write("?>>");
            string p = Console.ReadLine();
            byte[] pwd = new byte[6];
            if (p == "0") pwd = // password here;
            else if (p == "1") pwd = // password here;
            liboprfid.Operations.Pay.LetPay(v, pwd);
        }
        static void About()
        {
            Console.WriteLine("========= About =========");
            Console.WriteLine("Copyright © RisConn Studio");
            Console.WriteLine("- librcic liboprfid");
            Console.WriteLine("- Oprfid Techlogy");
            Console.WriteLine("=========================");
            Console.WriteLine("Copyright © Redism Software Nanchang LLC");
            Console.WriteLine("- OpRFIDInitCard");
            Console.WriteLine("=========================");
            Console.WriteLine("-> Press any key to return.");
            Console.ReadKey();
        }
    }
}
