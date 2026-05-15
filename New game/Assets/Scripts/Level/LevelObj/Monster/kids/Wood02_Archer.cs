using UnityEngine;

public class Wood02_Archer : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        DoRowAttack();
    }

    protected override void OnMoveSpecial(MonsterOnMove evt)
    {
        base.OnMoveSpecial(evt);
        if (evt.isHorizontalMove)
            evt.isCancelAtk = true;
    }

    private void DoRowAttack()
    {
        int rowY = currentPos.y;
        for (int x = currentPos.x - 1; x >= 0; x--)
        {
            Cell cell = GridMgr.Instance.GetCell(new GridPos(x, rowY));
            if (cell == null) continue;
            if (cell.nowStateType == CellStateType.EntityOccupied)
            {
                BaseDefTower tower = cell.nowObj as BaseDefTower;
                if (tower != null) { tower.Hurt(this); return; }
            }
        }
        GamePlayer.Instance.Hurt(currentAtk);
    }
}
