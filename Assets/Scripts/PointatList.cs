using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointatList : MonoBehaviour
{
    public ListManager logControl;
    public Text namePoint;
    public Text ShowSpeedValue;
    public Image imagetypeMove;
    public Sprite lineapoint;
    public Sprite PtPpoint;
    private float[] value = new float[5];
    private bool typeMove;
    public GameObject Buttontemplate;// el boton para eliminar y modificar el punto 
    private GameObject buttonactive;// la instancia del boton que se encuentra arriba


    private void Start()
    {
        buttonactive = Instantiate(Buttontemplate) as GameObject;

    }
    public void ActiveButton()
    {
        buttonactive.SetActive(true);
        buttonactive.GetComponent<ButtonPoint>().SetButtonObj(this.gameObject);
        buttonactive.transform.SetParent(Buttontemplate.transform.parent, false);
    }


    public void SetPointContend(string newTextString, bool typeOfMoviment, float[] valueAxis)
    {
        namePoint.text = newTextString;
        value[0] = valueAxis[0];
        value[1] = valueAxis[1];
        value[2] = valueAxis[2];
        value[3] = valueAxis[3];
        value[4] = valueAxis[4];
        ShowSpeedValue.text = "10%";
        typeMove = typeOfMoviment;
        if (typeMove)
        {
            imagetypeMove.GetComponent<Image>().sprite = lineapoint;
        }
        else
        {
            imagetypeMove.GetComponent<Image>().sprite = PtPpoint;
        }
        
    }
    public string SendValuePoint(int option)
    {
        /// if = 1  then  send   without  name  / para envio al servidor
        /// if = 0  then  send   with  name  / para envio para guardar dato
        string sendData = "";
        if (option == 0)
        {
            sendData = namePoint.text.Replace(" ", "_") + " ";
        }
        
        if (typeMove) // Caundo es movimiento lineal
        {
            sendData = sendData + "1 ";
        }
        else
        {
            sendData = sendData + "0 "; // caundo es PTP
        }

        foreach (float element in value)
        {
            sendData = sendData + element + " ";
        }

        return sendData;
    }
}
