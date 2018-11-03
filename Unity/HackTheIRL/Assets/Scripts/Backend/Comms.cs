using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comms : MonoBehaviour
{

    public static Comms instance;
    public const string backendURL = "localhost:5000";

    private void Start()
    {
        instance = this;
    }

    public delegate void GetImageCallback(Sprite img);
    public void GetImage(string url, GetImageCallback callback)
    {
        StartCoroutine(GetImageCoroutine(url, callback));
    }
    IEnumerator GetImageCoroutine(string url, GetImageCallback callback)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
        callback(newSprite);
    }

    public delegate void GetAlertsCallback(AlertModel[] alerts);
    public void GetAlerts(float lat, float lon, GetAlertsCallback callback)
    {
        StartCoroutine(GetAlertsCoroutine(lat, lon, callback));
    }

    IEnumerator GetAlertsCoroutine(float lat, float lon, GetAlertsCallback callback)
    {
        WWWForm form = new WWWForm();
        WWW www = new WWW(backendURL + "/GetAlerts", form);
        yield return www;

    }

}
