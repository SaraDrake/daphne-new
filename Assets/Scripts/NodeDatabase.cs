using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeDatabase : MonoBehaviour
{
    public List<NodeData> Nodes = new List<NodeData>();

    public NodeData FindNode(string id)
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (Nodes[i].NodeName == id)
            {
                return Nodes[i];
            }
        }

        throw new Exception("Can't find node with the ID: " + id);
    }
}

[Serializable]
public class NodeData
{
    public string NodeName;
    public List<float> SubtitleTimestamps;
    public List<EmoteData> Emotes;

    public NodeData(string name)
    {
        NodeName = name;
    }
}

[Serializable]
public class EmoteData : IComparable<EmoteData>
{
    public float TimeStamp;
    public String Animation;

    public int CompareTo(EmoteData other)
    {
        if (other == null) return 1;
        
        return TimeStamp.CompareTo(other.TimeStamp);
    }
}
