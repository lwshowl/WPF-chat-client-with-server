using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp5
{
    class Buddy
    {
        public static List<string> buddylist = new List<string>();
        public static List<string> nicknamelist = new List<string>();
        public static Hashtable account_nickname_table = new Hashtable();
        public static int buddycount;

        public void Getbuddy()
        {
            string sendMessage = "RequestForBuddy";//发送到服务端的内容
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket = Login.GetSocket();
            clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage+","+"null" + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码

            byte[] data = new byte[1024];
            clientSocket.Receive(data);//接收返回数据
            string returnMsg = Encoding.UTF8.GetString(data);
            if (!string.IsNullOrWhiteSpace(returnMsg))
            {
                returnMsg = returnMsg.Trim();
                returnMsg = returnMsg.Replace("\r", "");
                returnMsg = returnMsg.Replace("\n", "");
                returnMsg = returnMsg.Replace("\0", "");
                returnMsg = returnMsg.Substring(0, returnMsg.Length - 1);  //最后一个逗号去掉
            }
            string[] account = returnMsg.Split(',');
            Buddy.buddycount = Convert.ToInt32(account[0]);

            for (int i = 1; i < buddycount + 1; i++)
            {
                string[] name_nickname = account[i].Split('(');
                string accountid = name_nickname[0];
                string nickname = name_nickname[1].Substring(0, name_nickname[1].Length - 1);

                buddylist.Add(accountid);
                nicknamelist.Add(nickname);
                account_nickname_table.Add(accountid, nickname);

                Window2.table.Add(accountid,i);
                Window2.index_name_table.Add(i,accountid);

            }
        }
    } 
}
