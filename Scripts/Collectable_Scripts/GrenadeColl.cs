using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeColl : CollectableObjects, IItemCollected
{
    public void BoostApplied()
    {
        GameManager.instance.RemoveThePoints(createdPoint);
        ObjectCollected();
    }

    public override void ObjectCollected()
    {
        M4A4 m4A4 = FindObjectOfType<M4A4>();
        GameManager.instance.RemoveThePoints(createdPoint);

        if (!m4A4) return;

        if (m4A4.grenadeCount < 5)
        {
            m4A4.grenadeCount++;
            m4A4.Update_UI();
            SoundManager.PlaySound(SoundManager.Sound.GrenadeCollected);
            base.ObjectCollected();
        }

    }
}
