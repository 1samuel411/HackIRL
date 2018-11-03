using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlertModel : Model
{
    public Sprite image;
    public string name;
    public int severity;
    public int animalType;

    public AlertModel ()
    {
        name = "Anonymous";
        severity = 0;
        animalType = 0;
    }
}
