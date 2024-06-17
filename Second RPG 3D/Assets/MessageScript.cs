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
                buttonText.text = "Nh?n T ?? b? qua hu�ng d?n";
                break;
            case 1:
                buttonText.text = "?? di chuy?n s? d?ng W , A , S , D ";
                break;
            case 2:
                buttonText.text = " D�ng Q ?? ??i g�c nh�n ";
                break;
            case 3:
                buttonText.text = " ti�u di?t qu�i v?t v� sinh t?n ";
                break;
            default:
                menu.gameObject.SetActive(false);
                break;
        }
    }
}
