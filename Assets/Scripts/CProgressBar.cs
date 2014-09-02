using UnityEngine;
using System.Collections;

public class CProgressBar : MonoBehaviour 
{
    // front Bar 의 스프라이트
    public UISprite mFrontSprite;

    public void SetFill(float Value)
    {
        mFrontSprite.fillAmount = Value;
    }
}
