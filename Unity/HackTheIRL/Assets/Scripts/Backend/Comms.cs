using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Comms : MonoBehaviour
{

    public static Comms instance;
    public const string backendURL = "http://localhost:56139";

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
        // First, check if user has location service enabled
        if (!Input.location.isEnabledByUser)
            yield break;

        // Start service before querying location
        Input.location.Start();

        // Wait until service initializes
        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        // Service didn't initialize in 20 seconds
        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }

        // Connection has failed
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        else
        {
            // Access granted and location value could be retrieved
            print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
        }

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
        form.AddField("lat", Input.location.lastData.latitude.ToString());
        form.AddField("lon", Input.location.lastData.longitude.ToString());
        www = new WWW(backendURL + "/API/SubmitForm", form);
        yield return www;
        callback();

        // Stop service if there is no need to query location updates continuously
        Input.location.Stop();
    }

}
