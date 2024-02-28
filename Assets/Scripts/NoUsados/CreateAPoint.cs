using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CreateAPoint : MonoBehaviour
{
    [SerializeField]
    private string NamePoint;
    private Color mycolor;
    private ListManager logControl;
    private bool _trayectoryPTP = true;

    public Slider SliderSpeed;
    public Text TextSliderSpeed;

    public void LogPoint()
    {
        // save a point 
       // logControl.LogPointContend(NamePoint, mycolor);
    }
    
    public void TrayectoryPTP()
    {
        _trayectoryPTP = true;
    }
    public void TrayectoryLine()
    {
        _trayectoryPTP = false;
    }


    /// mostrar el porsentaje de la velocidad
    /// mostrar seleccionar la velocidad
}