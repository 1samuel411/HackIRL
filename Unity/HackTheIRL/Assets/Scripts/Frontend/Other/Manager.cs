using DeadMosquito.AndroidGoodies;
using Mapbox.Map;
using Mapbox.Unity;
using Mapbox.Unity.Map;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{

    public static double defLat = 28.5978112;
    public static double defLon = -81.2010768;

    private List<int> respondedAlerts = new List<int>();

    public static Manager instance;

    public AlertModel[] alertModels;
    public AbstractMap map;
    public float reloadInterval;
    private float curInterval;

    public InfoController infoController;

    private float curReloadInterval;

    private bool init;
    private bool completed = true;

    public List<PinDropController> pinDrops = new List<PinDropController>();
    public GameObject pinDropPrefab;
    public Transform pinDropParent;
    public PinDropController selfView;

    public GameObject overlay;

    private void Awake()
    {
        instance = this;
    }

#if UNITY_ANDROID
    private const string Location_STORAGE_PERMISSION = "android.permission.ACCESS_FINE_LOCATION";

    bool CheckPermissions()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            return true;
        }

        return (AndroidPermissionsManager.IsPermissionGranted(Location_STORAGE_PERMISSION));
    }


    void PermissionRequest()
    {
        if (CheckPermissions())
            return;

        AndroidPermissionsManager.RequestPermission(new[] { Location_STORAGE_PERMISSION }, new AndroidPermissionCallback(
            grantedPermission =>
            {
                    // permission granted.
                },
        deniedPermission =>
        {
                // The permission was denied.
            PermissionRequest();
        }));
    }
#endif

    private void Start()
    {
        PermissionRequest();

        Input.location.Start(0.5f, 0.5f);

        AGGPS.RequestLocationUpdates(500, 1, OnLocationChange);
    }

    void OnDestroy()
    {
        AGGPS.RemoveUpdates();
    }

    void OnLocationChange(AGGPS.Location loc)
    {
        Debug.Log("Our location was changed ---------------------- " + loc.Latitude + ", " + loc.Longitude);
        Debug.Log("Last Data: " + Input.location.lastData.latitude + ", " + Input.location.lastData.longitude);
        defLat = loc.Latitude;
        defLon = loc.Longitude;
    }

    void Update()
    {
        if (Time.time >= curReloadInterval && completed
#if !UNITY_EDITOR 
            && Input.location.lastData.latitude != 0 
# endif
            )
        {
            completed = false;
            Reload();
        }

        if (Time.time >= curInterval)
        {
            curInterval = Time.time + 2f;
            Debug.Log("Status of our location:  " + Input.location.status.ToString());
#if !UNITY_EDITOR
            defLat = Input.location.lastData.latitude;
            defLon = Input.location.lastData.longitude;
#endif
            ReloadPosition();
        }
    }

    void Reload()
    {
        Debug.Log("Reloading!");

        double alat = defLat;
        double alon = defLon;

        if (!init)
        {
            overlay.SetActive(false);
            init = true;
            if (Input.location.isEnabledByUser)
            {
                map.Initialize(new Mapbox.Utils.Vector2d(alat, alon), 17);
            }
            else
            {
                map.Initialize(new Mapbox.Utils.Vector2d(alat, alon), 17);
            }

            map.UpdateMap(map.CenterLatitudeLongitude, 18);
        }


        Comms.instance.GetAlerts(Input.location.lastData.latitude, Input.location.lastData.longitude, ReloadCallback);
    }

    public void DrawPoints()
    {
        // Clear existing
        for (int i = 0; i < pinDrops.Count; i++)
        {
            if (pinDrops[i] == null)
                continue;

            if (HasAlert(pinDrops[i].model.id))
                continue;

            Destroy(pinDrops[i].gameObject);
            pinDrops.RemoveAt(i);
        }
        for (int i = 0; i < alertModels.Length; i++)
        {
            if (respondedAlerts.Contains(alertModels[i].id))
                continue;
            if (ContainsPinId(alertModels[i].id))
                continue;

            GameObject newObj = Instantiate(pinDropPrefab);
            newObj.transform.SetParent(pinDropParent);
            newObj.transform.position = Vector3.one * 100;
            PinDropController controller = newObj.GetComponent<PinDropController>();
            controller.model = alertModels[i];
            pinDrops.Add(controller);
        }
    }

    bool ContainsPinId(int id)
    {
        Debug.Log("Contains pin id: " + id + "???");
        for(int i = 0; i < pinDrops.Count; i++)
        {
            if (pinDrops[i].model.id == id)
                return true;
        }
        return false;
    }

    bool HasAlert(int id)
    {
        for (int i = 0; i < alertModels.Length; i++)
        {
            if (alertModels[i].id == id)
                return true;
        }
        return false;
    }

    void ReloadCallback(AlertModel[] alerts)
    {
        completed = true;
        curReloadInterval = Time.time + reloadInterval;
        alertModels = alerts;
        DrawPoints();
    }

    void ReloadPosition()
    {
        Debug.Log("Reloading!");

        double alat = defLat;
        double alon = defLon;
        selfView.model.lat = (float)alat;
        selfView.model.lon = (float)alon;

        Debug.Log("Set lat: " + selfView.model.lat + ", Self Long: " + selfView.model.lon);
    }

    public void Responded(int alert)
    {
        respondedAlerts.Add(alert);
    }
}
