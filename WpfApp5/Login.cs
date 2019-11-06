using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;

namespace WpfApp5
{
    class Login
    {
        public static string login_nick_name;
        public static string loginname;
        public static string loginpwd;
        string localhost = "127.0.0.1";
        int port = 8083;
        public static Socket clientSocket;


        //-------------------------上传信息，访问服务器数据库---------------------------------//
        public string Verify(
            string username, string password)
        {
            IPAddress ip = IPAddress.Parse(localhost);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(new IPEndPoint(ip, port)); //配置服务器IP与端口  
                string sendMessage = username + "," + password;//发送到服务端的内容
                clientSocket.Send(Encoding.UTF8.GetBytes(sendMessage + "\r"));//向服务器发送数据，需要发送中文则需要使用Encoding.UTF8.GetBytes()，否则会乱码
            }
            catch
            {
                return "连接服务器失败";
            }

            byte[] data = new byte[1024];
            clientSocket.Receive(data);//接收返回数据
            string returnMsg = Encoding.UTF8.GetString(data);
            if (!string.IsNullOrWhiteSpace(returnMsg))
            {
            }
            returnMsg = returnMsg.Trim();
            returnMsg = returnMsg.Replace("\r", "");
            returnMsg = returnMsg.Replace("\n", "");
            returnMsg = returnMsg.Replace("\0", "");

            string[] splite = returnMsg.Split(',');

            string flag = splite[0];   //返回的信息中第一项是验证返回信息

            try
            {
                login_nick_name = splite[1]; //第二项是自己的昵称
            }
            catch
            {
                return "false";
            }

            return flag;
        }
        
        public static Socket GetSocket()
        {
            return clientSocket;
        }

    }
}
