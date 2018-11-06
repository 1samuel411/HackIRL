using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuController : Controller
{

    public new MainMenuView view;

    private void Expand()
    {
        view.expandedAnimator.gameObject.SetActive(true);
    }

    private void Contract()
    {
        view.expandedAnimator.gameObject.SetActive(false);
    }

    private bool toggled = false;
    public void Toggle()
    {
        toggled = !toggled;
        if (toggled)
            Expand();
        else
            Contract();
    }

    public void Report(int type)
    {
        view.formController.Open(type);
        Toggle();
    }

    public void OpenFaq()
    {
        Toggle();
        view.faqController.Open();
    }
}
