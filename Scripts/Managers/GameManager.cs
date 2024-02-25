using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    public GameObject[] points;
    public GameObject[] collectables;
    public Camera benimCam;


    List<int> createdPoints = new List<int>();

    internal int layerMask;

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        Time.timeScale = 0;
        layerMask = 1 << 9;         // sadece 9'u al = 512
        layerMask = ~layerMask;     // 9 hariç hepsini algýla = 513

        StartCoroutine(CreateCollectableObjects());
    }

    public IEnumerator CreateCollectableObjects()
    {


        while (true)
        {
            yield return new WaitForSeconds(5f);


            int randObject = Random.Range(0, collectables.Length);
            int randCount = Random.Range(0, points.Length);

            if (!createdPoints.Contains(randCount))
            {
                createdPoints.Add(randCount);

            }
            else
            {
                randCount = Random.Range(0, points.Length - 1);
                continue;

            }

            GameObject tempObject = Instantiate(collectables[randObject], points[randCount].transform.position, Quaternion.identity);
            tempObject.transform.SetParent(points[randCount].transform);
            tempObject.GetComponent<CollectableObjects>().createdPoint = randCount;


        }
    }

    public void RemoveThePoints(int point)
    {
        createdPoints.Remove(point);
    }



    public IEnumerator CameraTitre(float shakeDuration, float magnitude)
    {
        Vector3 orjinalPozisyon = benimCam.transform.localPosition;

        float gecenSure = 0.0f;
        while (gecenSure < shakeDuration)
        {
            float x = Random.Range(-1, 1) * magnitude;

            benimCam.transform.localPosition = new Vector3(x, orjinalPozisyon.y, orjinalPozisyon.z);
            gecenSure += Time.deltaTime;
            yield return null;
        }
        benimCam.transform.localPosition = orjinalPozisyon;

    }

    public void CreateDamageIFace(MonoBehaviour script, int damageAmount)
    {
        IDamageable damage = script.GetComponent<IDamageable>();
        if (damage == null) return;
        damage.Damage(damageAmount);
    }

}
