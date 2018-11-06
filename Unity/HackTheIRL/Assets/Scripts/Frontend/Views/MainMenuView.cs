using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class MainMenuView : View
{

    public Button expandButton;
    public Button pollutionButton;
    public Button injuryButton;
    public Button crimeButton;
    public Button faqButton;

    public FormController formController;
    public FaqController faqController;

    public Animator expandedAnimator;

}
