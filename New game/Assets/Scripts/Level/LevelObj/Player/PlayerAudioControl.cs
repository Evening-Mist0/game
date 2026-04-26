using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_PlayerAudioOperateCardType
{
    /// <summary>
    /// 合成卡牌
    /// </summary>
    Composite,
    /// <summary>
    /// 打出卡牌
    /// </summary>
    Play,

}


public class PlayerAudioControl : MonoBehaviour
{
    
    public void PlaySFX(E_PlayerAudioOperateCardType operateType,E_Element element)
    {
        if(operateType == E_PlayerAudioOperateCardType.Composite)
        {
            switch (element)
            {
                case E_Element.None:
                    break;
                case E_Element.Fire:
                    AudioMgr.Instance.PlaySFX("火系牌合成音效");
                    break;
                case E_Element.Water:
                    AudioMgr.Instance.PlaySFX("水系牌合成音效");

                    break;
                case E_Element.Earth:
                    AudioMgr.Instance.PlaySFX("土系牌合成音效");

                    break;
                case E_Element.Wood:
                    AudioMgr.Instance.PlaySFX("木系牌合成音效");

                    break;
            }
        }
        else
        {
            switch (element)
            {
                case E_Element.None:
                    break;
                case E_Element.Fire:
                    AudioMgr.Instance.PlaySFX("火系攻击音效");

                    break;
                case E_Element.Water:
                    AudioMgr.Instance.PlaySFX("水系攻击音效");

                    break;
                case E_Element.Earth:
                    AudioMgr.Instance.PlaySFX("土系攻击音效");

                    break;
                case E_Element.Wood:
                    AudioMgr.Instance.PlaySFX("木系攻击音效");

                    break;
            }
        }
           
    }
}
