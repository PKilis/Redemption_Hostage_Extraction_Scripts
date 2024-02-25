using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object_Pooling : MonoBehaviour
{
    public static Object_Pooling Instance;

    public GameObject kursunCarpmaEfekt_pooledObject;
    public int pooledAmount = 30;
    public bool willGrow = true;

    public List<GameObject> kursunCarpmaEfekt_pooledObjects;

    private GameObject mainObject;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        kursunCarpmaEfekt_pooledObjects = new List<GameObject>();
        mainObject = new GameObject("KursunCarpmaEfekti_Pool");

        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = Instantiate(kursunCarpmaEfekt_pooledObject);
            obj.transform.SetParent(mainObject.transform);
            obj.SetActive(false);
            kursunCarpmaEfekt_pooledObjects.Add(obj);
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < kursunCarpmaEfekt_pooledObjects.Count; i++)
        {
            if (!kursunCarpmaEfekt_pooledObjects[i].activeInHierarchy)
            {
                return kursunCarpmaEfekt_pooledObjects[i];
            }
   
        }

        if (willGrow)
        {
            GameObject obj = Instantiate(kursunCarpmaEfekt_pooledObject);
            obj.SetActive(false);
            kursunCarpmaEfekt_pooledObjects.Add(obj);
            return obj;
        }

        return null;
    }

}

