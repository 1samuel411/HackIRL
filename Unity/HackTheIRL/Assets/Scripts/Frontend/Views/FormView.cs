using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class FormView : View
{

    public Text headerText;
    public InputField nameField;
    public Button selectImageButton;
    public Dropdown severityDropdown;
    public Dropdown animalTypeDropdown;
    public Image selectedImage;
    public Button submitButton;

}
