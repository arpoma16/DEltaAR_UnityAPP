using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;            // Save string  
using SimpleFileBrowser;    //https://github.com/yasirkula/UnitySimpleFileBrowser
public class ListManager : MonoBehaviour
{
    
    public GameObject PointTemplate;
    private List<GameObject> PointItems;

    // lista de punto salvar
    string SAVE_FILE = "/SAvEGAME";
    string FILE_EXTENSION = ".Gtxt";

    


    void Start()
    {
        Debug.Log("start  list Manager");
        PointItems = new List<GameObject>();

        // File browser
        //https://github.com/yasirkula/UnitySimpleFileBrowser

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Code", ".Gtxt", ".txt"), new FileBrowser.Filter("Text Files", ".txt", ".pdf"));
        FileBrowser.SetDefaultFilter(".Gtxt");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe",".jpg",".png");

    }

    public void LogPointContend(string newTextString, bool typeOfMoviment, float[] valueaxis)
    {
        //https://www.youtube.com/watch?v=ZI6DwJtjlBA&t=5s

        
        GameObject newPoint = Instantiate(PointTemplate) as GameObject;
        newPoint.SetActive(true);

        newPoint.GetComponent<PointatList>().SetPointContend(newTextString, typeOfMoviment, valueaxis);
        newPoint.transform.SetParent(PointTemplate.transform.parent, false);

        PointItems.Add(newPoint.gameObject);

    }
    public void DeleteITEM(GameObject tempItem)
    {
        Debug.Log("Total elements: " + PointItems.Count);
        Destroy(tempItem.gameObject);
        PointItems.Remove(tempItem);
        Debug.Log("Current elements: " + PointItems.Count);
    }


    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, null, "Load File", "Select");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        //PointItems.Clear();

        string[] parts;
        bool auxbool;
        float[] currentValueaxis = new float[] { 0f, 0f, -300f, 0f, 0f };
        if (FileBrowser.Success)
        {
            using (FileStream fs = new FileStream(FileBrowser.Result, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        //list.Add(line); // Add to list.
                        Debug.Log(line);
                        parts = line.Split(' ');
                        if (parts[1]=="0")
                        {
                            auxbool = true;
                        }
                        else
                        {
                            auxbool = false;
                        }
                        currentValueaxis[0] = float.Parse(parts[2]);
                        currentValueaxis[1] = float.Parse(parts[3]);
                        currentValueaxis[2] = float.Parse(parts[4]);
                        currentValueaxis[3] = float.Parse(parts[5]);
                        currentValueaxis[4] = float.Parse(parts[6]);
                        LogPointContend(parts[0].Replace(" ", "_"), auxbool, currentValueaxis);

                    }
                }
            }
        }
    }

    IEnumerator ShowSaveDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForSaveDialog(false, null, "Save File", "Save");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);
        string data = "";
        // recorrer todos los puntos creados y guardar el archivo
        
        if (FileBrowser.Success)
        {
            //https://www.dotnetperls.com/streamreader
            if (FileBrowser.Success)
            {
                using (FileStream fs = new FileStream(FileBrowser.Result, FileMode.Create, FileAccess.Write))
                {
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        foreach (GameObject element in PointItems)
                        {
                            data = element.GetComponent<PointatList>().SendValuePoint(0);
                            writer.WriteLine(data);
                        }
                    }
                }
            }
        }
    }


    public void SaveProgram()
    {
        StartCoroutine(ShowSaveDialogCoroutine());
        Debug.Log("termine la  courouine");
    }

    public void LoadProgram()
    {
        //StartCoroutine(ShowLoadDialogCoroutine());
        StartCoroutine(ShowLoadDialogCoroutine());
        Debug.Log("termine la  courouine");
    }

    public string[] GetListPointString()
    {
        string[] points = new string[PointItems.Count];
        int count = 0;
        foreach (GameObject element in PointItems)
        {
            points[count] = element.GetComponent<PointatList>().SendValuePoint(1);
            count++;
        }
        return points;
    }


}
