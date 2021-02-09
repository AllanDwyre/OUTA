using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace Environnement.LevelGeneration
{
    //make different room Emplacement.
    //Link them with the climbLevel Generator
    public class LevelManager : MonoBehaviour
    {

        [Header("Grid Setup")]

        [SerializeField] Vector2 cellSize = new Vector2(6f, 2f);

        [Header("Room Setup")]

        [SerializeField] GameObject floor;
        [SerializeField] GameObject room;

        [SerializeField] Vector2 roomMinMax; // Nombre de salle (entre a et b)


        GroundLevelGeneration groundLevel;
        PathGeneration pathGen;

        List<Level> levels = new List<Level>();
        Vector3 lastPos;

        private void Awake()
        {
            groundLevel = GetComponent<GroundLevelGeneration>();
            pathGen = GetComponent<PathGeneration>();
        }
        private void Start()
        {
            LevelGeneration();
        }


        void LevelGeneration()
        {
            MakeLevel();
            MakeLevel(4,30);
            LinkLevels(1);
            //for (int i = 1; i < 4; i++)
            //{

            //    MakeLevel(Random.Range(4,30), Random.Range(20, 60));
            //    LinkLevels(i);
            //}
        }

        void MakeLevel()
        {
            Level level;
            groundLevel.MakeLevel(GenerateSize(), cellSize, floor, room, out level);
            levels.Add(level);
            lastPos = level.endPosition;
        }
        void MakeLevel(Vector3 offset)
        {
            Level level;
            groundLevel.MakeLevel(GenerateSize(), cellSize, floor, room, lastPos + offset, out level);
            levels.Add(level);
            lastPos = level.endPosition;
        }
        void MakeLevel(float x , float y)
        {
            MakeLevel(new Vector3(x, y));
        }

        void LinkLevels(int index)
        {
            Level[] arr_levels = levels.ToArray();
            Vector3 start = arr_levels[index-1].endPosition;
            Vector3 end = arr_levels[index].startPosition;
            pathGen.MakePath(start, end);
        }

        int GenerateSize()
        {
            return Random.Range((int)roomMinMax.x, (int)roomMinMax.y);
        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            foreach (Level item in levels)
            {
                Gizmos.DrawWireSphere(item.endPosition, 1.5f);
                Gizmos.DrawWireSphere(item.startPosition, 1f);
            }

        }
    }
}