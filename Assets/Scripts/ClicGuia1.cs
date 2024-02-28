using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class ClicGuia1 : MonoBehaviour
{
    public GameObject ShowDescription;
    public Text txtTitulo;
    public Text txtDetalles;
    public Image imgenDetalle;
    public ARmanager ARcontroler;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray,out hit))
            {
                BoxCollider bc = hit.collider as BoxCollider;
                if (bc != null)
                {
                    if (bc.gameObject.GetComponent<DescriptionGuia1>().valor == 0)
                    {
                        ShowDescription.gameObject.SetActive(true);
                        txtTitulo.text = bc.gameObject.GetComponent<TextMeshPro>().text;
                        txtDetalles.text = bc.gameObject.GetComponent<DescriptionGuia1>().GetDetalles();
                        imgenDetalle.gameObject.SetActive(false);
                        if (bc.gameObject.GetComponent<DescriptionGuia1>().imgDetalles != null)
                        {
                            imgenDetalle.sprite = bc.gameObject.GetComponent<DescriptionGuia1>().imgDetalles;
                            imgenDetalle.gameObject.SetActive(true);
                        }
                    }
                    if (bc.gameObject.GetComponent<DescriptionGuia1>().valor >0 && bc.gameObject.GetComponent<DescriptionGuia1>().valor <= 4)
                    {
                        ARcontroler.ChangeRobotValue(bc.gameObject.GetComponent<DescriptionGuia1>().valor);
                    }
                    if (bc.gameObject.GetComponent<DescriptionGuia1>().valor >= -4 && bc.gameObject.GetComponent<DescriptionGuia1>().valor <= -1)
                    {
                        ARcontroler.ChangeRobotValue(bc.gameObject.GetComponent<DescriptionGuia1>().valor);
                    }

                }
            }
        }
    }
}
