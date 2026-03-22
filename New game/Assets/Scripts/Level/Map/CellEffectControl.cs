using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 要求挂载Cell、SpriteRenderer、CellEventTrigger组件
[RequireComponent(typeof(Cell)), RequireComponent(typeof(SpriteRenderer)), RequireComponent(typeof(CellEventTrigger))]
public class CellEffectControl : MonoBehaviour
{
    // 精灵渲染器（替代UGUI的Image组件）
    private SpriteRenderer spriteRenderer;
    // 是否允许高亮（仅在卡牌操作时生效）
    public bool isAllowedHighLight;
    // 当前挂载的Cell脚本引用
    private Cell myCell;

    /// <summary>
    /// 初始化组件和默认状态
    /// </summary>
    private void Awake()
    {
        // 获取组件引用
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCell = GetComponent<Cell>();

        // 初始化颜色为白色（默认无高亮）
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// 进入高亮状态（单个单元格）
    /// </summary>
    public void EnterHighLight()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.green;
        }
    }

    /// <summary>
    /// 进入高亮状态（单元格列表批量处理）
    /// </summary>
    /// <param name="cellList">需要高亮的单元格列表</param>
    public void EnterHighLight(List<Cell> cellList)
    {
        if (cellList == null) return;

        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].myUIControl.EnterHighLight();
        }
    }

    /// <summary>
    /// 退出高亮状态（单个单元格）
    /// </summary>
    public void ExitHightLight()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }

    /// <summary>
    /// 退出高亮状态（单元格列表批量处理）
    /// </summary>
    /// <param name="cellList">需要退出高亮的单元格列表</param>
    public void ExitHightLight(List<Cell> cellList)
    {
        if (cellList == null) return;

        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].myUIControl.ExitHightLight();
        }
    }

    /// <summary>
    /// 在格子上生成对应技能的特效
    /// </summary>
    public void CreatEffect()
    {

    }

    /// <summary>
    /// 鼠标进入物体范围时触发（替代IPointerEnterHandler）
    /// 注意：物体必须有碰撞体（如BoxCollider2D）才能触发
    /// </summary>
    private void OnMouseEnter()
    {
        Debug.Log("鼠标进入单元格范围");

        // 非玩家卡牌操作阶段则直接返回
        if (!LevelStepMgr.Instance.ComfirNowStateType(E_LevelState.PlayerTurn_CardOperate))
            return;

        // 获取当前卡牌操作状态
        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        //Debug.Log("判定为卡牌操作状态，当前是否允许单元格高亮：" + state.isAllowedCellHighLight);

        // 更新预选单元格和列表
        state.preSlectedCell = myCell;
        state.UpdatePreSlectedCellList(myCell);

        // 允许高亮时执行批量高亮
        if (state.isAllowedCellHighLight)
        {
            EnterHighLight(state.preSlectedCellList);
        }
    }

    /// <summary>
    /// 鼠标离开物体范围时触发（替代IPointerExitHandler）
    /// </summary>
    private void OnMouseExit()
    {
        // 获取当前卡牌操作状态
        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        if (state == null) return;

        // 批量退出高亮并清空预选数据
        ExitHightLight(state.preSlectedCellList);
        state.ClearPreSlectedCellAndList();
    }

    /// <summary>
    /// 鼠标点击物体时触发（替代IPointerClickHandler）
    /// </summary>
    private void OnMouseDown()
    {
        // 获取当前卡牌操作状态
        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        if (state == null) return;

        Debug.Log("取消选中的单元格");
        // 退出高亮并清空预选数据
        ExitHightLight(state.preSlectedCellList);
        state.ClearPreSlectedCellAndList();
    }

   

    #region 重要说明
    // 使用OnMouse系列方法的注意事项：
    // 1. 不需要挂载Physics Raycaster组件（UGUI射线检测）
    // 2. 物体必须挂载碰撞体组件（如BoxCollider2D/Collider2D），且碰撞体需启用
    // 3. 碰撞体不能设置为IsTrigger（除非在OnMouseOver中处理，否则会失效）
    // 4. 相机需开启Physics 2D Raycaster（2D场景）
    #endregion
}