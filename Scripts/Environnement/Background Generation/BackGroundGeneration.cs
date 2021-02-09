using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundGeneration : MonoBehaviour
{
    
    public GameObject target;
    [SerializeField]
    public GameObject[] backGroundObjet;
    [Header("Paramètre")]
    public int maxCube = 10;
    public Vector3 bounds;
    [Range(1, 20)]
    public int maxScale = 6;
    void Start()
    {
        int currentCubeNbr = 0;
        while(currentCubeNbr <= maxCube)
        {
            Vector3 pos = new Vector3(Random.Range(-bounds.x, bounds.x),
                Random.Range(-bounds.y, bounds.y),
                Random.Range(10, bounds.z));
            int index = Random.Range(0, backGroundObjet.Length);
            GameObject currentCube = Instantiate(backGroundObjet[index], target.transform.position + pos, Quaternion.identity, this.transform);
            int scale = Random.Range(1, maxScale);
            currentCube.transform.localScale = new Vector3(scale, scale, scale);
            Vector3 rot = new Vector3(Random.Range(-180, 180), Random.Range(-180, 180), Random.Range(-180, 180));
            currentCube.transform.rotation = Quaternion.Euler(rot);
            currentCubeNbr++;
        }
    }
}
