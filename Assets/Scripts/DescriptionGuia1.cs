using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionGuia1 : MonoBehaviour
{
    [TextArea]
    public string Detalles;

    public Sprite imgDetalles;
    public int valor;

    public string GetDetalles()
    {
        string aux = string.Empty;
        aux = Detalles.Replace("\\n", "\n");
        return aux;
    }
}
