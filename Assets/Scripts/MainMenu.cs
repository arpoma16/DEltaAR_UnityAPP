using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public InputField iPText;
    public InputField portText;
    string ipValue;
    string portValue;
    public GameObject ConectedButton;


    void Awake()
    {
        Debug.Log("Awake called.");
    }

    // Use this for initialization
    void Start ()
    {
        IsConected();
        Debug.Log("Start Main Menu");
        ipValue = PlayerPrefs.GetString("IPvalue","192.168.0.1");
        iPText.text = ipValue;

        portValue = PlayerPrefs.GetString("Portvalue","5001");
        portText.text = portValue;

    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    public void IsConected()
    {
        if (NetworkManager.instance.socketState())
        {
            ConectedButton.gameObject.SetActive(false);
        }

    }
    public void Conect()
    {

        NetworkManager.instance.Connect();
    }
    public void Disconect()
    {
        NetworkManager.instance.ClosetConection();
    }

    public void ControlRobot()
    {
        SceneManager.LoadScene(1);
    }
    public void Guia1Scena()
    {
        PlayerPrefs.SetInt("Guide", 0);
        SceneManager.LoadScene(2);
    }
    public void Guia2Scena()
    {
        PlayerPrefs.SetInt("Guide", 1);
        SceneManager.LoadScene(2);
    }
    public void Guia3Scena()
    {
        PlayerPrefs.SetInt("Guide", 2);
        SceneManager.LoadScene(2);
    }
    public void Guia4Scena()//Aplicacion
    {
        PlayerPrefs.SetInt("Guide", 3);
        SceneManager.LoadScene(2);
    }

    public void SalirAppp()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

    public void  SetgetIP_Port()
    {
        ipValue = iPText.text;
        PlayerPrefs.SetString("IPvalue", ipValue);
        portValue = portText.text;
        PlayerPrefs.SetString("Portvalue", portValue);
        Debug.Log("ip "+ PlayerPrefs.GetString("IPvalue") + " puerto"+ PlayerPrefs.GetString("Portvalue"));
    }

    public bool CheckPortValid(string strPort)
    {
        try
        {
            int temp;
            temp = int.Parse(strPort);
        }
        catch
        {
            return false;
        }
        return true;
    }
    public bool CheckIPValid(string strIP)
    {
        //  Split string by ".", check that array length is 3
        char chrFullStop = '.';
        string[] arrOctets = strIP.Split(chrFullStop);
        if (arrOctets.Length != 4)
        {
            return false;
        }
        //  Check each substring checking that the int value is less than 255 and that is char[] length is !> 2
        int MAXVALUE = 255;
        int temp; // Parse returns Int32
        foreach (string strOctet in arrOctets)
        {
            if (strOctet.Length < 1 || strOctet.Length > 3)
            {
                return false;
            }
            try
            {
                temp = int.Parse(strOctet);
                if (temp > MAXVALUE)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }
        return true;
    }
}
