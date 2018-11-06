using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comms : MonoBehaviour
{

    public static Comms instance;
    //public const string backendURL = "http://localhost:56139";
    public const string backendURL = "http://hacktheirl.azurewebsites.net";

    private void Start()
    {
        instance = this;
    }

    #region GetImage
    public delegate void GetImageCallback(Texture2D img);
    public void GetImage(string url, GetImageCallback callback)
    {
        Debug.Log("Getting image from: " + url);
        StartCoroutine(GetImageCoroutine(url, callback));
    }
    IEnumerator GetImageCoroutine(string url, GetImageCallback callback)
    {
        WWW www = new WWW(url);
        yield return www;
        Texture2D texture = www.texture;
        callback(texture);
    }
    #endregion

    #region GetAlerts
    public delegate void GetAlertsCallback(AlertModel[] alerts);
    public void GetAlerts(float lat, float lon, GetAlertsCallback callback)
    {
        StartCoroutine(GetAlertsCoroutine(lat, lon, callback));
    }
    IEnumerator GetAlertsCoroutine(float lat, float lon, GetAlertsCallback callback)
    {
        WWWForm form = new WWWForm();
        double alat = Manager.defLat;
        double alon = Manager.defLon;
        form.AddField("lat", alat.ToString());
        form.AddField("lon", alon.ToString());
        WWW www = new WWW(backendURL + "/API/GetAlerts", form);
        yield return www;

        AlertModel[] models = JsonConvert.DeserializeObject<AlertModel[]>(www.text);

        callback(models);
    }
    #endregion

    #region Submit Form
    public delegate void SubmitFormCallback();
    public void SubmitForm(AlertModel model, SubmitFormCallback callback)
    {
        StartCoroutine(SubmitFormCoroutine(model, callback));
    }
    IEnumerator SubmitFormCoroutine(AlertModel model, SubmitFormCallback callback)
    {
        WWWForm form;
        WWW www;

        if (model.image != null)
        {
            Debug.Log("------------------- Uploading Image: " + backendURL + "/API/UploadImage");
            // Upload image
            form = new WWWForm();
            form.AddBinaryData("Image", model.image.EncodeToPNG());
            www = new WWW(backendURL + "/API/UploadImage", form);
            yield return www;
            model.imageUrl = www.text;
            Debug.Log("------------------- Uploaded Image: " + www.text);
        }
        else
        {
            model.imageUrl = "null";
        }

        // Upload alert
        Debug.Log("------------------- Uploading Alert: " + backendURL + "/API/SubmitForm");
        form = new WWWForm();
        form.AddField("name", model.name);
        form.AddField("imageUrl", model.imageUrl);
        form.AddField("type", model.type);
        form.AddField("severity", model.severity);
        form.AddField("animalType", model.animalType);
        form.AddField("lat", model.lat.ToString());
        form.AddField("lon", model.lon.ToString());
        www = new WWW(backendURL + "/API/SubmitForm", form);
        yield return www;
        Debug.Log("------------------- Uploaded Alert: " + www.text);
        callback();
    }
    #endregion

    #region Respond
    public delegate void RespondCallback();
    public void RespondToAlert(int alert, RespondCallback callback)
    {
        StartCoroutine(RespondToAlertCoroutine(alert, callback));
    }
    IEnumerator RespondToAlertCoroutine(int alert, RespondCallback callback)
    {
        Manager.instance.Responded(alert);
        // Upload alert
        WWWForm form = new WWWForm();
        form.AddField("id", alert);
        WWW www = new WWW(backendURL + "/API/Respond", form);
        yield return www;

        callback();
    }
    #endregion
}
