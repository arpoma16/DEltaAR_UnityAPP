using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour
{
    // robot 
    public Transform BrazoSup1;
    public Transform BrazoSup2;
    public Transform BrazoSup3;
    public Transform BrazoInf1;
    public Transform BrazoInf2;
    public Transform BrazoInf3;
    public Transform Eslabon1;
    public Transform Eslabon2;
    public Transform Eslabon3;
    public Transform Eslabon4;
    public Transform Eslabon5;
    public Transform Eslabon6;
    public Transform BrazoInf1final;
    public Transform BrazoInf2final;
    public Transform BrazoInf3final;


    private Quaternion QuaBrazoSup1;
    private Quaternion QuaBrazoSup2;
    private Quaternion QuaBrazoSup3;
    private Quaternion QuaBrazoInf1;
    private Quaternion QuaBrazoInf2;
    private Quaternion QuaBrazoInf3;
    private Quaternion QuaEslabon1;
    private Quaternion QuaEslabon2;
    private Quaternion QuaEslabon3;
    private Quaternion QuaEslabon4;
    private Quaternion QuaEslabon5;
    private Quaternion QuaEslabon6;

    //Gripper

    public Transform Efectorfinal;
    public Transform CuartotoEje;
    public Transform gripperPinza1;
    public Transform gripperPinza2;
    public Transform gripperEngrane1;
    public Transform gripperEngrane2;
    public Transform gripperEslabon1;
    public Transform gripperEslabon2;
    public Transform referencePinzagripper;

    private Quaternion initialrotation;
    private Quaternion initrotationGripperEngrane1;
    private Quaternion initrotationGripperEngrane2;
    private Quaternion initrotationGripperEslabon1;
    private Quaternion initrotationGripperEslabon2;
    private Vector3 initpinza1, initpinza2;
    private float angulo=0f;
    public Material transparenica;

    public void Start()
    {
        BrazoSup1.localRotation = BrazoSup1.localRotation * Quaternion.AngleAxis(5f, Vector3.right);
        BrazoSup2.localRotation = BrazoSup2.localRotation * Quaternion.AngleAxis(5f, Vector3.right);
        BrazoSup3.localRotation = BrazoSup3.localRotation * Quaternion.AngleAxis(5f, Vector3.right);
        QuaBrazoSup1 = BrazoSup1.localRotation;
        QuaBrazoSup2 = BrazoSup2.localRotation;
        QuaBrazoSup3 = BrazoSup3.localRotation;
        QuaEslabon1 = Eslabon1.localRotation;
        QuaEslabon2 = Eslabon1.localRotation;

        BrazoInf1.localRotation = BrazoInf1.localRotation * Quaternion.AngleAxis(-5f, Vector3.right);
        BrazoInf2.localRotation = BrazoInf2.localRotation * Quaternion.AngleAxis(-5f, Vector3.right);
        BrazoInf3.localRotation = BrazoInf3.localRotation * Quaternion.AngleAxis(-5f, Vector3.right);

        QuaBrazoInf1 = BrazoInf1.localRotation ;
        QuaBrazoInf2 = BrazoInf2.localRotation ; 
        QuaBrazoInf3 = BrazoInf3.localRotation ; 

        initialrotation = CuartotoEje.transform.localRotation;
        initrotationGripperEngrane1 = gripperEngrane1.transform.localRotation;
        initrotationGripperEngrane2 = gripperEngrane2.transform.localRotation;
        initrotationGripperEslabon1 = gripperEslabon1.transform.localRotation;
        initrotationGripperEslabon2 = gripperEslabon2.transform.localRotation;
        //Vector3 punto1 = referencePinzagripper.transform.TransformPoint(gripperPinza1.transform.position);
        //initpinza1 = gripperEngrane1.transform.InverseTransformPoint(punto1);
        initpinza1 = new Vector3(-0.0045f, -0.01411f, 0.0194f);
        initpinza2 = new Vector3(0.0013f, -0.01302f, 0.02026f);
        Debug.Log(BrazoSup1.transform.localRotation);
        MoveGripperTo(new Vector3(0f, -0.300f, 0f));
    }

    public void  HidevirtualRobot()
    {
        Renderer[] children;
        children =gameObject.GetComponentsInChildren<Renderer>();
        
        foreach (Renderer rend in children)
        {
            rend.enabled = false;
            
        }

    }
    public void TransparencevirtualRobot()
    {
        Renderer[] children;
        children = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer rend in children)
        {
            var color = rend.material.color;
            var newColor = new Color(color.r, color.g, color.b, 0.5f);
            rend.material = transparenica;
            rend.material.SetColor("_Color", newColor);
        }
    }

    public float GetAngleGripper()
    {
        return angulo;
    }

    public Vector3 Getposition()
    {
        return new Vector3(-Efectorfinal.transform.localPosition.x, Efectorfinal.transform.localPosition.y, -Efectorfinal.transform.localPosition.z);
    }



    public void RotateGripper(float ofset)
    {
        //Debug.Log("Angulo de  griper");
        //Debug.Log(ofset);
        CuartotoEje.transform.localRotation = initialrotation * Quaternion.AngleAxis(-ofset, Vector3.up);

    }
    public void OpenGripper(float angle)
    {
        angulo = angle;
        gripperEngrane1.transform.localRotation = initrotationGripperEngrane1 * Quaternion.AngleAxis(-angle, Vector3.right);
        gripperEngrane2.transform.localRotation = initrotationGripperEngrane2 * Quaternion.AngleAxis(-angle, Vector3.right);
        gripperEslabon1.transform.localRotation = initrotationGripperEslabon1 * Quaternion.AngleAxis(angle, Vector3.forward);
        gripperEslabon2.transform.localRotation = initrotationGripperEslabon2 * Quaternion.AngleAxis(angle, Vector3.forward);
        
        //Vector3 punto1 = gripperEngrane1.transform.TransformPoint(initpinza1);
        //gripperPinza1.transform.position = referencePinzagripper.transform.InverseTransformPoint(punto1);
        gripperPinza1.transform.position = gripperEngrane1.transform.TransformPoint(initpinza1);
        gripperPinza2.transform.position = gripperEngrane2.transform.TransformPoint(initpinza2);

    }
    public void MoveGripperTo(Vector3 posicion)
    {
        Efectorfinal.transform.localPosition = new Vector3(-posicion.x,posicion.y,-posicion.z);
        MovePosition(posicion);
    }

    public void MovePosition(Vector3 posicion)
    {
        //Debug.Log("posicion => "+posicion.ToString("F4"));
        Vector4 Angulos = Kinematics.Inverse(posicion.x, posicion.z, posicion.y);
        float sin120 = Mathf.Sqrt(3.0f) / 2.0f;
         float cos120 = -0.5f;
        if (Angulos.x == 0)
        {
            //Debug.Log("Angulos  => " +Angulos.ToString("F4"));
            //TriangleInf.position = new Vector3(float.Parse(Valuex.text) * (-0.001f), float.Parse(Valuez.text) * (-0.001f) - 0.02f, float.Parse(Valuey.text) * (0.001f));
            BrazoSup1.localRotation = QuaBrazoSup1 * Quaternion.AngleAxis(-Angulos.y, Vector3.right);
            BrazoSup2.localRotation = QuaBrazoSup2 * Quaternion.AngleAxis(-Angulos.z, Vector3.right);
            BrazoSup3.localRotation = QuaBrazoSup3 * Quaternion.AngleAxis(-Angulos.w, Vector3.right);
            Vector4 BrazoInf;


            BrazoInf = Kinematics.AngleBrazoInf(Angulos.y, posicion.x, posicion.z, posicion.y);

            //Debug.Log("Angulos inf  => "+BrazoInf.ToString("F4"));
            BrazoInf1.localRotation = QuaBrazoInf1 * Quaternion.AngleAxis(Angulos.y + BrazoInf.x, Vector3.right);
            Eslabon1.localRotation = QuaEslabon1 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            Eslabon2.localRotation = QuaEslabon2 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            BrazoInf1final.transform.localPosition = new Vector3(BrazoInf.z, 0.342f- BrazoInf.w, BrazoInf1final.transform.localPosition.z);

            BrazoInf = Kinematics.AngleBrazoInf(Angulos.z, posicion.x * cos120+ posicion.z * sin120, posicion.z * cos120- posicion.x * sin120, posicion.y);

            BrazoInf2.localRotation = QuaBrazoInf2 * Quaternion.AngleAxis(Angulos.z + BrazoInf.x, Vector3.right);
            Eslabon3.localRotation = QuaEslabon1 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            Eslabon4.localRotation = QuaEslabon2 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            BrazoInf2final.transform.localPosition = new Vector3(BrazoInf.z, 0.342f - BrazoInf.w, BrazoInf2final.transform.localPosition.z);
            
            BrazoInf = Kinematics.AngleBrazoInf(Angulos.w, posicion.x * cos120 - posicion.z * sin120, posicion.z * cos120 + posicion.x * sin120, posicion.y);

            BrazoInf3.localRotation = QuaBrazoInf3 * Quaternion.AngleAxis(Angulos.w + BrazoInf.x, Vector3.right);
            Eslabon5.localRotation = QuaEslabon1 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            Eslabon6.localRotation = QuaEslabon2 * Quaternion.AngleAxis(BrazoInf.y, Vector3.right);
            BrazoInf3final.transform.localPosition = new Vector3(BrazoInf.z, 0.342f - BrazoInf.w, BrazoInf1final.transform.localPosition.z);

        }

    }
}
