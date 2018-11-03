using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormController : Controller
{

    public new FormView view;
    public int type;
    public AlertModel alertModel;

    private void OnEnable()
    {
        
    }

    public void Open(int type)
    {
        alertModel = new AlertModel();
        Open();
        if(type == 0)
        {
            view.headerText.text = "Report a Injury";
            view.animalTypeDropdown.gameObject.SetActive(true);
            view.severityDropdown.gameObject.SetActive(true);
        }
        else if(type == 1)
        {
            view.headerText.text = "Report Pollution";
            view.animalTypeDropdown.gameObject.SetActive(false);
            view.severityDropdown.gameObject.SetActive(true);
        }
        else if(type == 2)
        {
            view.animalTypeDropdown.gameObject.SetActive(false);
            view.severityDropdown.gameObject.SetActive(true);
            view.headerText.text = "Report a Crime";
        }
    }

    private void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        view.animalTypeDropdown.value = alertModel.animalType;
        view.severityDropdown.value = alertModel.severity;
        view.nameField.text = alertModel.name;
        if(alertModel.image == null)
        {
            view.selectedImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            view.selectedImage.color = new Color(1, 1, 1, 1);
            view.selectedImage.sprite = alertModel.image;
        }
    }

    public void SelectImage(Texture2D texture)
    {
        Sprite newSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
        alertModel.image = newSprite;
    }

    public void SubmitForm()
    {
        Debug.Log("Submitting Form!");
    }

    public void SetSeverity(int val)
    {
        alertModel.severity = val;
    }

    public void SetAnimalType(int val)
    {
        alertModel.animalType = val;
    }

    public void SetName(string text)
    {
        alertModel.name = text;
    }

}