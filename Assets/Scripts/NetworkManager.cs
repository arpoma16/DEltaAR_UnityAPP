using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

public class NetworkManager : MonoBehaviour
{
    // variables  singleton
    public static NetworkManager instance = null;


    // variables para la comunicacion
    private string _ip = "192.168.1.1";
    private string Port = "5001";
    private bool _socketReady = false;

    private TcpClient _tcpClient = null;
    private NetworkStream _netStream = null;
    private StreamWriter _socketWriter = null;
    private StreamReader _socketReader = null;


    // Start is called before the first frame update
    void Awake()
    {
        // documentation singleton : https://www.studica.com/blog/how-to-create-a-singleton-in-unity-3d
        if (instance == null)
        {
            instance = this;
        }else if(instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Application.runInBackground = true; 
        // -----Revisar no recuendo porque se deja  ejecutando en segundo plano 
        //talves para que el envio no  interfiera con el resto de los scripts
    }
    void Start()
    {
        
    }

    void Update()
    {
        // Update is called once per frame
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            ClosetConection();
        }
    }



    void OnApplicationQuit()
    {
        Debug.Log("Salio de la aplicacion");
        ClosetConection();
    }

    public void ClosetConection()
    {
        if (!_socketReady)
        {
            return;
        }
        if (_socketWriter != null)
        {
            _socketWriter.Write("EXIT");
            _socketWriter.Flush();
            _socketReader.Close();
            _socketWriter.Close();
            _socketReady = false;
        }
        if (_netStream != null)
        {
            _netStream.Close();
        }
        if (_tcpClient != null)
        {
            _tcpClient.Close();
        }
        _socketReady = false;
    }

    public bool socketState()
    {
        return _socketReady;
    }

    public int Connect()
    {
        //IPAddress test = IPAddress.Parse(_ip);
        // documetation unity : https://stackoverflow.com/questions/38816660/sending-data-from-unity-to-raspberry/41049471
        // https://answers.unity.com/questions/601572/unity-talking-to-arduino-via-wifiethernet.html
        //documentation  rasberry pi :https://www.youtube.com/watch?v=PYBZtV2-sLQ
        //----
        // revissar si asi la comunicacion es mejor 
        //https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
        Debug.Log("Funcion Conectar");
        _ip = PlayerPrefs.GetString("IPvalue");
        Port = PlayerPrefs.GetString("Portvalue");
        try
        {
            _tcpClient = new TcpClient();

            _tcpClient.Connect(IPAddress.Parse(_ip), int.Parse(Port));

            _tcpClient.ReceiveTimeout = 100;// tiempo de espera en ms 
            _netStream = _tcpClient.GetStream();
            _socketReader = new StreamReader(_netStream);
            _socketWriter = new StreamWriter(_netStream);
            _socketReady = true;
            Debug.Log("Se conecto");
        }
        catch (Exception e)
        {
            Debug.Log("Socket error:" + e);
            // poner que se pueda activar un panel para que  muestre el error 
            //FailComunicationService.SetActive(true);
            return 1;
        }
        return 0;
    }


    public void SendDataServer(string Data)
    {
        //_socketWriter.WriteLine(Data);
        Debug.Log("---> Se envio: " + Data);
        if (_socketReady)
        {
            _socketWriter.Write(Data);
            _socketWriter.Flush();
        }
    }

    public string ReciveDataServer()
    {
        // read data server 
        string data ="";
        if (_socketReady)
        {
            try
            {
                data = _socketReader.ReadLine();
            }
            catch (Exception e)
            {
                //Debug.Log("Error al recivir dato");
            }
        }
        return data;
    }
    /*
    public void SendConmandServer(string stringSendComand)  // get value -1 and 1
    {
        // use only for depuration
        //get value slider gripper
       // stringSendComand = InputTextSendComand.text;
        _socketWriter.WriteLine(stringSendComand);
        _socketWriter.Flush();
    }*/

}
