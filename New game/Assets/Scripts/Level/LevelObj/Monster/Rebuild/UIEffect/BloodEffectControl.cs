using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodEffectControl : MonoBehaviour
{
    //个位数
    public SpriteRenderer sr1;
    //十位数
    public SpriteRenderer sr2;
    //血量为个位数的时候的位置(相对父对象坐标)
    public Vector3 controlLocalPos1;
    //血量为十位数的时候的位置(相对父对象坐标)
    public Vector3 controlLocalPos2;
    
    
    /// <summary>
    /// 更新血量图片
    /// </summary>
    public void UpdateBlood(int hp)
    {
        if (hp < 0)
            return;

        string path1 = "Number/Num_";
        if (hp < 10)
        {
            sr2.gameObject.SetActive(false);
            this.transform.localPosition = controlLocalPos1;
            path1 += hp.ToString();
            Sprite sp = Resources.Load<Sprite>(path1);
            if (sp == null)
                Debug.LogError($"传入的资源路径错误{path1}");
            else
            {
                Debug.Log("血量图片资源加载成功");
                sr1.sprite = sp;
            }
        }
        else
        {
            sr2.gameObject.SetActive(true);
            string basePath = "Number/Num_";
            this.transform.localPosition = controlLocalPos2;

            int tempHp = hp;

            // 个位数值
            int pos1 = tempHp % 10;
            Debug.Log("计算出个位数的值为" + pos1);
            path1 = basePath + pos1.ToString();

            // 十位数值  
            int pos2 = tempHp / 10;
            Debug.Log("计算出十位数的值为" + pos2);
            string path2 = basePath + pos2.ToString();

            // 加载个位数图片
            Sprite sp1 = Resources.Load<Sprite>(path1);
            if (sp1 == null)
                Debug.LogError($"传入的资源路径错误{path1}");
            else
            {
                Debug.Log("血量图片个位数资源加载成功");
                sr1.sprite = sp1; 
            }

            // 加载十位数图片  
            Sprite sp2 = Resources.Load<Sprite>(path2);
            if (sp2 == null)
                Debug.LogError($"传入的资源路径错误{path2}");
            else
            {
                Debug.Log("血量图片十位数资源加载成功");
                sr2.sprite = sp2;
            }
        }
    }

}
