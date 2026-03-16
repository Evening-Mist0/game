using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//抽象的AI行为
public interface I_AIAction 
{
    public bool Move(GridPos speed);

    public void BeStopped(BaseLevelObject stopObj);

    public void Atk(BaseLevelObject obj);

    public void Die();

}
