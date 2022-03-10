using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{

    public Image background;
    public TextMeshProUGUI title;
    public Button playButton;
    public Animator animator;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Hide() 
    {

        animator.SetTrigger("Hide");

    }

    public void Show()
    {

        animator.SetTrigger("Show");

    }

}
