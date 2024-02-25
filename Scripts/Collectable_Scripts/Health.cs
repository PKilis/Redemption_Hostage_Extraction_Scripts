using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : CollectableObjects, IItemCollected
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

        if (m4A4.health >= 100) return;

        if (m4A4.health <= 70)
        {
            m4A4.health += 30;
        }
        else
        {
            m4A4.health = 100;
        }

        m4A4.Update_UI();
        SoundManager.PlaySound(SoundManager.Sound.HealthCollected);
        base.ObjectCollected();


    }

}
