using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonPoint : MonoBehaviour
{
    //--------------*************************
    // -----------este es el botton del punto
    //----------------***********************

    private GameObject Pointyactive = null;
    public ListManager logControl;
    public Image PositionBtn;

    public void SetButtonObj(GameObject Pointacti )
    {
        Pointyactive = Pointacti;
        //PositionBtn.transform.position = Pointacti.transform.position;
        PositionBtn.transform.position = new Vector3(Pointacti.transform.position.x+200, Pointacti.transform.position.y, Pointacti.transform.position.z);
        //Debug.Log("x:" + Pointacti.transform.position.x + " y:" + Pointacti.transform.position.y+" z:" + Pointacti.transform.position.z);
    }
    public void DeleteObj()
    {
        logControl.DeleteITEM(Pointyactive.gameObject);
        Destroy(this.gameObject);
    }
    public void ModObj()
    {
        //  Poner para modificar el objeto
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
