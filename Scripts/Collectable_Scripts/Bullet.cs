using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : CollectableObjects, IItemCollected
{
    public void BoostApplied()
    {
        ObjectCollected();
    }

    public override void ObjectCollected()
    {
        M4A4 m4A4 = FindObjectOfType<M4A4>();
        GameManager.instance.RemoveThePoints(createdPoint);

        if (!m4A4) return;

        m4A4.toplamMermiSayisi += 30;
        m4A4.Update_UI();
        SoundManager.PlaySound(SoundManager.Sound.AmmoCollected);
        base.ObjectCollected();


    }
}
