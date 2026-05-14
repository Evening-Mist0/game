using UnityEngine;

/// <summary>
/// 木精（小怪）：向前成功移动时在后面分裂一个相同的木精，每只木精有一次分裂机会。
/// 一回合移动一格，攻击力为2。
/// </summary>
public class Wood01_WoodSpirit : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Header("分裂配置")]
    [Tooltip("分裂时使用的资源路径（与自身预制体相同）")]
    public string splitResName;

    private bool _hasSplit = false;
    private GridPos _prevPosBeforeMove;

    protected override void OnMoveSpecial(MonsterOnMove evt)
    {
        base.OnMoveSpecial(evt);
        // 缓存移动前的位置：OnMoveOverSpecial 回调时 currentPos 已经更新为新位置，
        // 需要旧位置来在"后面"一格生成分身
        _prevPosBeforeMove = evt.currentPos;
    }

    protected override void OnMoveOverSpecial(MonsterOnMoveOver evt)
    {
        base.OnMoveOverSpecial(evt);

        if (_hasSplit)
            return;

        if (string.IsNullOrEmpty(splitResName))
        {
            Debug.LogWarning($"[{monsterName}] 未设置 splitResName，无法分裂");
            return;
        }

        Cell targetCell = GridMgr.Instance.GetCell(_prevPosBeforeMove);
        if (targetCell == null)
        {
            Debug.LogWarning($"[{monsterName}] 分裂目标格子({_prevPosBeforeMove.x},{_prevPosBeforeMove.y})不存在");
            return;
        }

        // 格子被实体防御塔或怪物占用则无法分裂
        if (targetCell.nowStateType != CellStateType.None &&
            targetCell.nowStateType != CellStateType.GhostOccupied)
        {
            Debug.Log($"[{monsterName}] 目标格子已被占用(state={targetCell.nowStateType})，无法分裂");
            return;
        }

        int result = MonsterCreater.Instance.CreateOneMonsterAt(
            splitResName, _prevPosBeforeMove.x, _prevPosBeforeMove.y);

        if (result > 0)
        {
            _hasSplit = true;
            LevelStepMgr.Instance.monsterAliveCount += result;
            Debug.Log($"[{monsterName}] 木精分裂成功：在({_prevPosBeforeMove.x},{_prevPosBeforeMove.y})生成分身");
        }
    }
}
