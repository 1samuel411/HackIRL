using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinDropController : MonoBehaviour
{

    public PinDropView view;
    public AlertModel model;

    public Color[] colors;
    public Sprite[] icons;
    public Sprite pollution;
    public Sprite self;

    public bool selfShow;

    public RectTransform rectTransform;
    public RectTransform canvas;

    public Vector3 offset;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();

        UpdatePos();
    }

    void UpdatePos()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>().gameObject.GetComponent<RectTransform>();

        Vector3 tarPos = Manager.instance.map.GeoToWorldPosition(new Mapbox.Utils.Vector2d(model.lat, model.lon), false) + offset;
        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(tarPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * canvas.sizeDelta.x) - (canvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * canvas.sizeDelta.y) - (canvas.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        rectTransform.anchoredPosition = WorldObject_ScreenPosition;

        transform.localEulerAngles = new Vector3(0, 0, 0);
        transform.localScale = Vector3.one;
    }

    void UpdateUI()
    {
        if(selfShow)
        {
            view.icon.sprite = self;
            view.severity.color = Color.grey;
            return;
        }
        if (model.type == 0)
            view.icon.sprite = icons[model.animalType];
        else if (model.type == 1)
            view.icon.sprite = pollution;
        view.severity.color = colors[model.severity];
    }

    public void OnClick()
    {
        Manager.instance.map.SetCenterLatitudeLongitude(new Mapbox.Utils.Vector2d(model.lat, model.lon));
        Manager.instance.map.UpdateMap(Manager.instance.map.CenterLatitudeLongitude, 19f);

        if (!selfShow)
            Manager.instance.infoController.Open(this);
    }

    public Vector3 worldToUISpace(Canvas parentCanvas, Vector3 worldPos)
    {
        //Convert the world for screen point so that it can be used with ScreenPointToLocalPointInRectangle function
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        Vector2 movePos;

        //Convert the screenpoint to ui rectangle local point
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera, out movePos);
        //Convert the local point to world point
        return parentCanvas.transform.TransformPoint(movePos);
    }
}
