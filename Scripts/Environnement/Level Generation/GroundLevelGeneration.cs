using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environnement.LevelGeneration
{
    public class GroundLevelGeneration : MonoBehaviour
    {
        Grid grid;
        Vector2 cellSize;

        GameObject floor;
        GameObject room;
        //Output : 
        [HideInInspector] public Vector3 EndofRoomStrip { get; private set; }
        [HideInInspector] public Vector3 startPosition { get; private set; }
        [HideInInspector] public int Size { get; private set; }

        public void MakeLevel(int size , Vector2 cellSize, GameObject floor, GameObject room , out Level level)
        {
            Size = size;
            this.cellSize = cellSize;
            this.floor = floor;
            this.room = room;
            startPosition = Vector3.up * cellSize.y;
            grid = new Grid(width: Size, height: 2, cellSizeX: cellSize.x, cellSizeY: cellSize.y, originPosition: Vector3.zero);
            InitialiseLevel(Size);
            MakeRoom(Size);
            level = new Level(startPosition, EndofRoomStrip,Size);
        }
        public void MakeLevel(int size, Vector2 cellSize, GameObject floor, GameObject room,Vector3 position, out Level level)
        {
            grid = new Grid(width: Size, height: 2, cellSizeX: cellSize.x, cellSizeY: cellSize.y, originPosition: position);
            InitialiseLevel(Size);
            MakeRoom(Size);
            startPosition = position + Vector3.up * cellSize.y;
            level = new Level(startPosition, EndofRoomStrip, Size);
        }

        /// <summary>
        /// Make the ground based on a grid
        /// </summary>
        void InitialiseLevel(int levelSize)
        {
            for (int i = 0; i < levelSize; i++)
            {
                grid.SetValue(i, 0, Instantiate<GameObject>(floor, grid.GetCellPosition(i, 0), Quaternion.identity, transform));
            }
        }

        /// <summary>
        /// Fait les salles, laisse 2 espaces libre
        /// </summary>
        void MakeRoom(int roomNumber)
        {
            
            for (int i = 1;  i < roomNumber - 1; i++)
            {
                grid.SetValue(i, 1, Instantiate<GameObject>(room, grid.GetCellPosition(i, 1), Quaternion.identity, transform));

                if (i == roomNumber - 1)
                {
                    EndofRoomStrip = grid.GetWorldPosition(i, 0) + new Vector3(cellSize.x, cellSize.y, 0);
                }
            }
        }

    }

    public struct Level
    {
        public Vector3 endPosition;
        public Vector3 startPosition;
        public int size;

        // Constructeur
        public Level(Vector3 startPosition, Vector3 endofRoomStrip, int size)
        {
            this.startPosition = startPosition;
            this.endPosition = endofRoomStrip;
            this.size = size;
        }
    }


}