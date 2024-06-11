using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;

public static class NetManager
{
    //�����׽���
    static Socket socket;
    //���ջ�����
    static byte[] readBuff = new byte[1024];
    //ί������
    public delegate void MsgListener(string str);
    //�����б�
    private static Dictionary<string, MsgListener> listeners = new Dictionary<string, MsgListener>();
    //��Ϣ�б�
    static List<string> msgList = new List<string>();

    //��Ӽ��������Э���Ӧ��ί��
    public static void AddListener(string msgName, MsgListener listener)
    {
        listeners[msgName] = listener;
    }

    //��ȡ��������ÿͻ���ip��ַ�Ͷ˿ں�
    public static string GetDesc()
    {
        if (socket == null) return "";
        if (!socket.Connected) return "";
        return socket.LocalEndPoint.ToString();
    }

    //����
    public static void Connect(string ip, int port)
    {
        //Socket
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        //Connect ����ͬ����ʽ�򻯴��룩
        socket.Connect(ip, port);
        //BeginReceive����ʼ���տͻ�����Ϣ��ʹ���첽�ķ���
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCallback, socket);
    }

    //Receive�ص�
    private static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            Socket socket = (Socket)ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            string[] allrecv = recvStr.Split('*');//��Ϊÿ����������Ϣ������*Ϊ��ͷ
            foreach (string a in allrecv)
            {
                if (a == "") continue;
                msgList.Add(a);//����Ϣ������Ϣ�б�
                Debug.Log("������Ϣ��" + recvStr);
            }
            socket.BeginReceive(readBuff, 0, 1024, 0,
                ReceiveCallback, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("Socket Receive fail" + ex.ToString());
        }
    }

    //��������������Ϣ
    public static void Send(string sendStr)
    {
        if (socket == null) return;
        if (!socket.Connected) return;

        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.Send(sendBytes);
    }

    //Update��������Ϣ�б����Ϣ
    public static void Update()
    {
        if (msgList.Count <= 0)
            return;
        string msgStr = msgList[0];
        Debug.Log("������Ϣ:" + msgStr);
        msgList.RemoveAt(0);
        string[] split = msgStr.Split('|');
        string msgName = split[0];//��һ��ΪЭ������
        string msgArgs = split[1];
        //�����ص�;
        if (listeners.ContainsKey(msgName))
        {
            //���ö�ӦЭ���ί�к���
            listeners[msgName](msgArgs);
        }
    }
}