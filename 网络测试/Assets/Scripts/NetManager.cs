using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

public static class NetManager
{
    //定义套接字
    static Socket socket;
    //接收缓冲区
    static byte[] readBuff = new byte[1024];
    //委托类型
    public delegate void MsgListener(string str);
    //监听列表
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //消息列表
    static List<string> msgList = new List<string>();

    //添加监听，添加协议对应的委托
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //获取描述，获得客户端ip地址和端口号
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    //连接
    public static void Connect(string ip, int port)
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //Connect （用同步方式简化代码）
        socket.Connect(ip, port);
        //BeginReceive，开始接收客户端信息，使用异步的方法
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    //Receive回调
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            string[] allrecv = recvStr.Split('*');//因为每条独立的消息都是以*为开头
            foreach (string a in allrecv)
            {
                if (a == "") continue;
                msgList.Add(a);//把消息放入消息列表
                Debug.Log("接收消息：" + recvStr);
            }
            socket.BeginReceive(readBuff, 0, 1024, 0,
                ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //给服务器发送消息
    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //Update，处理消息列表的消息
    public static void Update()
    {
        if (msgList.Count <= 0)
            return;
        string msgStr = msgList[0];
        Debug.Log("处理消息:" + msgStr);
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];//第一项为协议名称
        string msgArgs = split[1];
        //监听回调;
        if (listeners.ContainsKey(msgName))
        {
            //调用对应协议的委托函数
            listeners[msgName](msgArgs);
        }
    }
}