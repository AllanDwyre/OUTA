using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Environnement.LevelGeneration
{
    public class Grid
    {
        private int width;
        private int height;
        private float cellSizeX;
        private float cellSizeY;
        private Vector3 originPosition;
        private GameObject[,] gridArray;


        //Constructeur : 
        public Grid(int width, int height, float cellSizeX, float cellSizeY, Vector3 originPosition)
        {
            this.width = width;
            this.height = height;
            this.cellSizeX = cellSizeX;
            this.cellSizeY = cellSizeY;
            this.originPosition = originPosition;

            gridArray = new GameObject[width, height];

            for (int x = 0; x < gridArray.GetLength(0); x++)
            {
                for (int y = 0; y < gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

        /// <summary>
        /// Retourne la position dans le monde de chaque case
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, 0) * cellSizeX + new Vector3(0, y) * cellSizeY + originPosition ;
        }
        public Vector3 GetCellPosition(int x, int y)
        {
            return new Vector3(x * cellSizeX + cellSizeX/2 , y * cellSizeY + cellSizeY / 2) + originPosition;
        }

        public void SetValue(int x, int y, GameObject value)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                gridArray[x, y] = value;
            }
        }
        public void SetValue(Vector3 worldPosition, GameObject value)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetValue(x, y, value);
        }

        public GameObject GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                return gridArray[x, y];
            }
            else
            {
                return null;
            }
        }
        public GameObject GetValue(Vector3 worldPosition)
        {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        /// <summary>
        /// Retourne les x et y selon une position global
        /// </summary>
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(( worldPosition - originPosition ).x / cellSizeX);
            y = Mathf.FloorToInt(( worldPosition - originPosition ).y / cellSizeX);
        }

    }
}