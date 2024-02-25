using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableObjects : MonoBehaviour
{

    public string itemName;
    public int createdPoint;

    private void Start()
    {
        if (itemName == "Bullet")
        {
            gameObject.transform.rotation = Quaternion.Euler(-90, transform.rotation.y, transform.rotation.z);
        }
    }
    public virtual void ObjectCollected()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectCollected();
            IItemCollected collectable = other.GetComponent<IItemCollected>();
            if (collectable == null) return;
            collectable.BoostApplied();
            // Debug.Log("kaldýrýlmasý gerekn created point : " + createdPoint);
            Debug.Log("Kaldýrýldý : " + createdPoint);
            GameManager.instance.RemoveThePoints(createdPoint);

        }
    }
}

public interface IItemCollected
{
    void BoostApplied();
}
