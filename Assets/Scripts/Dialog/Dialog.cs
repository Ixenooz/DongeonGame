using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]

public class Dialog
{
    [SerializeField] List<string> lines;

    public List<string> Lines {
        get{ return lines; }
    }
    // Add a constructor that accepts a list of strings
    public Dialog(List<string> lines)
    {
        this.lines = lines;
    }
}
