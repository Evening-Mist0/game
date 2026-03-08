using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//抽象的AI行为
public interface I_AIAction 
{
    public void Move(Vector3 dirOrPos);

    public void StopMove();

    public void AtkPlayer();

    public void ChangeAction();
}
