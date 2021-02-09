using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environnement.LevelGeneration
{
    public class GridTest : MonoBehaviour
    {
        Grid grid;

        private void Start()
        {
            grid = new Grid(width : 10, height :  10, cellSizeX : 6f, cellSizeY: 2f, originPosition : new Vector3(-15, 5));
        }
    }
}
