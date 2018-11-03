using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlertModel : Model
{
    public int id;
    public Texture2D image;
    public string imageUrl;
    public string name;
    public int type;
    public int severity;
    public int animalType;

    public AlertModel ()
    {
        name = "Anonymous";
        severity = 0;
        animalType = 0;
    }
}
