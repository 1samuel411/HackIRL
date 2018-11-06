using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoController : Controller
{

    public new InfoView view;
    public AlertModel alert;

    public void Open(AlertModel model)
    {
        Open();
        alert = model;
        if(!String.IsNullOrEmpty(alert.imageUrl) && alert.imageUrl != "null")
            Comms.instance.GetImage(alert.imageUrl, GetImageResponse);
    }

    public void Open(PinDropController pinDrop)
    {
        Open();
        alert = pinDrop.model;
        if (!String.IsNullOrEmpty(alert.imageUrl) && alert.imageUrl != "null")
            Comms.instance.GetImage(alert.imageUrl, GetImageResponse);
    }

    void GetImageResponse(Texture2D texture)
    {
        alert.image = texture;
    }

    public override void Close()
    {
        base.Close();
        alert = new AlertModel();
    }

    private void Update()
    {
        if(alert.image == null)
        {
            view.image.color = new Color(1, 1, 1, 0);
        }
        else
        {
            view.image.color = new Color(1, 1, 1, 1);
            view.image.sprite = Sprite.Create(alert.image, new Rect(0, 0, alert.image.width, alert.image.height), Vector2.one * 0.5f);
        }

        if (alert.type == 0)
        {
            view.headerText.text = "Injury";
            view.descText.text = GetAnimal(alert.animalType) + "   |   " + GetSeverity(alert.severity);
        }
        else if (alert.type == 1)
        {
            view.headerText.text = "Pollution";
            view.descText.text = GetSeverity(alert.severity);
        }
        else if (alert.type == 2)
        {
            view.headerText.text = "Crime";
            view.descText.text = GetSeverity(alert.severity);
        }

        view.nameText.text = alert.name;
    }

    public void Respond()
    {
        Comms.instance.RespondToAlert(alert.id, RespondCallback);
        Close();
        for (int i = 0; i < Manager.instance.pinDrops.Count; i++)
        {
            if (Manager.instance.pinDrops[i].model.id == alert.id)
            {
                Destroy(Manager.instance.pinDrops[i].gameObject);
            }
        }
    }

    void RespondCallback()
    {
        Close();
        for(int i = 0; i < Manager.instance.pinDrops.Count; i++)
        {
            if(Manager.instance.pinDrops[i].model.id == alert.id)
            {
                Destroy(Manager.instance.pinDrops[i].gameObject);
            }
        }
    }

    public string GetAnimal(int i)
    {
        switch (i)
        {
            case 0:
                return "Dolphin";
                break;
            case 1:
                return "Manatee";
                break;
            case 2:
                return "Sea Turtle";
                break;
            case 3:
                return "Bird";
                break;
        }

        return "";
    }

    public string GetSeverity(int i)
    {
        switch (i)
        {
            case 0:
                return "Low Priority";
                break;
            case 1:
                return "Moderate";
                break;
            case 2:
                return "Severe";
                break;
        }

        return "";
    }

}
