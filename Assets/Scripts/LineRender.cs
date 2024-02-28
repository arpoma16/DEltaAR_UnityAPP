using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRender : MonoBehaviour
{
    public GameObject[] go_raw; //objetos de los cuales se va ahacer la linea
    private LineRenderer lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        drawLines();
    }
    // Draw Lines for Line Renderer
    private void drawLines()
    {
        // Set positions size
        lineRenderer.positionCount = go_raw.Length;

        // Set all line psitions
        for (int i = 0; i < go_raw.Length; i++)
        {
            lineRenderer.SetPosition(i, new Vector3(go_raw[i].transform.position.x, go_raw[i].transform.position.y, go_raw[i].transform.position.z));
        }
    }
}
