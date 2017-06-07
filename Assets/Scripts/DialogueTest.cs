using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DialogueTest : MonoBehaviour
{
    public Canvas DialogueCanvas;
    public Canvas DebugCanvas;
    public GameObject FileBrowser;

    public Yarn.Unity.DialogueRunner runner;
    public GameObject ListParent;
    public GameObject ListItem;

    private List<Button> ListofMenuItems = new List<Button>();

    private string path;
    private string[] filePaths;


    // Use this for initialization
    void Start ()
    {
        path = PlayerPrefs.GetString("path");

        LoadAssets();
        PopulateList();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            FileBrowser.gameObject.SetActive(true);
        }
	}

    

    public void DirectoryUpdate(string dir)
    {
        path = dir;
        Debug.Log(path);
        PlayerPrefs.SetString("path", path);
    }

    public void RefreshAllNodesFromDisk()
    {
        LoadAssets();
        PopulateList();
    }

    void LoadAssets()
    {
        filePaths = System.IO.Directory.GetFiles(path, "*.json");
        Debug.Log(path);
        runner.Clear();
        foreach (string file in filePaths)
        {
            Debug.Log(file);
            runner.dialogue.LoadFile(file);
        }
    }

    void PopulateList()
    {
        ClearList();

        List<string> nodes = new List<string>(runner.dialogue.allNodes);
        Button button;

        for (int i = 0; i < nodes.Count; i++)
        {
            string node = nodes[i];
            button = Instantiate(ListItem).GetComponent<Button>();
            button.transform.SetParent(ListParent.transform, false);
            button.GetComponentInChildren<Text>().text = node;
            button.onClick.AddListener(() => { NodeButtonOnClick(node); });
            ListofMenuItems.Add(button);
        }
    }

    public void NodeButtonOnClick(string node)
    {
        Debug.Log(node);
        runner.StartDialogue(node);
        DebugCanvas.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        DebugCanvas.gameObject.SetActive(true);

    }

    void ClearList()
    {
        for (int i = ListofMenuItems.Count-1; i >= 0; i--)
        {
            Destroy(ListofMenuItems[i].gameObject);
        }

        ListofMenuItems.Clear();
    }

}
  