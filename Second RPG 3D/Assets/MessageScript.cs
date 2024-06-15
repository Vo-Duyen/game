using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class MessageScript : MonoBehaviour
{
    public TextMeshProUGUI buttonText;
    
    private int buoc = 0 ;
    public GameObject menu;

    
   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            buoc++;
        }

        switch (buoc)
        {
            case 0:
                buttonText.text = "1";
                break;
            case 1:
                buttonText.text = "2";
                break;
            case 2:
                buttonText.text = "3";
                break;
            case 3:
                buttonText.text = "4";
                break;
            default:
                menu.gameObject.SetActive(false);
                break;
        }
    }
}
