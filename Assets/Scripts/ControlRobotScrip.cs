 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using UnityEngine.UI;


// when change value cartesian to grades  you have to recived that value 
// input data or only with botton  ?? for that momente  only botton
// set the value whe change to screen

public class ControlRobotScrip : MonoBehaviour {

    // Variables de control ** currentValueaxis
    //  if tipeJOGInterfase = 2 (M1,M2,M3) else (x,y,z)
    // if TypeMovement = true Movemen line else PTP
    /// <summary> all value of array sum  1  and this have sing  for idf it is + or -
    /// +-1 = 0 => X o M1
    /// +-2 = 1 => y o M2
    /// +-3 = 2 => Z O M3
    /// +-4 = 3 => C [0-360]
    /// +-5 = 4 => griper [0-360]
    /// </summary>
    float[] currentValueaxis = new float[] { 0f, 0f, -275f, 0f, 0f };
    public Text[] ShowValuePositionXYZ;
    public Text[] ShowValuePositionangulo;
    // Hacer matriz para valores maximos 
    private float value_1X = 0;
    private float value_2Y = 0;
    private float value_3Z = 0;
    private float value_4C = 0f;//grados
    private bool griperOpen = false;  //false = cerrado    true = abierto
    //private float value_Gripper = 0f; //gripper -- grados
    private float value_ResolutionMM = 1f;
    private float value_ResolutionDeg = 1f;
    private float value_velocidad = 10f;

    //min variables de control
    private int value_1X_min = -150;
    private int value_2Y_min = -150;
    private int value_3Z_min = -150;
    private int value_4C_min = 0;
    //max variable de control
    private int value_1X_max = 150;
    private int value_2Y_max = 150;
    private int value_3Z_max = 150;
    private int value_4C_max = 360;

    // Variables de interfaz
    private int contadorPoints = 1;
    public ListManager logControl;

    private int tipeJOGInterfase = 0;

    public Slider SliderResolution;
    public Text TextSliderResolution;


    public Slider SliderSpeed;
    public Text TextSliderSpeed;

    public InputField InputTextNamePoint;
    private string stringNamePoint;
    private bool TypeMovement = false;// false = PTP     true = line

    public GameObject ControlJoint;
    public GameObject ControlCartesian;
    public GameObject ControlJPAD;
    public GameObject FailComunicationService;
    public GameObject indicatorPAth;

    public InputField InputTextSendComand;
    private string stringSendComand;


    // Variables de envio 
    bool Forward = false;



    // variables para la comunicacion
    private string _ip;
    private string Port;
    private bool _socketReady = false;
    public GameObject ConectedButton;
    /*
    private TcpClient _tcpClient = null;
    private NetworkStream _netStream = null;
    private StreamWriter _socketWriter = null;
    private StreamReader _socketReader = null;*/

    void Awake()
    {
        Debug.Log("Awake called control de robot ");
    }

    // Use this for initialization
    void Start()
    {
        string Recive = "";

        IsConected();
        ShowDataValue();

        Debug.Log("Start called control de robot ");
        _ip = PlayerPrefs.GetString("IPvalue");
        Port = PlayerPrefs.GetString("Portvalue");
        Debug.Log("IP "+_ip +"-puerto"+Port);

        //Application.runInBackground = true;
        //Connect();

        // pedir los valores 

        //enviart a home 

        //optener lso valores de los que se encuentra el robot
        if (IsConected())
        {
            NetworkManager.instance.SendDataServer("JOG");
            Recive = NetworkManager.instance.ReciveDataServer();
            if (Recive != "")
            {
                Debug.Log("-->  Se recivio:" + Recive);
                //Debug.Log(Recive);
                //waitAnswer = false;
            }
        }
    }

    public bool IsConected()
    {
        if (NetworkManager.instance.socketState())
        {
            ConectedButton.SetActive(false);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Conect()
    {
        string Recive = "";
        NetworkManager.instance.Connect();
        if (indicatorPAth.gameObject.activeInHierarchy)
        {
            NetworkManager.instance.SendDataServer("PATH");
        }
        else
        {
            NetworkManager.instance.SendDataServer("JOG");
        }
        Recive = NetworkManager.instance.ReciveDataServer();
        if (Recive != "")
        {
            Debug.Log("-->  Se recivio:" + Recive);
            //Debug.Log(Recive);
            //waitAnswer = false;
        }
    }
    public void Disconect()
    {
        NetworkManager.instance.ClosetConection();
    }
    // Update is called once per frame
    void Update() {

        // send data current time

    }

    #region ComentedSocket
    /*
    void OnApplicationQuit()
    {
        Debug.Log("Salio de la aplicacion");
        if (_socketWriter != null)
        {
            //_socketWriter.WriteLine("EXIT");
            _socketWriter.Write("EXIT");
            _socketWriter.Flush();
            _socketReader.Close();
            _socketWriter.Close();
        }
        if (_netStream != null)
        {
            _netStream.Close();
        }
        if (_tcpClient != null)
        {
            _tcpClient.Close();
        }
        // Update is called once per frame
    }*/



    /*
    private void Connect()
    {
        //IPAddress test = IPAddress.Parse(_ip);

        try
        {
            _tcpClient = new TcpClient();
            //int port = Int32.Parse(_port);

            _tcpClient.Connect(IPAddress.Parse(_ip), int.Parse(Port));
            _netStream = _tcpClient.GetStream();
            _socketReader = new StreamReader(_netStream);
            _socketWriter = new StreamWriter(_netStream);
            _socketReady = true;
        }
        catch (Exception e)
        {    
            Debug.Log("Socket error: bla " + e);
            // poner que se pueda activar un panel para que  muestre el error 
            //FailComunicationService.SetActive(true);
        }
    }*/
    #endregion
    /// <summary>
    /// 
    /// </summary>

    // Funciones de interfas

    public void MenuScenaload()
    {
        SceneManager.LoadScene(0);
        //OnApplicationQuit();
    }

    public void SaveValueResolution()
    {
        value_ResolutionMM = SliderResolution.value;
        Debug.Log("el valor de la resolucion " + value_ResolutionMM);
    }

    public void SeeValueResolution(float value)
    {
        TextSliderResolution.text = "" + value + "[mm]";

    }
    public void SaveValueGripper() ///////////////////////////////////
    {
        if (griperOpen)
        {
            currentValueaxis[4] = 80f;
        }
        else
        {
            currentValueaxis[4] = 0f;
        }

        ChangeRobotValue(0);
        //Debug.Log("el valor de la Gripper " + value_Gripper);
    }
    public void GriperOpen()
    {
        griperOpen = true;
    }
    public void GripperClose()
    {
        griperOpen = false;
    }

    public void SavePointValue()
    {
        
        stringNamePoint = InputTextNamePoint.text;
        if (stringNamePoint == "")
        {
            stringNamePoint = "Point"+ contadorPoints;
        }
        contadorPoints++;

        value_velocidad = SliderSpeed.value;

        Debug.Log("Punto " + stringNamePoint + "moviment lineal " + TypeMovement + " Velocidad " + value_velocidad);
        logControl.LogPointContend(stringNamePoint, TypeMovement , currentValueaxis); // falta enviar posicion los valores de lso servos
    }

    public void SeeValueSpeed(float value)
    {
        TextSliderSpeed.text = "" + value + " [mm/s]";

    }
    public void TypemoveLine()
    {
        TypeMovement = true;
    }
    public void TypemovePTP()
    {
        TypeMovement = false;
    }

    // maneja la interfaz  if   var tipeJOGInterfase = 2 mueve los motores directamete  caso contrario cinematica inversa
    public void ChangeInterfaceJOG()
    {
        string data = "";
        string[] parts;
        tipeJOGInterfase++;

        if (tipeJOGInterfase == 3)
        {
            tipeJOGInterfase = 0;
        }

        switch (tipeJOGInterfase)
        {
            case 0:
                ControlJoint.SetActive(true);
                ControlCartesian.SetActive(false);
                ControlJPAD.SetActive(false);
                Debug.Log("caso 0");
                // pedir valor de xyz
                SendDataServer("XYZ");
                // recivir currentValueaxis[] con los valores de los angulos
                do
                {
                    data = ReciveDataServer();
                    parts = data.Split(' ');
                    Debug.Log("recived data:" + data);
                } while (parts[0] != "XYZ");
                //parts = data.Split(' ');
                currentValueaxis[0] = float.Parse(parts[1]);
                currentValueaxis[1] = float.Parse(parts[2]);
                currentValueaxis[2] = float.Parse(parts[3]);
                //mostrarlo 
                ShowDataValue();
                break;
            case 1:
                ControlJoint.SetActive(false);
                ControlCartesian.SetActive(true);
                ControlJPAD.SetActive(false);
                Debug.Log("caso 1");
                break;
            case 2:
                ControlJoint.SetActive(false);
                ControlCartesian.SetActive(false);
                ControlJPAD.SetActive(true);
                Debug.Log("caso 2");
                // pedir el angulo al servidor
                SendDataServer("Angle");
                // recivir currentValueaxis[] con los valores de los angulos
                do
                {
                    data = ReciveDataServer();
                    parts = data.Split(' ');
                    Debug.Log("recived data:" + data);
                } while (parts[0] != "Angle");
                currentValueaxis[0] = float.Parse(parts[1]);
                currentValueaxis[1] = float.Parse(parts[2]);
                currentValueaxis[2] = float.Parse(parts[3]);
                // pedir el angulo al servidor
                // setiar currentValueaxis[] con los valores de los angulos
                // mostrarlo en los texto
                ShowDataValue();
                
                break;
        }

    }

    public void ShowDataValue()//mostra el el valor en xyz
    {
        if (tipeJOGInterfase != 2)
        {
            for (int i = 0; i < ShowValuePositionXYZ.Length; i++)
            {
                ShowValuePositionXYZ[i].text = currentValueaxis[i] + "";
            }
        }
        else
        {
            for (int i = 0; i < ShowValuePositionangulo.Length; i++)
            {
                ShowValuePositionangulo[i].text = currentValueaxis[i] + "";
            }
        }

           
    }

    // Manage value of posicion XYZC 
    // La misma funcio para `sumar(value=+1) o restar(value=-1) solo ingresa el numero 

    // hay que mostrarlo 
    // como muestro el cambiio entre mover angulo ymover CI
    // 1 ---hacer que el programa calcule la cinematica inversa
    // 2 ---Hacer que la  rasberry me envie el valor  *** gana***

    // hay que  enviar el valor una vez cambia  como hacerlo()

    // que y como deveria hacerlo ??
    // aplicar serializacion para compremir datos 
    // como y de que forma los envio  ?? igua que los guardo-- los guardo como GCODE??

    //  --- hacer los movimiento todos en una sola funcion(el gripper  tambien ** ver luego) 
    // me acuer que ise una forma de enviar datos   sin hacer que se cierre el servidor ?? donde esta ???


    public void ChangeRobotValue( int valuechage)// Value eje , value () if +1 or -1
    {
        // solo para enviar datos por JOG  no tiene tipod e movimiento todo es ptp

        //encuentra que eje es 
        int value ;
        if (valuechage != 0)
        {
            if (valuechage > 0)
            {
                value = 1;
            }
            else
            {
                value = -1;
            }
            valuechage = valuechage * value - 1;
            //value_1X = value_1X + value_ResolutionMM * (value);
            if (valuechage == 0)
            {
                if(currentValueaxis[valuechage] < 150 && currentValueaxis[valuechage] > -150)
                {
                    currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM * (value);
                }
                else
                {
                    if (currentValueaxis[valuechage] > 150)
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] - value_ResolutionMM ;
                    }
                    else
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM;
                    }
                    
                    FailComunicationService.SetActive(true);
                }
            }
            if (valuechage == 1)
            {
                if (currentValueaxis[valuechage] < 150 && currentValueaxis[valuechage] > -150)
                {
                    currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM * (value);
                }
                else
                {
                    if (currentValueaxis[valuechage] > 150)
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] - value_ResolutionMM;
                    }
                    else
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM;
                    }
                    FailComunicationService.SetActive(true);
                }
            }
            if (valuechage == 2)
            {
                if (currentValueaxis[valuechage] < -250 && currentValueaxis[valuechage] > -375)
                {
                    currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM * (value);
                }
                else
                {
                    if (currentValueaxis[valuechage] > -250)
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] - value_ResolutionMM;
                    }
                    else
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM;
                    }
                    FailComunicationService.SetActive(true);
                }
            }
            if (valuechage == 3)
            {
                if (currentValueaxis[valuechage] < 180 && currentValueaxis[valuechage] > -180)
                {
                    currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM * (value);
                }
                else
                {
                    if (currentValueaxis[valuechage] > 180)
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] - value_ResolutionMM;
                    }
                    else
                    {
                        currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM;
                    }
                    FailComunicationService.SetActive(true);
                }
            }
            //currentValueaxis[valuechage] = currentValueaxis[valuechage] + value_ResolutionMM * (value);
            ShowDataValue();
        }

        // preproces value for send
        string sendData = "";
        if (tipeJOGInterfase == 2) // Caundo esta en angulo
        {
            sendData = "0 ";
        }
        else
        {
            sendData = "1 "; // caundo esta en xyz
        }

        foreach (float element in currentValueaxis)
        {
            sendData = sendData + element +" ";
        }

        Debug.Log("Data send "+ sendData);
        // send value 
        SendDataServer(sendData);
        Debug.Log("recived position:" + ReciveDataServer());
    }

    public void SendDataServer(string Data)
    {
        //_socketWriter.WriteLine(Data);
        //_socketWriter.Write(Data);
        //_socketWriter.Flush();
        if (!NetworkManager.instance.socketState())
        {
            return;
        }
        NetworkManager.instance.SendDataServer(Data);
        // read data server 
    }
    public string ReciveDataServer()
    {
        // read data server 
        if (!NetworkManager.instance.socketState())
        {
            return null;
        }

        string data;
        //data = _socketReader.ReadLine();
        data = NetworkManager.instance.ReciveDataServer();
        return data;
    }




    public void SendConmandServer()  // get value -1 and 1
    {
        //get value slider gripper
        if (!NetworkManager.instance.socketState())
        {
            return;
        }
        stringSendComand = InputTextSendComand.text;
        //NetworkManager.instance.SendConmandServer(stringSendComand);
        //_socketWriter.WriteLine(stringSendComand);
        //_socketWriter.Flush();
    }

    public void PlayPointProgram()
    {
        foreach (string element in logControl.GetListPointString())
        {
            SendDataServer(element);
            Debug.Log(ReciveDataServer());
            Debug.Log("recivio datos");
        }
    }

}
