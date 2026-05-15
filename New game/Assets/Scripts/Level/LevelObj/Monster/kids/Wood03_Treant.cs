using System.Collections.Generic;
using UnityEngine;

public class Wood03_Treant : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    private static readonly Color HealRangeColor = new Color(0.2f, 0.55f, 0.7f, 0.4f);
    private List<Cell> _highlightedCells = new List<Cell>();

    protected override void Start()
    {
        base.Start();
        HighlightRange();
    }

    
    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
            base.OnEnterSpecial(evt);
    }
    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        HealInRange();
        HighlightRange();
    }

    protected override void OnMoveOverSpecial(MonsterOnMoveOver evt)
    {
        base.OnMoveOverSpecial(evt);
        HighlightRange();
    }

    protected override void OnDeadSpecial(MonsterOnDead evt)
    {
        base.OnDeadSpecial(evt);
        ClearHighlights();
    }

    private void OnDestroy()
    {
        ClearHighlights();
    }

    private void OnDisable()
    {
        ClearHighlights();
    }

    private void HealInRange()
    {
        List<BaseMonsterCore> all = MonsterCreater.Instance.GetAllAliveMonsters();
        List<Cell> range = GetCrossRangeCells();
        for (int i = 0; i < all.Count; i++)
        {
            BaseMonsterCore m = all[i];
            if (m == this || m == null) continue;
            GridPos mp = m.currentPos;
            for (int j = 0; j < range.Count; j++)
            {
                if (range[j].logicalPos.x == mp.x && range[j].logicalPos.y == mp.y)
                {
                    m.AddHp(2);
                    break;
                }
            }
        }
    }

    private List<Cell> GetCrossRangeCells()
    {
        List<Cell> list = new List<Cell>();
        GridPos c = currentPos;
        GridPos[] offsets = { new GridPos(0,1), new GridPos(0,-1), new GridPos(-1,0), new GridPos(1,0) };
        for (int i = 0; i < offsets.Length; i++)
        {
            Cell cell = GridMgr.Instance.GetCell(new GridPos(c.x + offsets[i].x, c.y + offsets[i].y));
            if (cell != null) list.Add(cell);
        }
        return list;
    }

    private void HighlightRange()
    {
        ClearHighlights();
        List<Cell> range = GetCrossRangeCells();
        for (int i = 0; i < range.Count; i++)
        {
            range[i].myUIControl.SetPersistentHighlight(HealRangeColor);
            _highlightedCells.Add(range[i]);
        }
    }

    private void ClearHighlights()
    {
        for (int i = 0; i < _highlightedCells.Count; i++)
        {
            if (_highlightedCells[i] != null)
                _highlightedCells[i].myUIControl.ClearPersistentHighlight();
        }
        _highlightedCells.Clear();
    }
}
