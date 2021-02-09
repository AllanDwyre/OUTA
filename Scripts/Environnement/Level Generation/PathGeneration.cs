using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environnement.LevelGeneration
{
    public class PathGeneration : MonoBehaviour
    {
        // Make a path beetween 2 rooms
        //Different path Behavior

        // make 2 point in LevelManager

        [SerializeField] GameObject cube;
        [Tooltip("Nombre de cube par metre")]
        [SerializeField] float ratio = .7f;
        [SerializeField] Vector2 pathOffset = new Vector2(2f, 1f);
        [SerializeField] float xOffset = 1.7f;
        Vector3 start;
        Vector3 end;

        public void MakePath(Vector3 _start , Vector3 _end)
        {
            start = _start + ( Vector3.right * pathOffset.x + Vector3.up * pathOffset.y );
            end = _end - (Vector3.right * pathOffset.x + Vector3.up * pathOffset.y);
            MakePath();
        }

        private void MakePath()
        {
            print(GetDist(start, end));
            // ratio point/distance : 1 cube tout les 2 metres
            int numberOfPoint = Mathf.RoundToInt(ratio * GetDist(start, end));
            print(numberOfPoint);
            GameObject[] array = new GameObject[numberOfPoint+1];
            for (int i = 0; i <= numberOfPoint; i++)
            {
                Vector3 pos = GetDir(start, end) * (( (float)i / (float)numberOfPoint) * GetDist(start, end) );
                pos += start;
                array[i] = Instantiate(cube, pos, Quaternion.identity);
            }
            InterlapolateCube(array);
        }


        void InterlapolateCube(GameObject[] array)
        {
            float offset = xOffset;
            for(int i = 0; i < array.Length; i++)
            {
                if(i%2 == 0)
                {
                    array[i].transform.position += ProjectDirectionOnPlane(Vector3.right, GetDir(start, end)) * offset;
                }
                else
                {
                    array[i].transform.position -= ProjectDirectionOnPlane(Vector3.right, GetDir(start, end)) * offset;
                }
            }
        }
        Vector3 GetDir(Vector3 from, Vector3 to)
        {
            return new Vector3(to.x - from.x, to.y - from.y, to.z - from.z).normalized;
        }
        float GetDist(Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a, b);
        }
         
        Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
        {
            return ( direction - normal * Vector3.Dot(direction, normal) ).normalized;
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(start, end);
        }
    }
}