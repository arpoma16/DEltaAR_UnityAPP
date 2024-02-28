using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;
 /// <summary>
 ///  make a prefact on scenefor  send  what guia is 
 ///  but a number  and set the max and min  stages 
 /// </summary>

public class ARmanager : MonoBehaviour
{
    public UnityEngine.UI.Text txtDescipcion;
    public UnityEngine.UI.Text txtEtapa;
    public UnityEngine.UI.Image RobotGuia;
    public UnityEngine.UI.Button BtnPlay;



    public RobotMovement RobotMove ;



    public GameObject[] ContainerGuias; // contiene todas las guias y hace invisible 

    public GameObject[] Guia0Elements;// contine los elementos utilizados en la guia0

    public GameObject[] LayerObjectGuia1;// contine los elementos utilizados en la guia1
    public GameObject[] Guia2Elements;// contine los elementos utilizados en la guia2
    public GameObject TemplateObjecToMove;
    private List<GameObject> ObjectsToMove = new List<GameObject>();
    

    // targetas en los objetos 
    public GameObject[] ARobjects;
    private bool[] targetState = new bool[] {false,false,false,false,false,false,false,false,false,false,false};//leng=10 // vhange for byte  after for use less memory
    private int[] targetstate = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    // Objetos de las gias 
    private Vector3 lastposition;
    private Vector3 lastangle;
    private Vector3[] InitLocalSpace;
    private Quaternion[] InitRotation;

    //Movimento del robot 
    float[] currentValueaxis = new float[] { 0f, 0f, -310f, 0f, 0f };
    int resolution_Value =20;
    int tipeJOGInterfase = 2;
    // Variables de las guias 

    private int stage;// utilizar grabcet para la estapas de cada guia
    private int[] stagemax= new int[] { 9, 21,20,20};// utilizar grabcet para la estapas de cada guia
    private bool[] stageStatus = new bool[25];// Etapas del grafcet 
    private int numberguide = 0;
    private bool completeStage;
    private bool detectRobot;
    private int currentTarget = 1;
    private bool clicnext = false;
    private string Recive;
    private bool waitAnswer = false;
    private bool moveobj = false;
    private bool[] stageMovement = new bool[25];
    private int stageMove=0;
    private int numberObj = 0;
    private Vector3 startposition =new Vector3(0f,-300f,0f);
    private Vector3 endposition = new Vector3(0f, -300f, 0f);
    private Quaternion startrotation;
    private Quaternion endrotation;
    private float perc = 0f;
    private float currentLerptime = 0;
    private float Lerptime = 3f; //
    private float LerpVelocidad = 0.04f;// =>  m/s
    private bool next = false;
    private int numgreen = -1;
    private int numblue = -1;
    private float angle = 0.0f;
    private Vector3 axis = Vector3.zero;
    private float OfsetGripper = 0.18f;
  

    #region focos

    public void TextDescription(string show)
    {
        txtDescipcion.text = show;
    }
    //Hace foco automaticamente, generalmente el mas usado
    public void modoContinuousAuto()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_CONTINUOUSAUTO);
    }

    //Enfoca al infinito, para objetos distantes
    public void modoInfinity()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_INFINITY);
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_INFINITY);
    }

    //Enfoca objetos muy pequeños
    public void modoMacro()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_MACRO);
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_MACRO);
    }

    //Deja el modo de foco como viene por defecto en la camara de nuestro dispositivo
    public void modoNormal()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_FIXED);
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_NORMAL);
    }

    //Hace foco cuando tocamos la pantalla, ideal cuando nuestro dispositivo no cuenta con foco automatico o queremos controlar el foco manualmente
    public void modoTriggerAuto()
    {
        VuforiaBehaviour.Instance.CameraDevice.SetFocusMode(FocusMode.FOCUS_MODE_TRIGGERAUTO);   
        //CameraDevice.Instance.SetFocusMode(CameraDevice.FocusMode.FOCUS_MODE_TRIGGERAUTO);
    }
    #endregion

    public GameObject ConectedButton;

    // funtions Scene 
    public void MenuScenaload()
    {
        SceneManager.LoadScene(0);
    }
    public bool CanConect()// se conecta al robot y se desconecta para ver si  estaconectado a la red
    {
        if (IsConected())
        {
            return true;
        }
        else
        {
            if (NetworkManager.instance.Connect() == 0)
            {
                Disconect();
                return true;
            }
            else
            {
                return false;
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
        NetworkManager.instance.Connect();

        NetworkManager.instance.SendDataServer("PATH");
        Recive = NetworkManager.instance.ReciveDataServer();
        if (Recive != "")
        {
            Debug.Log("-->  Se recivio:" + Recive);
            //Debug.Log(Recive);
            waitAnswer = false;
        }
    }
    public void Disconect()
    {
        NetworkManager.instance.ClosetConection();
    }

    void Start()
    {

        Debug.Log("Start ARmanager ");
    
        numberguide = PlayerPrefs.GetInt("Guide");
        detectRobot = false;
        RobotGuia.gameObject.SetActive(true);
        stage = 0;

        if(numberguide == 1)
        {
            RobotMove.HidevirtualRobot();
        }
        if (numberguide == 2)
        {
            RobotMove.HidevirtualRobot();
        }
        if (numberguide == 3)
        {
            RobotMove.TransparencevirtualRobot();
        }
        for (int i = 0; i < ContainerGuias.Length; i++)
        {
            ContainerGuias[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < stageStatus.Length; i++)
        {
            stageStatus[i] = false;
            stageMovement[i] = false;
        }

        stageStatus[0] = true;
        stageMovement[0] = true;

        //ContainerGuias[numberguide].gameObject.SetActive(true);
        //lastposition = gameObject.GetComponent<Transform>().position;

        txtDescipcion.text = "Por favor, enfoque al robot y siga las instrucciones ";
        txtEtapa.text = "Configuración del Ambiente ";



        #region ARconfiguration
        // configuracion de la Algoritmo  de AR(movimiento segun los target )
        List<Vector3> initposition = new List<Vector3>();
        List<Quaternion> initAngle = new List<Quaternion>();

        for ( int i = 0 ; i < ARobjects.Length; i++)
        {

            initAngle.Add(ARobjects[i].transform.rotation);
            //Transforms position from world space to local space
            initposition.Add(ARobjects[i].transform.InverseTransformPoint(gameObject.transform.position));
        }
        initAngle.Add(gameObject.transform.rotation);
        InitRotation = initAngle.ToArray();
        InitLocalSpace = initposition.ToArray();
        #endregion

        BtnPlay.gameObject.SetActive(false);

        if (IsConected())
        {
            NetworkManager.instance.SendDataServer("PATH");
            Recive = NetworkManager.instance.ReciveDataServer();
            if (Recive != "")
            {
                Debug.Log("-->  Se recivio:" + Recive);
                //Debug.Log(Recive);
                waitAnswer = false;
            }
        }
    }

    /*Documeuntacion
        * Quaternion a vector//https://answers.unity.com/questions/525952/how-i-can-converting-a-quaternion-to-a-direction-v.html
        * Operation Quaternion //https://forum.unity.com/threads/get-the-difference-between-two-quaternions-and-add-it-to-another-quaternion.513187/
        */
    public void ChangePosition(int target)
    {
        //NetworkManager.instance.SendDataServer("print targeta "+ target.ToString());
        Debug.Log("Change Position  "+target+"-" + " extended");
        currentTarget = target;
        Vector3 targetvector = ARobjects[target].transform.rotation * (new Vector3(0, 0, 1));
         Debug.Log(targetvector);
        float Anglex =Vector3.Cross(targetvector, new Vector3(0, 0, 1)).x;
        float Angley = Vector3.Cross(targetvector, new Vector3(0, 0, 1)).y;
        float Anglez = Vector3.Cross(targetvector, new Vector3(0, 0, 1)).z;
        //txtDescipcion.text = target.ToString()+" Ang " +Anglex.ToString()+ " " + Angley.ToString()+ " " + Anglez.ToString();
        if (target == 4)
        {
            if (Anglex < 0.002 && Anglex > -0.002) 
            {
                Debug.Log("Change position ref target 4  ");
                //Transforms position from local space to world space
                gameObject.transform.position = ARobjects[target].transform.TransformPoint(InitLocalSpace[target]);
                // multiplicacion entre quaternios  es como suma , cuaternio inversoda el cuaternio negativo
                Quaternion relative = Quaternion.Inverse(InitRotation[target]) * InitRotation[ARobjects.Length];
                gameObject.transform.rotation = ARobjects[target].transform.rotation * relative;
            }
        }
        else
        {
            if (Anglex > 0.998)
            {
                Debug.Log("Change ref target other ");
                //Transforms position from local space to world space
                gameObject.transform.position = ARobjects[target].transform.TransformPoint(InitLocalSpace[target]);
                // multiplicacion entre quaternios  es como suma , cuaternio inversoda el cuaternio negativo
                Quaternion relative = Quaternion.Inverse(InitRotation[target]) * InitRotation[ARobjects.Length];
                gameObject.transform.rotation = ARobjects[target].transform.rotation * relative;
            }
        }
    }

    // para pruebas 
    public void Rotation(int target)
    {
        Vector3 targetvector = ARobjects[target].transform.rotation * (new Vector3(0, 0, 1));
        float Anglex = Vector3.Cross(targetvector, new Vector3(0, 0, 1)).x;
        Debug.Log("Rotacion");
        Debug.Log(Anglex);
    }

    public void ClicMOve()// crea objetos de la Guia aplicacion
    {
        if (numberguide == 3)// guia de aplicacion
        {
            Guia3Movimiento();
        }
        if (numberguide == 2)// guia de aplicacion
        {
            for (int i = 0; i < stageMovement.Length; i++)
            {
                stageMovement[i] = false;
            }

            stageMovement[0] = true;
            moveobj = true;
            numberObj = 0;
            stageMove = 0;
        }
    }

    public void Guia3Movimiento()// crea objetos de la Guia aplicacion
    {
        moveobj = false;
        if (ObjectsToMove.Count > 0)
        {
            foreach(GameObject objtomove in ObjectsToMove)
            {
                Destroy(objtomove);
            }
        }
        ObjectsToMove.Clear();
        Color[] colors = new Color[] { Color.green, Color.blue,Color.yellow };
        GameObject newObjecttoMove;
        List<Vector2> puntosnoiguales = new List<Vector2>();
        
        int c;
        int anglei;
        float xx;
        float zz;
        xx = Random.Range(-1, 2);
        zz = Random.Range(-2, 3);
        for (int i = 0; i < 3; i++)
        {
            bool areequ = true;
            if(puntosnoiguales.Count >0)
            while (areequ)
            {
                areequ = false;
                c = Random.Range(0, 2);
                xx = Random.Range(-1, 2);
                //xx = Random.Range(-0.08f,0.08f);
                zz = Random.Range(-2, 3);
                //zz = Random.Range(-0.14f, 0.14f);
                foreach( Vector2 point2d in puntosnoiguales){
                    if(xx == point2d.x && zz == point2d.y)
                    {
                        areequ = true;
                    }
                }
            }
            puntosnoiguales.Add(new Vector2(xx, zz));
            c = Random.Range(0,  2);

            anglei = Random.Range(0, 180);
            if (anglei > 90)
            {
                anglei = anglei - 180;
            }
            Vector3 position = new Vector3(TemplateObjecToMove.transform.localPosition.x+xx*0.07f, TemplateObjecToMove.transform.localPosition.y, TemplateObjecToMove.transform.localPosition.z+zz*0.07f);
            Quaternion rotation = Quaternion.AngleAxis(anglei, Vector3.up);
            newObjecttoMove = Instantiate(TemplateObjecToMove) as GameObject;

            newObjecttoMove.GetComponent<Renderer>().material.SetColor("_Color", colors[c]);

            newObjecttoMove.transform.SetParent(TemplateObjecToMove.transform.parent, false);
            newObjecttoMove.transform.localPosition = position;
            newObjecttoMove.transform.localRotation = rotation;
            newObjecttoMove.SetActive(true);

            ObjectsToMove.Add(newObjecttoMove.gameObject);

            Debug.Log(i);
            if (ObjectsToMove[i].GetComponent<Renderer>().material.color.Equals(Color.green))
            {
                Debug.Log("color verde 1");
            }
            if (Color.green == ObjectsToMove[i].GetComponent<Renderer>().material.GetColor("_Color"))
            {
                Debug.Log("color verde 2");
            }
        }
        for (int i = 0; i < stageMovement.Length; i++)
        {
            stageMovement[i] = false;
        }

        stageMovement[0] = true;
        moveobj = true;
        numberObj=0;
        stageMove = 0;

    }




    // Update is called once per frame
    void Update()
    {
        bool auxchange = true;
        if (waitAnswer)
        {
            Recive = NetworkManager.instance.ReciveDataServer();
            if (Recive != "")
            {
                Debug.Log("-->  Se recivio:"+ Recive);
                //Debug.Log(Recive);
                waitAnswer = false;
            }
        }
        
        if (moveobj)// mueve las piezas como lo haria el robot
        {
            // hacer grafcet de movimiento solo para uno  con una funcion para entrada de que numero de la lista hay que mover  
            // contador hata que sea mayor al numero de objetos  dea falso
            //Debug.Log("entro");
            if (numberguide == 3)// guia de aplicacion
            {
                GrafcetMoveObj1();
            }
            if (numberguide == 2 && stage ==10)//falta poner and stage para ver si es linea o pTP
            {
                GrafcetMoveLine();
            }
            if (numberguide == 2 && stage == 11)//falta poner and stage para ver si es linea o pTP
            {
                GrafcetMovePTP();
            }

        }
        #region MovimientoTargetas
        /*for(int i = 0; i < targetState.Length; i++)
        {
            if (targetState[i])
            {
                ChangePosition(i);
            }
        }*/

        for (int i = 0; i < targetstate.Length; i++)
        {
            if (targetstate[i] == 1)// mira quetargeta esta siendo detectada o trakeada
            {
                currentTarget = i;
                ChangePosition(i);// mueve al robot con respeto a esa targera
                auxchange = false;
            }
        }
        if (auxchange)// si no existe alguna targeta detectada o trakeada  busca las que tengan un extend traking para posicionar con respecto a esa
        {
            for (int i = 0; i < targetstate.Length; i++)
            {
                if (targetstate[i] == 2)// targetas con extend traking 
                {
                    currentTarget = i;
                    ChangePosition(i);
                }
            }
        }
        #endregion
    }
    public void GrafcetMovePTP()
    {
        string sendData = "";
        stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && !waitAnswer) || stageMovement[3]);//termino la guia 
        if (stageMovement[11])
        {// si es menor que 3   sumar pieza +1  y reiniciar las etapas si no  moveobj false //
            moveobj = false;
            next = false;
            Debug.Log(" <<<<<<<<<<   Stage 11   >>>>>>>>>>>>>");
            for (int i = 0; i < stageMovement.Length; i++)
            {
                stageMovement[i] = false;
            }
            stageMovement[0] = true;
            stageMove = 0;
        }
        stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && !waitAnswer) || stageMovement[2]);
        if (stageMovement[2])
        {
            if (stageMove == 1)
            {
                Debug.Log("<<<<<<    Stage 2  >>>>>>>>>>");//  Lineal /x /y  /z  /c/Griper
                sendData = "0 -100 0 -300 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }
            stageMove = 2;
        }

        stageMovement[1] = !stageMovement[2] && ((stageMovement[0] && !waitAnswer) || stageMovement[1]);
        if (stageMovement[1])
        {//se mueve al origen
            if (stageMove == 0)
            {
                Debug.Log("<<<<<<<<<<<<<<<     Stage 1    >>>>>>>>>>>>>>>>");// Lineal /x /y  /z  /c/Griper
                sendData = "0 100 0 -300 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                endposition = new Vector3(0.1f, -0.3f, 0f);
                waitAnswer = true;
            }
            stageMove = 1;
        }

        stageMovement[0] = !stageMovement[1] && (stageMovement[0]);
        if (stageMovement[0])
        {
            Debug.Log("Stage 0");
            endposition = new Vector3( -0.1f, -0.3f, 0f);
            stageMove = 0;
        }
    }
    public void GrafcetMoveLine()
    {
        string sendData = "";
        stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && !waitAnswer) || stageMovement[3]);//termino la guia 
        if (stageMovement[11])
        {// si es menor que 3   sumar pieza +1  y reiniciar las etapas si no  moveobj false //
            moveobj = false;
            next = false;
            Debug.Log(" <<<<<<<<<<   Stage 11   >>>>>>>>>>>>>");
            for (int i = 0; i < stageMovement.Length; i++)
            {
                stageMovement[i] = false;
            }
            stageMovement[0] = true;
            stageMove = 0;
        }
        stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && !waitAnswer) || stageMovement[2]);
        if (stageMovement[2])
        {
            if (stageMove == 1)
            {
                Debug.Log("<<<<<<    Stage 2  >>>>>>>>>>");//  Lineal /x /y  /z  /c/Griper
                sendData = "1 -100 0 -300 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }
            stageMove = 2;
        }

        stageMovement[1] = !stageMovement[2] && ((stageMovement[0] && !waitAnswer) || stageMovement[1]);
        if (stageMovement[1])
        {//se mueve al origen
            if (stageMove == 0)
            {
                Debug.Log("<<<<<<<<<<<<<<<     Stage 1    >>>>>>>>>>>>>>>>");// Lineal /x /y  /z  /c/Griper
                sendData = "1 100 0 -300 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                endposition = new Vector3( 0.1f, -0.3f, 0f);
                waitAnswer = true;
            }
            stageMove = 1;
        }

        stageMovement[0] = !stageMovement[1] && (stageMovement[0]);
        if (stageMovement[0])
        {
            Debug.Log("Stage 0");
            endposition = new Vector3( -0.1f, -0.3f, 0f);
            stageMove = 0;
        }
    }
    /*//grafcet  unido 
    public void GrafcetMoveObj()
    {
        //Debug.Log("GrafcetMoveObj");
        string sendData = "";
        stageMovement[11] = !stageMovement[12] && ((stageMovement[10] && true) || stageMovement[11]);//termino la guia 
        if (stageMovement[11])
        {// si es menor que 3   sumar pieza +1  y reiniciar las etapas si no  moveobj false //
         //stageMove = 11;
         //moveobj = false;
            next = false;
            Debug.Log(" <<<<<<<<<<   Stage 11   >>>>>>>>>>>>>");
            if (numberObj < 2)
            {
                numberObj++;
            }
            else
            {
                moveobj = false;
                numgreen = 0;
                numblue = 0;
            }

            for (int i = 0; i < stageMovement.Length; i++)
            {
                stageMovement[i] = false;
            }
            stageMovement[0] = true;
            stageMove = 0;
        }
        //stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && !waitAnswer) || stageMovement[10]);//termino la guia 
        //stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && next) || stageMovement[10]);//termino la guia
        stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && next && !waitAnswer) || stageMovement[10]);
        if (stageMovement[10])
        {// vuelve a posicion origen //
            Debug.Log("<<<<<<<<<         Stage 10         >>>>>>>>>>");
            stageMove = 10;
        }
        //stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && !waitAnswer) || stageMovement[9]);//termino la guia 
        //stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && next) || stageMovement[9]);//termino la guia 
        stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && next && !waitAnswer) || stageMovement[9]);
        if (stageMovement[9])
        {// sube a la pocicion z+10
            
            if (stageMove == 8)
            {
                Debug.Log("<<<<<<<<<<<          Stage 9         >>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+ OfsetGripper) * 1000)) + " 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
           
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));

            if (perc==1)
            {
                currentLerptime = 0f;
                next = true;
            }
            stageMove = 9;
        }
        //stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next && !waitAnswer) || stageMovement[8]);//Conectese al robot 
        //stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next ) || stageMovement[8]);//Conectese al robot 
        stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next && !waitAnswer) || stageMovement[8]);
        if (stageMovement[8])
        {//abre el gripper}
            
            if (stageMove == 7)
            {
                Debug.Log("<<<<<<<<<<       Stage 8          >>>>>>>>>>.");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+ OfsetGripper) * 1000)) + " 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }


            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.OpenGripper(30 + 30f * perc);
            if (perc == 1)
            {
                next = true;
                currentLerptime = 0f;
                startposition = ObjectsToMove[numberObj].transform.localPosition;
                endposition = new Vector3(startposition.x, startposition.y + 0.10f, startposition.z);
                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            }
            stageMove = 8;
        }
        //stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next && !waitAnswer) || stageMovement[7]);// que se conecte al wifi del robot
        //stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next ) || stageMovement[7]);// que se conecte al wifi del robot
        stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next && !waitAnswer) || stageMovement[7]);
        if (stageMovement[7])
        {// baja el gripper y baja la pieza }
            if (stageMove == 6)
            {
                Debug.Log("<<<<<<<<<<       Stage 7     >>>>>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+ OfsetGripper) * 1000 )) + " 0 20 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition, endposition, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));

            if (perc == 1)
            {
                currentLerptime = 0f;
                next = true;
                Lerptime = 1f;
            }


            stageMove = 7;
        }
        //stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next && !waitAnswer) || stageMovement[6]);
        //stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next) || stageMovement[6]);
        stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next && !waitAnswer) || stageMovement[6]);
        if (stageMovement[6])
        {// ver el color y mueve segun el color a la posicion xy y rota el gripper 
            if (stageMove == 5)
            {
                Debug.Log("<<<<<<<<<<        Stage 6         >>>>>>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+ OfsetGripper) * 1000 )) + " 0 20 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition, endposition, perc);
            ObjectsToMove[numberObj].transform.localRotation = Quaternion.Lerp(startrotation,endrotation, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f , startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));
            RobotMove.RotateGripper(angle - angle * perc);
            if (perc ==1)
            {
                currentLerptime = 0f;
                startposition = ObjectsToMove[numberObj].transform.localPosition;
                endposition = new Vector3(startposition.x, startposition.y - 0.10f, startposition.z);
                next = true;

                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            }

            stageMove = 6;
        } 
        //stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && !waitAnswer) || stageMovement[5]);//
        //stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && next) || stageMovement[5]);//
        stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && next && !waitAnswer) || stageMovement[5]);//
        if (stageMovement[5])
        {// Sube el robot  // sube la pieza


            if (stageMove == 4)
            {
                Debug.Log("<<<<<<<<<<    Stage 5         >>>>>>>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000 )) + " " + ((int)(angle)) + " 20 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime){currentLerptime = Lerptime;}
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition,endposition, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y+0.15f, startposition.z),new Vector3( endposition.x, endposition.y + 0.15f , endposition.z), perc));
            if (perc == 1 )
            {
                next = true;
                currentLerptime = 0f;
                startposition = ObjectsToMove[numberObj].transform.localPosition;
                startrotation = ObjectsToMove[numberObj].transform.localRotation;
                endrotation = Quaternion.identity; 




                if (ObjectsToMove[numberObj].GetComponent<Renderer>().material.color.Equals(Color.green))
                {
                    // sumarle a z o a x  cual num de posicion tiene
                    endposition = new Vector3(-0.10f+ numgreen*0.06f, startposition.y, +0.10f);
                    numgreen++;
                }
                else
                {
                    // sumarle a z o a x  cual num de posicion tiene
                    endposition = new Vector3(-0.10f+ numblue * 0.06f, startposition.y, -0.10f);
                    numblue++;
                }

                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;

            }



            stageMove = 5;
        }
        //stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && !waitAnswer) || stageMovement[4]);
        //stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && next) || stageMovement[4]);
        stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && next && !waitAnswer) || stageMovement[4]);
        if (stageMovement[4])
        {//cierra el gripper
            

            if (stageMove == 3)
            {
                Debug.Log("<<<<<<<<<<<       Stage 4     >>>>>>>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+(OfsetGripper-0.15f)) * 1000)) + " " + ((int)(angle)) + " 20 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.OpenGripper(60f - 30f * perc);
            if (perc == 1)
            {
                next = true;
                currentLerptime = 0f;
                startposition = ObjectsToMove[numberObj].transform.localPosition;
                endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.10f, ObjectsToMove[numberObj].transform.localPosition.z);
                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            }

            stageMove = 4;
        } 
        //stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && !waitAnswer) || stageMovement[3]);
        //stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && next ) || stageMovement[3]);
        stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && next && !waitAnswer) || stageMovement[3]);
        if (stageMovement[3])
        {// baja  a la posicion de la pieza

            

            if (stageMove == 2)
            {
                Debug.Log("<<<<<<<<<<   Stage 3   >>>>>>>>>");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + (OfsetGripper - 0.15f)) * 1000)) + " " + ((int)(angle)) + " 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;

            RobotMove.MoveGripperTo(Vector3.Lerp(startposition, endposition, perc));

            if (perc == 1)
            {
                next = true;
                currentLerptime = 0;
                startposition = endposition;
                endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y +0.1f+ 0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            }

            stageMove = 3;

        }


        //stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && !waitAnswer) || stageMovement[2]);
        //stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && next) || stageMovement[2]);
        stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && (next && !waitAnswer)) || stageMovement[2]);
        if (stageMovement[2])
        {// se mueve sobre la pieza, abre el griper y rota 

            

            if (stageMove == 1)
            {
                Debug.Log("<<<<<<    Stage 2  >>>>>>>>>>");
                //NetworkManager.instance.SendDataServer("PATH");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y+(OfsetGripper - 0.15f)) * 1000 )) + " " + ((int)(angle)) + " 40 ";
                NetworkManager.instance.SendDataServer(sendData);
                waitAnswer = true;
            }

            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.MoveGripperTo(Vector3.Lerp(startposition, endposition, perc));
            
            RobotMove.RotateGripper(angle * perc);

            if (perc == 1)
            {
                next = true;
                currentLerptime = 0;
                startposition = endposition;
                endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                Lerptime =0.100f / LerpVelocidad;
            }


            stageMove = 2;
        } 

        stageMovement[1] = !stageMovement[2] && ((stageMovement[0] && next) || stageMovement[1]);
        if (stageMovement[1])
        {//se mueve al origen
           
            if (stageMove == 0)
            {
                Debug.Log("<<<<<<<<<<<<<<<     Stage 1    >>>>>>>>>>>>>>>>");
                //NetworkManager.instance.SendDataServer("0 0 0 -300 0 0 ");
                //                                    Lineal /x /y  /z  /c/Griper
                sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)(endposition.y * 1000)) + " 0 40 ";
                NetworkManager.instance.SendDataServer(sendData);

                waitAnswer = true;
            }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;

            RobotMove.MoveGripperTo(Vector3.Lerp(startposition,endposition,perc));

            if (RobotMove.GetAngleGripper() != 60f) {
                RobotMove.OpenGripper(60f * perc);
            }
            
            if (perc == 1)
            {
                next = true;
                currentLerptime = 0;
                startposition = endposition;
                endposition =new Vector3( ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y+0.10f+0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                Debug.Log("endposition estage 1:" + endposition.ToString());
                Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;

                startrotation = Quaternion.identity;
                endrotation = ObjectsToMove[numberObj].transform.localRotation;
                ObjectsToMove[numberObj].transform.localRotation.ToAngleAxis(out angle, out axis);
                if (axis.y < 0)
                {
                    angle = -angle;
                }
            }


            stageMove = 1;

        }

        stageMovement[0] = !stageMovement[1] && (stageMovement[0]);
        if (stageMovement[0])
        {
            Debug.Log("Stage 0");
            startposition = RobotMove.Getposition();
            endposition = new Vector3(0f, -0.25f, 0f);
            Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            next = true;
            currentLerptime = 0f;
            stageMove = 0;
            
        }
    }
    //Grafcet primero virtual luego real*/
    public void GrafcetMoveObj1()
    {
        //Debug.Log("GrafcetMoveObj");
        string sendData = "";
        stageMovement[11] = !stageMovement[12] && ((stageMovement[10] && true) || stageMovement[11]);//termino la guia 
        if (stageMovement[11])
        {// si es menor que 3   sumar pieza +1  y reiniciar las etapas si no  moveobj false //
         //stageMove = 11;
         //moveobj = false;
            next = false;
            Debug.Log(" <<<<<<<<<<   Stage 11   >>>>>>>>>>>>>");
            if (numberObj < 2)
            {
                numberObj++;
            }
            else
            {
                moveobj = false;
                numgreen = 0;
                numblue = 0;
            }

            for (int i = 0; i < stageMovement.Length; i++)
            {
                stageMovement[i] = false;
            }
            stageMovement[0] = true;
            stageMove = 0;
        }
        //stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && !waitAnswer) || stageMovement[10]);//termino la guia 
        //stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && next) || stageMovement[10]);//termino la guia
        stageMovement[10] = !stageMovement[11] && ((stageMovement[9] && next) || stageMovement[10]);
        if (stageMovement[10])
        {// vuelve a posicion origen //
            Debug.Log("<<<<<<<<<         Stage 10         >>>>>>>>>>");
            stageMove = 10;
        }
        //stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && !waitAnswer) || stageMovement[9]);//termino la guia 
        //stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && next) || stageMovement[9]);//termino la guia 
        stageMovement[9] = !stageMovement[10] && ((stageMovement[8] && next ) || stageMovement[9]);
        if (stageMovement[9])
        {// sube a la pocicion z+10
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;

            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));

            if (perc == 1)
            {
                if (stageMove == 8)
                {
                    Debug.Log("<<<<<<<<<<<          Stage 9         >>>>>>>>>");//  Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000)) + " 0 40 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    currentLerptime = 0f;
                    next = true;
                }
                stageMove = 9;
            }
        }
        //stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next && !waitAnswer) || stageMovement[8]);//Conectese al robot 
        //stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next ) || stageMovement[8]);//Conectese al robot 
        stageMovement[8] = !stageMovement[9] && ((stageMovement[7] && next ) || stageMovement[8]);
        if (stageMovement[8])
        {//abre el gripper}
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.OpenGripper(30 + 30f * perc);
            if (perc == 1)
            {
                if (stageMove == 7)
                {
                    Debug.Log("<<<<<<<<<<       Stage 8          >>>>>>>>>>.");// Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000)) + " 0 40 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = ObjectsToMove[numberObj].transform.localPosition;
                    endposition = new Vector3(startposition.x, startposition.y + 0.10f, startposition.z);
                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
                }
                stageMove = 8;
            } 
        }
        //stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next && !waitAnswer) || stageMovement[7]);// que se conecte al wifi del robot
        //stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next ) || stageMovement[7]);// que se conecte al wifi del robot
        stageMovement[7] = !stageMovement[8] && ((stageMovement[6] && next) || stageMovement[7]);
        if (stageMovement[7])
        {// baja el gripper y baja la pieza }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition, endposition, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));

            if (perc == 1)
            {
                if (stageMove == 6)
                {
                    Debug.Log("<<<<<<<<<<       Stage 7     >>>>>>>>>>>>");// Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000)) + " 0 20 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    currentLerptime = 0f;
                    next = true;
                    Lerptime = 1f;
                }
                stageMove = 7;
            }
        }
        //stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next && !waitAnswer) || stageMovement[6]);
        //stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next) || stageMovement[6]);
        stageMovement[6] = !stageMovement[7] && ((stageMovement[5] && next ) || stageMovement[6]);
        if (stageMovement[6])
        {// ver el color y mueve segun el color a la posicion xy y rota el gripper 
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition, endposition, perc);
            ObjectsToMove[numberObj].transform.localRotation = Quaternion.Lerp(startrotation, endrotation, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));
            RobotMove.RotateGripper(angle - angle * perc);
            if (perc == 1)
            {
                if (stageMove == 5)
                {
                    Debug.Log("<<<<<<<<<<        Stage 6         >>>>>>>>>>>>>");//Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000)) + " 0 20 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if(!waitAnswer)
                {
                    currentLerptime = 0f;
                    startposition = ObjectsToMove[numberObj].transform.localPosition;
                    endposition = new Vector3(startposition.x, startposition.y - 0.10f, startposition.z);
                    next = true;

                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
                }
                stageMove = 6;
            }

            
        }
        //stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && !waitAnswer) || stageMovement[5]);//
        //stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && next) || stageMovement[5]);//
        stageMovement[5] = !stageMovement[6] && ((stageMovement[4] && next ) || stageMovement[5]);//
        if (stageMovement[5])
        {// Sube el robot  // sube la pieza
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            ObjectsToMove[numberObj].transform.localPosition = Vector3.Lerp(startposition, endposition, perc);
            RobotMove.MoveGripperTo(Vector3.Lerp(new Vector3(startposition.x, startposition.y + 0.15f, startposition.z), new Vector3(endposition.x, endposition.y + 0.15f, endposition.z), perc));
            if (perc == 1)
            {
                if (stageMove == 4)
                {
                    Debug.Log("<<<<<<<<<<    Stage 5         >>>>>>>>>>>>>>");// Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + OfsetGripper) * 1000)) + " " + ((int)(angle)) + " 20 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }

                if (!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = ObjectsToMove[numberObj].transform.localPosition;
                    startrotation = ObjectsToMove[numberObj].transform.localRotation;
                    endrotation = Quaternion.identity;

                    if (ObjectsToMove[numberObj].GetComponent<Renderer>().material.color.Equals(Color.green))
                    {// sumarle a z o a x  cual num de posicion tiene
                        endposition = new Vector3(-0.10f + numgreen * 0.06f, startposition.y, +0.10f);
                        numgreen++;
                    }
                    else
                    {// sumarle a z o a x  cual num de posicion tiene// sumarle a z o a x  cual num de posicion tiene
                        endposition = new Vector3(-0.10f + numblue * 0.06f, startposition.y, -0.10f);
                        numblue++;
                    }

                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
                }

                stageMove = 5;
            }

        }
        //stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && !waitAnswer) || stageMovement[4]);
        //stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && next) || stageMovement[4]);
        stageMovement[4] = !stageMovement[5] && ((stageMovement[3] && next) || stageMovement[4]);
        if (stageMovement[4])
        {//cierra el gripper
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.OpenGripper(60f - 30f * perc);
            if (perc == 1)
            {
                if (stageMove == 3)
                {
                    Debug.Log("<<<<<<<<<<<       Stage 4     >>>>>>>>>>>>>>");
                    //                                    Lineal /x /y  /z  /c/Griper
                    sendData = "1 " + ((int)(startposition.x * 1000)) + " " + ((int)(startposition.z * 1000)) + " " + ((int)((startposition.y + (OfsetGripper - 0.15f)) * 1000)) + " " + ((int)(angle)) + " 20 ";
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = ObjectsToMove[numberObj].transform.localPosition;
                    endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.10f, ObjectsToMove[numberObj].transform.localPosition.z);
                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
                }
                stageMove = 4;
            }
        }
        //stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && !waitAnswer) || stageMovement[3]);
        //stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && next ) || stageMovement[3]);
        stageMovement[3] = !stageMovement[4] && ((stageMovement[2] && next ) || stageMovement[3]);
        if (stageMovement[3])
        {// baja  a la posicion de la pieza
            if (currentLerptime == 0f) { Debug.Log("<<<<<<<<<<<<<<<     Stage 3  begin    >>>>>>>>>>>>>>>>"); }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;

            RobotMove.MoveGripperTo(Vector3.Lerp(startposition, endposition, perc));

            if (perc == 1f)
            {
                if (stageMove == 2)
                {
                    Debug.Log("<<<<<<<<<<          Stage 3  end               >>>>>>>>>");
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + (OfsetGripper - 0.15f)) * 1000)) + " " + ((int)(angle)) + " 40 ";// Lineal /x /y  /z  /c/Griper
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = endposition;
                    endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.1f + 0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
                }

                stageMove = 3;
            }
        }

        stageMovement[2] = !stageMovement[3] && ((stageMovement[1] && (next )) || stageMovement[2]);
        if (stageMovement[2])
        {// se mueve sobre la pieza, abre el griper y rota 
            if (currentLerptime == 0f) { Debug.Log("<<<<<<<<<<<<<<<     Stage 2  begin  >>>>>>>>>>>>>>>>"); }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;
            RobotMove.MoveGripperTo(Vector3.Lerp(startposition, endposition, perc));

            RobotMove.RotateGripper(angle * perc);

            if (perc == 1)
            {
                if (stageMove == 1)
                {
                    Debug.Log("<<<<<<    Stage 2  end  >>>>>>>>>>");
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)((endposition.y + (OfsetGripper - 0.15f)) * 1000)) + " " + ((int)(angle)) + " 40 ";//  Lineal /x /y  /z  /c/Griper
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if(!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = endposition;
                    endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                    Lerptime = 0.100f / LerpVelocidad;
                }
                stageMove = 2;
            }
        }

        stageMovement[1] = !stageMovement[2] && ((stageMovement[0] && next) || stageMovement[1]);
        if (stageMovement[1])
        {//se mueve al origen
            if (currentLerptime == 0f)
            {
                Debug.Log("<<<<<<<<<<<<<<<     Stage 1  begin  >>>>>>>>>>>>>>>>");
                Debug.Log("endposition estage 1:" + startposition.ToString());
            }
            next = false;
            currentLerptime += Time.deltaTime;
            if (currentLerptime >= Lerptime) { currentLerptime = Lerptime; }
            perc = currentLerptime / Lerptime;

            RobotMove.MoveGripperTo(Vector3.Lerp(startposition, endposition, perc));

            if (RobotMove.GetAngleGripper() != 60f)
            {
                RobotMove.OpenGripper(60f * perc);
            }

            if (perc == 1f)
            {
                if (stageMove == 0)
                {
                    Debug.Log("<<<<<<<<<<<<<<<     Stage 1  end   >>>>>>>>>>>>>>>>");
                    sendData = "1 " + ((int)(endposition.x * 1000)) + " " + ((int)(endposition.z * 1000)) + " " + ((int)(endposition.y * 1000)) + " 0 40 ";// Lineal /x /y  /z  /c/Griper
                    NetworkManager.instance.SendDataServer(sendData);
                    waitAnswer = true;
                }
                if (!waitAnswer)
                {
                    next = true;
                    currentLerptime = 0f;
                    startposition = endposition;
                    endposition = new Vector3(ObjectsToMove[numberObj].transform.localPosition.x, ObjectsToMove[numberObj].transform.localPosition.y + 0.10f + 0.15f, ObjectsToMove[numberObj].transform.localPosition.z);
                    Debug.Log("endposition estage 1:" + endposition.ToString());
                    Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;

                    startrotation = Quaternion.identity;
                    endrotation = ObjectsToMove[numberObj].transform.localRotation;
                    ObjectsToMove[numberObj].transform.localRotation.ToAngleAxis(out angle, out axis);
                    if (axis.y < 0)
                    {
                        angle = -angle;
                    }
                }
                stageMove = 1;
            }
        }

        stageMovement[0] = !stageMovement[1] && (stageMovement[0]);
        if (stageMovement[0]) //  configuracion de parametros
        {
            Debug.Log("Stage 0");
            startposition = RobotMove.Getposition();
            endposition = new Vector3(0f, -0.25f, 0f);
            Lerptime = Vector3.Distance(startposition, endposition) / LerpVelocidad;
            next = true;
            currentLerptime = 0f;
            stageMove = 0;
        }
    }


    public void LostRobot()
    {
        detectRobot = false;
        RobotGuia.gameObject.SetActive(true);
    }
    
    public void TargetState(int TargetNumber, bool TargerState)
    {
        /// Estados de la targeta 
        ///  0 - no es detectada 
        ///  1 - es detectada y seguida (dar masprioridad que el traking extendido)
        ///  2 - traking extend

        if (TargerState)
        {
            currentTarget = TargetNumber;
            //NetworkManager.instance.SendDataServer("print targeta " + TargetNumber.ToString());
            targetState[TargetNumber] = true;
        }
        else
        {
            targetState[TargetNumber] = false;
        }

        if (TargetNumber == 0 && TargerState == true && detectRobot == false)
        {
            detectRobot = true;
            RobotGuia.gameObject.SetActive(false);

        }
        else
        {
            // ejecutar funcion del stage 
            //txtDescipcion.text = "targeta" + TargetNumber.ToString() + "  Estado" + TargerState.ToString();
        }
    }
    public void TargetState(int TargetNumber, int TargerState)
    {
        /// Estados de la targeta 
        ///  0 - no es detectada 
        ///  1 - es detectada y seguida (dar masprioridad que el traking extendido)
        ///  2 - traking extend

        if (TargerState == 1)
        {
            //currentTarget = TargetNumber;
            //NetworkManager.instance.SendDataServer("print targeta " + TargetNumber.ToString());
            targetstate[TargetNumber] = TargerState;
        }
        else
        {
            targetstate[TargetNumber] = TargerState;
        }

        if (TargetNumber == 0 && TargerState == 1 && detectRobot == false)
        {
            detectRobot = true;
            RobotGuia.gameObject.SetActive(false);
        }
        else
        {
            // ejecutar funcion del stage 
            //txtDescipcion.text = "targeta" + TargetNumber.ToString() + "  Estado" + TargerState.ToString();
        }
    }




    #region Guias

    /// <summary>
    /// Cambia el valor segun se aplasten los botones  de la guia 
    /// </summary>
    /// <param name="nextOrPrev"></param>
    public void ChangeStage( int nextOrPrev)// todas las estages enpiesan con 1
    {
        clicnext = true;

        if (nextOrPrev <1)
        {
            clicnext = false;
            stage = stage - 1;

            for (int i = 0; i < stageStatus.Length; i++)
            {
                stageStatus[i] = false;
            }

            if (stage <= 0)
            {
                stage = 0;
                detectRobot = false;
                RobotGuia.gameObject.SetActive(true);
                txtDescipcion.text = "Por favor, enfoque al robot y siga las instrucciones";
                txtEtapa.text = "Configuración del ambiente";
            }
            stageStatus[stage] = true;
        }
        if (detectRobot) // enfocar al robot para comensar la guia 
        {// define el numero de guia a segirse tiene que adquirirse mediante playerprefact que guarde la escena de menu a lo que  se escoja la guia 
            if (numberguide == 0)//guia de como conectarse
            {
                GrafcetGuia0();
                Guia0(stage);
            }
            if (numberguide == 1)// guia de partes
            {
                GrafcetGuia1();
                Guia1(stage);
            }
            if (numberguide == 2)// guia de caracteristicas
            {
                GrafcetGuia2();
                Guia2(stage);
            }
            if (numberguide == 3)// guia de aplicacion
            {
                GrafcetGuia3();
                Guia3(stage);
            }
        }

        clicnext = false;
        

    }
   


    public void GrafcetGuia0()
    {
        stageStatus[10] = !stageStatus[11] && ((stageStatus[9] && clicnext) || stageStatus[10]);//termino la guia 
        if (stageStatus[10]) { stage = 10; }
        stageStatus[9] = !stageStatus[10] && ((stageStatus[8] && clicnext) || stageStatus[9]);//Conectese al robot 
        if (stageStatus[9]) { stage = 9; }
        stageStatus[8] = !stageStatus[9] && ((stageStatus[7] && clicnext && (CanConect())) || stageStatus[8]);// que se conecte al wifi del robot
        if (stageStatus[8]) { stage = 8; }
        stageStatus[7] = !stageStatus[8] && ((stageStatus[6] && clicnext) || stageStatus[7]);
        if (stageStatus[7]) { stage = 7; }
        stageStatus[6] = !stageStatus[7] && ((stageStatus[5] && clicnext) || stageStatus[6]);//
        if (stageStatus[6]) { stage = 6; }
        stageStatus[5] = !stageStatus[6] && ((stageStatus[4] && clicnext) || stageStatus[5]);
        if (stageStatus[5]) { stage = 5; }
        stageStatus[4] = !stageStatus[5] && ((stageStatus[3] && clicnext && (currentTarget == 3)) || stageStatus[4]);
        if (stageStatus[4]) { stage = 4; }
        stageStatus[3] = !stageStatus[4] && ((stageStatus[2] && clicnext) || stageStatus[3]);// pide que se apunte la parte posterior del robot
        if (stageStatus[3]) { stage = 3; }
        stageStatus[2] = !stageStatus[3] && ((stageStatus[1] && clicnext) || stageStatus[2]);// pide que se apunte la parte posterior del robot
        if (stageStatus[2]) { stage = 2;  }
        stageStatus[1] = !stageStatus[2] && ((stageStatus[0] && clicnext) || stageStatus[1]);
        if (stageStatus[1]) { stage = 1;  }
        stageStatus[0] = !stageStatus[1] && (stageStatus[0]);
    }

    public void Guia0(int stage) // encendido y coneccion del robot 
    {
        ContainerGuias[numberguide].gameObject.SetActive(true);
        for (int i = 0; i < Guia0Elements.Length; i++)
        {
            Guia0Elements[i].gameObject.SetActive(false);
        }

        // 1- enfocar al robot 
        if (stage == 1)// 2- presentacion del robot ( es una  herramienta para la ensenarna sa de robotica )
        {
            txtEtapa.text = "Actividad";
            txtDescipcion.text = "Instrucciones, pulse la flecha para continuar";
            Guia0Elements[0].gameObject.SetActive(true);
        }
        if (stage == 2)// 3- Claibracion de la camara 
        {
            txtEtapa.text = "Calibración de cámara";
            txtDescipcion.text = "Pulse en el icono de cámara debajo del botón conectar ";
            Guia0Elements[0].gameObject.SetActive(false);

        }
        if (stage == 3)// 3- pedirleque enfoque la parte trasera del robot 
        {
            txtEtapa.text = "Encendido del robot ";
            txtDescipcion.text = "Enfoque el logo que se encuentra en la parte posterior del DeltaAR ";
            Guia0Elements[0].gameObject.SetActive(false);

        }
        // 4- peridrle que conecte la alimentacion del robot
        if (stage == 4)
        {
            txtEtapa.text = "Encendido del robot";
            txtDescipcion.text = "Conecte el cable de alimentación como se indica";
            Guia0Elements[1].gameObject.SetActive(true);
        }
        // 5- hacer que prenda al robot 
        if (stage == 5)
        {
            txtEtapa.text = "Encendido del robot";
            txtDescipcion.text = "Presione el interruptor para encender a DeltaAR";
            Guia0Elements[1].gameObject.SetActive(false);
            Guia0Elements[2].gameObject.SetActive(true);
        }
        // 6- pedirle que se conecte  al wifi del robot 
        if (stage == 6)
        {
            txtEtapa.text = "Conexión a DeltaAR";
            txtDescipcion.text = "El robot se moverá a su posición de trabajo";
            Guia0Elements[2].gameObject.SetActive(false);

        }
        if (stage == 7)
        {
            txtEtapa.text = "Conexión a DeltaAR";
            txtDescipcion.text = "El robot se moverá a su posición de trabajo";
        }
        if (stage == 8)
        {
            txtEtapa.text = "Conexión a DeltaAR";
            txtDescipcion.text = "Conéctese a la red WIFI del robot";
            Guia0Elements[3].gameObject.SetActive(true);
        }
        if (stage == 9)
        {
            txtEtapa.text = "Conexión a DeltaAR";
            txtDescipcion.text = "Pulse conectar";
            Guia0Elements[3].gameObject.SetActive(false);
        }
        if (stage == 10)
        {
            txtEtapa.text = "Guía terminada con éxito";
            txtDescipcion.text = "Congratulations";
        }
        // 7- pedirle que se conecte al robot 
    }


    public void GrafcetGuia1()
    {
        stageStatus[22] = !stageStatus[23] && ((stageStatus[21] && clicnext) || stageStatus[22]);
        if (stageStatus[22]) { stage = 22; }
        stageStatus[21] = !stageStatus[22] && ((stageStatus[20] && clicnext) || stageStatus[21]);
        if (stageStatus[21]) { stage = 21; }
        stageStatus[20] = !stageStatus[21] && ((stageStatus[19] && clicnext) || stageStatus[20]);
        if (stageStatus[20]) { stage = 20; }
        stageStatus[19] = !stageStatus[20] && ((stageStatus[18] && clicnext) || stageStatus[19]);
        if (stageStatus[19]) { stage = 19; }
        stageStatus[18] = !stageStatus[19] && ((stageStatus[17] && clicnext) || stageStatus[18]);
        if (stageStatus[18]) { stage = 18; }
        stageStatus[17] = !stageStatus[18] && ((stageStatus[16] && clicnext) || stageStatus[17]);
        if (stageStatus[17]) { stage = 17; }
        stageStatus[16] = !stageStatus[17] && ((stageStatus[15] && clicnext) || stageStatus[16]);
        if (stageStatus[16]) { stage = 16; }
        stageStatus[15] = !stageStatus[16] && ((stageStatus[14] && clicnext) || stageStatus[15]);
        if (stageStatus[15]) { stage = 15; }
        stageStatus[14] = !stageStatus[15] && ((stageStatus[13] && clicnext) || stageStatus[14]);
        if (stageStatus[14]) { stage = 14; }
        stageStatus[13] = !stageStatus[14] && ((stageStatus[12] && clicnext) || stageStatus[13]);
        if (stageStatus[13]) { stage = 13; }
        stageStatus[12] = !stageStatus[13] && ((stageStatus[11] && clicnext) || stageStatus[12]);
        if (stageStatus[12]) { stage = 12; }
        stageStatus[11] = !stageStatus[12] && ((stageStatus[10] && clicnext) || stageStatus[11]);
        if (stageStatus[11]) { stage = 11; }
        stageStatus[10] = !stageStatus[11] && ((stageStatus[9] && clicnext) || stageStatus[10]);
        if (stageStatus[10]) { stage = 10; }
        stageStatus[9] = !stageStatus[10] && ((stageStatus[8] && clicnext) || stageStatus[9]);
        if (stageStatus[9]) { stage = 9; }
        stageStatus[8] = !stageStatus[9] && ((stageStatus[7] && clicnext) || stageStatus[8]);
        if (stageStatus[8]) { stage = 8; }
        stageStatus[7] = !stageStatus[8] && ((stageStatus[6] && clicnext) || stageStatus[7]);
        if (stageStatus[7]) { stage = 7; }
        stageStatus[6] = !stageStatus[7] && ((stageStatus[5] && clicnext) || stageStatus[6]);
        if (stageStatus[6]) { stage = 6; }
        stageStatus[5] = !stageStatus[6] && ((stageStatus[4] && clicnext) || stageStatus[5]);
        if (stageStatus[5]) { stage = 5; }
        stageStatus[4] = !stageStatus[5] && ((stageStatus[3] && clicnext) || stageStatus[4]);
        if (stageStatus[4]) { stage = 4; }
        stageStatus[3] = !stageStatus[4] && ((stageStatus[2] && clicnext) || stageStatus[3]);
        if (stageStatus[3]) { stage = 3; }
        stageStatus[2] = !stageStatus[3] && ((stageStatus[1] && clicnext) || stageStatus[2]);
        if (stageStatus[2]) { stage = 2; }
        stageStatus[1] = !stageStatus[2] && ((stageStatus[0] && clicnext) || stageStatus[1]);
        if (stageStatus[1]) { stage = 1; }
        stageStatus[0] = !stageStatus[1] && (stageStatus[0]);
    }
    public void GrafcetGuia2()
    {
        /*
        stageStatus[15] = !stageStatus[16] && ((stageStatus[14] && clicnext) || stageStatus[15]);
        if (stageStatus[15]) { stage = 15; }
        stageStatus[14] = !stageStatus[15] && ((stageStatus[13] && clicnext) || stageStatus[14]);
        if (stageStatus[14]) { stage = 14; }*/
        stageStatus[13] = !stageStatus[14] && ((stageStatus[12] && clicnext && !waitAnswer) || stageStatus[13]);
        if (stageStatus[13]) { stage = 13; }
        stageStatus[12] = !stageStatus[13] && ((stageStatus[11] && clicnext && !waitAnswer) || stageStatus[12]);
        if (stageStatus[12]) { stage = 12; }
        stageStatus[11] = !stageStatus[12] && ((stageStatus[10] && clicnext && !waitAnswer) || stageStatus[11]);
        if (stageStatus[11]) { stage = 11; }
        stageStatus[10] = !stageStatus[11] && ((stageStatus[9] && clicnext) || stageStatus[10]);
        if (stageStatus[10]) { stage = 10; }
        stageStatus[9] = !stageStatus[10] && ((stageStatus[8] && clicnext && !waitAnswer) || stageStatus[9]);
        if (stageStatus[9]) { stage = 9; }
        stageStatus[8] = !stageStatus[9] && ((stageStatus[7] && clicnext && !waitAnswer) || stageStatus[8]);
        if (stageStatus[8]) { stage = 8; }
        stageStatus[7] = !stageStatus[8] && ((stageStatus[6] && clicnext && !waitAnswer) || stageStatus[7]);
        if (stageStatus[7]) { stage = 7; }
        stageStatus[6] = !stageStatus[7] && ((stageStatus[5] && clicnext && !waitAnswer) || stageStatus[6]);
        if (stageStatus[6]) { stage = 6; }
        stageStatus[5] = !stageStatus[6] && ((stageStatus[4] && clicnext && !waitAnswer) || stageStatus[5]);
        if (stageStatus[5]) { stage = 5; }
        stageStatus[4] = !stageStatus[5] && ((stageStatus[3] && clicnext) || stageStatus[4]);
        if (stageStatus[4]) { stage = 4; }
        stageStatus[3] = !stageStatus[4] && ((stageStatus[2] && clicnext && !waitAnswer) || stageStatus[3]);
        if (stageStatus[3]) { stage = 3; }
        stageStatus[2] = !stageStatus[3] && ((stageStatus[1] && clicnext && (IsConected()) && !waitAnswer) || stageStatus[2]);
        if (stageStatus[2]) { stage = 2; }
        stageStatus[1] = !stageStatus[2] && ((stageStatus[0] && clicnext ) || stageStatus[1]);
        if (stageStatus[1]) { stage = 1; }
        stageStatus[0] = !stageStatus[1] && (stageStatus[0]);
    }
    public void GrafcetGuia3()
    {
        /*
        stageStatus[15] = !stageStatus[16] && ((stageStatus[14] && clicnext) || stageStatus[15]);
        if (stageStatus[15]) { stage = 15; }
        stageStatus[14] = !stageStatus[15] && ((stageStatus[13] && clicnext) || stageStatus[14]);
        if (stageStatus[14]) { stage = 14; }
        stageStatus[13] = !stageStatus[14] && ((stageStatus[12] && clicnext && !waitAnswer) || stageStatus[13]);
        if (stageStatus[13]) { stage = 13; }
        stageStatus[12] = !stageStatus[13] && ((stageStatus[11] && clicnext && !waitAnswer) || stageStatus[12]);
        if (stageStatus[12]) { stage = 12; }
        stageStatus[11] = !stageStatus[12] && ((stageStatus[10] && clicnext && !waitAnswer) || stageStatus[11]);
        if (stageStatus[11]) { stage = 11; }
        stageStatus[10] = !stageStatus[11] && ((stageStatus[9] && clicnext) || stageStatus[10]);
        if (stageStatus[10]) { stage = 10; }
        stageStatus[9] = !stageStatus[10] && ((stageStatus[8] && clicnext && !waitAnswer) || stageStatus[9]);
        if (stageStatus[9]) { stage = 9; }
        stageStatus[8] = !stageStatus[9] && ((stageStatus[7] && clicnext && !waitAnswer) || stageStatus[8]);
        if (stageStatus[8]) { stage = 8; }
        stageStatus[7] = !stageStatus[8] && ((stageStatus[6] && clicnext && !waitAnswer) || stageStatus[7]);
        if (stageStatus[7]) { stage = 7; }
        stageStatus[6] = !stageStatus[7] && ((stageStatus[5] && clicnext && !waitAnswer) || stageStatus[6]);
        if (stageStatus[6]) { stage = 6; }
        stageStatus[5] = !stageStatus[6] && ((stageStatus[4] && clicnext && !waitAnswer) || stageStatus[5]);// finalizacionde la practica
        if (stageStatus[5]) { stage = 5; }*/
        stageStatus[5] = !stageStatus[6] && ((stageStatus[4] && clicnext) || stageStatus[5]);// organizacion de piesas
        if (stageStatus[5]) { stage = 5; }
        stageStatus[4] = !stageStatus[5] && ((stageStatus[3] && clicnext) || stageStatus[4]);// organizacion de piesas
        if (stageStatus[4]) { stage = 4; }
        stageStatus[3] = !stageStatus[4] && ((stageStatus[2] && clicnext &&  (currentTarget == 0)) || stageStatus[3]);// creacion de piesas
        if (stageStatus[3]) { stage = 3; }
        stageStatus[2] = !stageStatus[3] && ((stageStatus[1] && clicnext && (IsConected()) ) || stageStatus[2]);// pedir que se conecte 
        if (stageStatus[2]) { stage = 2; }
        stageStatus[1] = !stageStatus[2] && ((stageStatus[0] && clicnext) || stageStatus[1]); //introduccion
        if (stageStatus[1]) { stage = 1; }
        stageStatus[0] = !stageStatus[1] && (stageStatus[0]);
    }

    public void Guia1(int stage)// partes del  robot  --- pedir que este conectado al robot 
    {
        ContainerGuias[numberguide].gameObject.SetActive(true);
        for (int i = 0; i < LayerObjectGuia1.Length; i++)
        {
            LayerObjectGuia1[i].gameObject.SetActive(false);
        }

        // 1- enfocar al robot
        // 2- partes del robot 
        // 3- robot 
        // 4- estructura
        // 5- mesa de trabajo
        // 6- caja de control 
        // 7- cables de control
        // 8- Partes del robot delta
        // 9- triangulo fijo 
        // 10- actuadores 
        // 11- eslabon superior           
        // 12 - eslabon inferior 
        // 14-  triangulo movil
        // 15 - juntas (opcional )
        // 15 - 4to eje 
        // 16 - efector final 

        //------------------------------

        // 2- partes del robot 
        //es un Robot robot delta de uso didactico  para la ensenanza de robotica  mediante el uso de realidad aumentada, con un precicion de maxima de 1 cm
        if (stage == 1)
        {
            txtEtapa.text = "Partes de DeltaAR"; 
            txtDescipcion.text = "Las principales partes del robot son";
            LayerObjectGuia1[0].gameObject.SetActive(true);
        }
        // 3- robot 
        if (stage == 2)
        {
            txtEtapa.text = "Robot";
            txtDescipcion.text = "Robot paralelo de 4 grados de libertad";
            LayerObjectGuia1[0].gameObject.SetActive(false);
            LayerObjectGuia1[1].gameObject.SetActive(true);
        }
        // 4- estructura
        if (stage == 3)
        {
            txtEtapa.text = "Estructura"; 
            txtDescipcion.text = "Estructura metálica que sostiene al robot";
            LayerObjectGuia1[1].gameObject.SetActive(false);
            LayerObjectGuia1[2].gameObject.SetActive(true);
        }
        // 5- mesa de trabajo
        if (stage == 4)
        {
            txtEtapa.text = "Mesa de trabajo";
            txtDescipcion.text = "Área donde se colocan las cosas";
            LayerObjectGuia1[2].gameObject.SetActive(false);
            LayerObjectGuia1[3].gameObject.SetActive(true);
        }
        // 6- caja de control
        if (stage == 5)
        {
            txtEtapa.text = "Caja de control"; 
            txtDescipcion.text = "Se encuentran los componentes electrónicos";
            LayerObjectGuia1[3].gameObject.SetActive(false);
            LayerObjectGuia1[4].gameObject.SetActive(true);
        }
        // 7- cables de control
        if (stage == 6)
        {
            txtEtapa.text = "Cables de control"; 
            txtDescipcion.text = "Cables de los motores del robot";
            LayerObjectGuia1[4].gameObject.SetActive(false);
            LayerObjectGuia1[5].gameObject.SetActive(true);
        }
        // 8- Morfologia
        if (stage == 7)
        {
            txtEtapa.text = "Morfología"; 
            txtDescipcion.text = "Elementos constructivos que forma parte del robot";
            LayerObjectGuia1[5].gameObject.SetActive(false);
            LayerObjectGuia1[6].gameObject.SetActive(true);
        }
        // 9- Estructura mecanica
        if (stage == 8)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Conformado por eslabones y articulaciones";
            LayerObjectGuia1[6].gameObject.SetActive(false);
            LayerObjectGuia1[7].gameObject.SetActive(true);
        }
        // 10- Configuracion
        if (stage == 9)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[7].gameObject.SetActive(false);
            LayerObjectGuia1[8].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 11- Triangulo fijo
        if (stage == 10)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[8].gameObject.SetActive(false);
            LayerObjectGuia1[9].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 12- Antebrazo
        if (stage == 11)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[9].gameObject.SetActive(false);
            LayerObjectGuia1[10].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 13- brazo
        if (stage == 12)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[10].gameObject.SetActive(false);
            LayerObjectGuia1[11].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 14- TrianguloMovil
        if (stage == 13)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[11].gameObject.SetActive(false);
            LayerObjectGuia1[12].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 15- Articulacion
        if (stage == 14)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[12].gameObject.SetActive(false);
            LayerObjectGuia1[13].gameObject.SetActive(true);
            LayerObjectGuia1[21].gameObject.SetActive(true);
        }
        // 16- Cuarto eje
        if (stage == 15)
        {
            txtEtapa.text = "Estructura Mecánica";
            txtDescipcion.text = "Configuración de un robot delta";
            LayerObjectGuia1[13].gameObject.SetActive(false);
            LayerObjectGuia1[14].gameObject.SetActive(true);
        }
        // 17- Actuadores
        if (stage == 16)
        {
            txtEtapa.text = "Actuadores";
            txtDescipcion.text = "Proporciona movimiento al robot";
            LayerObjectGuia1[14].gameObject.SetActive(false);
            LayerObjectGuia1[15].gameObject.SetActive(true);
        }
        // 18- Sistema de transmision
        if (stage == 17)
        {
            txtEtapa.text = "Sistema de Transmisión";
            txtDescipcion.text = "Transmite movimiento del actuador al eslabón";
            LayerObjectGuia1[15].gameObject.SetActive(false);
            LayerObjectGuia1[16].gameObject.SetActive(true);
        }
        // 19- Sensores
        if (stage == 18)
        {
            txtEtapa.text = "Sensores";
            txtDescipcion.text = "Permite medir parámetro del robot";
            LayerObjectGuia1[16].gameObject.SetActive(false);
            LayerObjectGuia1[17].gameObject.SetActive(true);
        }
        // 20- Sistemas de control
        if (stage == 19)
        {
            txtEtapa.text = "Sistema de control";
            txtDescipcion.text = "Proporciona movimiento al actuador";
            LayerObjectGuia1[17].gameObject.SetActive(false);
            LayerObjectGuia1[18].gameObject.SetActive(true);
        }
        // 21- efector final 
        if (stage == 20)
        {
            txtEtapa.text = "Efector final";
            txtDescipcion.text = "Dispositivo que se une  a la muñeca del robot con el fin de realizar una tarea especifica";//puede ser un pinza o una herrmaineta(anctorchade soldatura, fresadora)
            LayerObjectGuia1[18].gameObject.SetActive(false);
            LayerObjectGuia1[19].gameObject.SetActive(true);
        }
        // 22- efector final 
        if (stage == 21)
        {
            txtEtapa.text = "Efector final";
            txtDescipcion.text = "TCP Tool CENTER POINT";
            LayerObjectGuia1[19].gameObject.SetActive(false);
            LayerObjectGuia1[20].gameObject.SetActive(true);
        }
        // 22- efector final 
        if (stage == 22)
        {
            txtEtapa.text = "Congratulations";
            txtDescipcion.text = "Guía terminada con éxito";
            LayerObjectGuia1[20].gameObject.SetActive(false);
            //LayerObjectGuia1[20].gameObject.SetActive(true);
        }
    }


    public void Guia2(int stage)// caracterirsticasdel robot ---  pidir que se conecte al robot 
    {
        ContainerGuias[numberguide].gameObject.SetActive(true);
        for (int i = 0; i < Guia2Elements.Length; i++)
        {
            Guia2Elements[i].gameObject.SetActive(false);
        }
        // 1 - enfocar al robot 
        if (stage == 1)
        {
            txtEtapa.text = "GDL-Grados de libertad";
            txtDescipcion.text = "Por favor conéctese al robot para continuar";
        }
        // 2 - Grados de libertad 
        if (stage == 2)
        {
            txtEtapa.text = "GDL-Grados de libertad";
            txtDescipcion.text = "Cada uno de los movimientos independientes ue puede realizar el robot";
            Guia2Elements[0].gameObject.SetActive(true);
            // se encuentr en path
            tipeJOGInterfase = 2;//PTP
            resolution_Value = 20;
        }
        // 3- Movimiento en x
        if (stage == 3)
        {
            txtEtapa.text = "GDL-Grados de libertad";
            txtDescipcion.text = "Movimiento en X. Pulse en la flecha para mover";
            Guia2Elements[1].gameObject.SetActive(true);
            tipeJOGInterfase = 2;//PTP
            resolution_Value = 20;
        }
        // 4- Movimiento en y
        if (stage == 4)
        {
            txtEtapa.text = "GDL-Grados de libertad ";
            txtDescipcion.text = "Movimiento en Y. Pulse en la flecha para mover";
            Guia2Elements[2].gameObject.SetActive(true);
            tipeJOGInterfase = 2;//PTP
            resolution_Value = 20;
        }
        // 5- Movimiento en z
        if (stage == 5)
        {
            txtEtapa.text = "GDL-Grados de libertad ";
            txtDescipcion.text = "Movimiento en Z. Pulse en la flecha para mover";
            Guia2Elements[3].gameObject.SetActive(true);
            tipeJOGInterfase = 2;//PTP
            resolution_Value = 20;
        }
        // 6- Movimiento en C
        if (stage == 6)
        {
            txtEtapa.text = "GDL-Grados de libertad ";
            txtDescipcion.text = "Movimiento en C. Pulse en la flecha para mover";
            Guia2Elements[4].gameObject.SetActive(true);
            tipeJOGInterfase = 2;//PTP
            resolution_Value = 20;
        }
        // 7- Volumen de trabajo
        if (stage == 7)
        {
            txtEtapa.text = "Volumen de trabajo";
            txtDescipcion.text = "Espacio al cual puede desplazarse el extremo de su muñeca";
            Guia2Elements[5].gameObject.SetActive(true);
        }
        // 8- Sistema de cordenadas wold
        if (stage == 8)
        {
            txtEtapa.text = "Sistema de coordenadas World";
            txtDescipcion.text = "Sistema de coordenadas cartesianas situado en un punto fijo";
            Guia2Elements[6].gameObject.SetActive(true);
        }

        // 9-  tipos de movimiento del robot 
        if (stage == 9)
        {
            txtEtapa.text = "Tipos de movimiento del robot"; 
            txtDescipcion.text = "Para programación de trayectorias";
            BtnPlay.gameObject.SetActive(false);
        }
        // 10- movimiento lineal 
        if (stage == 10)
        {
            txtEtapa.text = "Movimiento LIN ";
            txtDescipcion.text = "Los ejes del robot se coordinan entre sí de tal manera que el TCP, se mueve a lo largo de una recta desde un punto de origen a un punto de destino";
            BtnPlay.gameObject.SetActive(true);
            Guia2Elements[7].gameObject.SetActive(true);


        }
        // 11- Movimiento  ptp
        if (stage == 11)//dibujar la curva
        {
            txtEtapa.text = "Movimiento  PTP ";
            txtDescipcion.text = "Trayectoria más rápida entre 2 puntos, no es lineal";
            //desplaza el TCP(Punto de trabjao de la herramienta) al punto de destino a lo largo de la trayectoria mas rapida , la trayectoria mas rapidano es, 
            //en regla general  la trayectoria mas corta ypor ello no es una recta,dado que lo ejes del robot se mueven de forma rotacional la trayectorias curvas 
            //pueden ejecutarse se forma mas rapida  que las rectas, no puede predecirce la trayectoria exacta
            NetworkManager.instance.SendDataServer("PATH");
            waitAnswer = true;
            BtnPlay.gameObject.SetActive(true);
            Guia2Elements[8].gameObject.SetActive(true);
        }
        if (stage==12)
        {
            txtEtapa.text = "Congratulations";
            txtDescipcion.text = "Guía terminada con éxito";
            BtnPlay.gameObject.SetActive(false);
        }
        // 5 - cinematia inversa (Proximamente )
        //NetworkManager.instance.SendDataServer("JOG");
        //tipeJOGInterfase = 0;//xyz
        // 6- cinematica directa (Proximamente )
        //tipeJOGInterfase = 2;//angulo


    }

    public void Guia3(int stage)// aplicativo
    {
        ContainerGuias[numberguide].gameObject.SetActive(true);
        // como  se mueve los motores ( xyz o angulo) 
        // como guardar los puntos 
        // como guardar y como abrir un programa 
        // como ejectuar un programa 
        if (stage == 1)
        {
            txtEtapa.text = "Guía de Aplicación";
            txtDescipcion.text = " Conéctese al robot para continuar ";
            //NetworkManager.instance.SendDataServer("PATH");
            //                                 lineal /x /y  /z  /c/Griper
            //NetworkManager.instance.SendDataServer("0 -150 0 -350 0 0 ");
        }

        if (stage == 2)
        {
            txtEtapa.text = "Enfoque al robot ";
            txtDescipcion.text = "Para comenzar la aplicación ";
            //NetworkManager.instance.SendDataServer("PATH");
            //                                 lineal /x /y  /z  /c/Griper
            //NetworkManager.instance.SendDataServer("0 -150 0 -350 0 0 ");
        }
        if (stage == 3)
        {
            txtEtapa.text = "Pick and Place ";
            txtDescipcion.text = "Enfoque al robot para continuar ";
            BtnPlay.gameObject.SetActive(false);
            //NetworkManager.instance.SendDataServer("PATH");
            //                                 lineal /x /y  /z  /c/Griper
            //NetworkManager.instance.SendDataServer("0 -150 0 -350 0 0 ");
        }
        if (stage == 4)
        {
            txtEtapa.text = "Pick and Place ";
            txtDescipcion.text = "Haga clic en Play para ejecutar la aplicación ";
            BtnPlay.gameObject.SetActive(true);

        }
        if (stage == 5)
        {
            txtEtapa.text = "Congratulations ";
            txtDescipcion.text = "Guía terminada con éxito";
            BtnPlay.gameObject.SetActive(false);
        }
    }

    #endregion

    public void ChangeRobotValue(int valuechage)// Value eje , value () if +1 or -1
    {
        // solo para enviar datos por JOG  no tiene tipod e movimiento todo es ptp

        //encuentra que eje es 
        int value;
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
            currentValueaxis[valuechage] = currentValueaxis[valuechage] + resolution_Value * (value);
            ShowDataValue();
        }

        // preproces value for send
        string sendData = "";
        if (tipeJOGInterfase == 2) // Caundo esta en angulo//Ptp
        {
            sendData = "0 ";
        }
        else
        {
            sendData = "1 "; // caundo esta en xyz//Line
        }

        foreach (float element in currentValueaxis)
        {
            sendData = sendData + element + " ";
        }

        Debug.Log("Data send " + sendData);
        // send value 
        NetworkManager.instance.SendDataServer(sendData);
        Debug.Log("recived position:" + NetworkManager.instance.ReciveDataServer());
    }
    public void ShowDataValue()
    {

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
                Debug.Log("caso 0");
                // pedir valor de xyz
                NetworkManager.instance.SendDataServer("XYZ");
                // recivir currentValueaxis[] con los valores de los angulos
                do
                {
                    data = NetworkManager.instance.ReciveDataServer();
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

                Debug.Log("caso 1");
                break;
            case 2:
                Debug.Log("caso 2");
                // pedir el angulo al servidor
                NetworkManager.instance.SendDataServer("Angle");
                // recivir currentValueaxis[] con los valores de los angulos
                do
                {
                    data = NetworkManager.instance.ReciveDataServer();
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
}
