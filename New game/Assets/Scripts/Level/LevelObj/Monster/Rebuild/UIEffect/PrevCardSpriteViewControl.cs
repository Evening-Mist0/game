using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrevCardSpriteViewControl : MonoBehaviour
{
    public TMP_Text textCharacter;
    public Image nowImage;

    public void UpdateText(string content)
    {   
        textCharacter.text = content;
    }

    public void UpdateImage(E_Element type)
    {
        switch (type)
        {
            case E_Element.None:
                nowImage.sprite = Resources.Load<Sprite>("CardSprite/NoneCard");
                break;
            case E_Element.Fire:
                nowImage.sprite = Resources.Load<Sprite>("CardSprite/BasicalFireCard");
                break;
            case E_Element.Water:
                nowImage.sprite = Resources.Load<Sprite>("CardSprite/BasicalWaterCard");
                break;
            case E_Element.Earth:
                nowImage.sprite = Resources.Load<Sprite>("CardSprite/BasicalEarthCard");
                break;
            case E_Element.Wood:
                nowImage.sprite = Resources.Load<Sprite>("CardSprite/BasicalWoodCard");
                break;
        }
    }
}
