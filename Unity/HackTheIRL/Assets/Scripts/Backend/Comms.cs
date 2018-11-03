using Newtonsoft.Json;
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

    public delegate void GetImageCallback(Texture2D img);
    public void GetImage(string url, GetImageCallback callback)
    {
        StartCoroutine(GetImageCoroutine(url, callback));
    }
    IEnumerator GetImageCoroutine(string url, GetImageCallback callback)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        callback(texture);
    }

    public delegate void GetAlertsCallback(AlertModel[] alerts);
    public void GetAlerts(float lat, float lon, GetAlertsCallback callback)
    {
        StartCoroutine(GetAlertsCoroutine(lat, lon, callback));
    }
    IEnumerator GetAlertsCoroutine(float lat, float lon, GetAlertsCallback callback)
    {
        WWWForm form = new WWWForm();
        WWW www = new WWW(backendURL + "/API/GetAlerts", form);
        yield return www;

    }

    public delegate void SubmitFormCallback();
    public void SubmitForm(AlertModel model, SubmitFormCallback callback)
    {
        StartCoroutine(SubmitFormCoroutine(model, callback));
    }
    IEnumerator SubmitFormCoroutine(AlertModel model, SubmitFormCallback callback)
    {
        // Upload image
        WWWForm form = new WWWForm();
        form.AddBinaryData("Image", model.image.EncodeToPNG());
        WWW www = new WWW(backendURL + "/API/UploadImage", form);
        yield return www;
        model.imageUrl = www.text;

        // Upload alert
        form = new WWWForm();
        form.AddField("imageUrl", model.imageUrl);
        form.AddField("name", model.imageUrl);
        form.AddField("type", model.type);
        form.AddField("severity", model.imageUrl);
        form.AddField("animalType", model.imageUrl);
        www = new WWW(backendURL + "/API/SubmitForm", form);
        yield return www;
        callback();
    }

}
