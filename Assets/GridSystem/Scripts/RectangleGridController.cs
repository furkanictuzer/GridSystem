using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridSystem.Scripts
{
    public class RectangleGridController : MonoBehaviour
    {
        [SerializeField] private Vector3Int gridSize = Vector3Int.one;
        [SerializeField] private Vector3 cellSize = Vector3.one;
        [SerializeField] private Vector3 gridGap;
        [Space(15)]
        
        [SerializeField] private Cell cellPrefab;

        [Space(15)]
        [SerializeField] private List<Cell> generatedCells = new List<Cell>();
        
        private Cell[,,] _cells;
        
        [SerializeField] private bool centered;

        private Vector3 _currentGridGap;
        private bool _centeredNow;

        private void Start()
        {
            Setup();
            
            ReGenerateGrid();
        }

        private void FixedUpdate()
        {
            if (!CheckRelocate())
            {
                Setup();
                RelocateCells();
            }
        }

        private void Setup()
        {
            _currentGridGap = gridGap;
            _centeredNow = centered;
        }

        private bool CheckRelocate()
        {
            return _currentGridGap == gridGap && _centeredNow == centered;
        }
    
        private void GenerateGrid()
        {
            _cells = new Cell[gridSize.x, gridSize.y, gridSize.z];
            
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    for (int k = 0; k < gridSize.z; k++)
                    {
                        Vector3 localPosition = GetLocalCellPosition(new Vector3Int(i, j, k));
                
                        Cell generatedCell = Instantiate(cellPrefab, transform, true);

                        generatedCell.transform.localPosition = localPosition;
                        generatedCell.transform.rotation = Quaternion.identity;
                
                        generatedCell.name = $"Cell {i}x{j}x{k}";
                
                        AddCell(new Vector3Int(i,j,k), generatedCell);
                    }
                }
            }
        }

        private void ReGenerateGrid()
        {
            ClearGrid();
            GenerateGrid();
        }

        private void ClearGrid()
        {
            if (_cells == null) return;
            
            foreach (var cell in generatedCells)
            {
                DestroyImmediate(cell.gameObject);
            }
        }

        private void RelocateCells()
        {
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    for (int k = 0; k < gridSize.z; k++)
                    {
                        Vector3Int index = new Vector3Int(i, j, k);
                        
                        Cell generatedCell = GetCell(index);
                        Vector3 localPosition = GetLocalCellPosition(index);

                        generatedCell.transform.localPosition = localPosition;
                        generatedCell.transform.rotation = Quaternion.identity;
                    }
                }
            }
        }

        private Cell GetCell(Vector3Int index)
        {
            return index.x < _cells.GetLength(0) && index.y < _cells.GetLength(1) && index.z < _cells.GetLength(2)
                ? _cells[index.x, index.y, index.z]
                : null;
        }

        private void AddCell(Vector3Int index ,Cell cell)
        {
            generatedCells.Add(cell);
            _cells[index.x, index.y, index.z] = cell;
        }

        private Vector3 GetLocalCellPosition(Vector3Int cellIndex)
        {
            Vector3 output = new Vector3(
                cellIndex.x * (cellSize.x + gridGap.x) + cellSize.x / 2f,
                cellIndex.y * (cellSize.y + gridGap.y) + cellSize.y / 2f,
                cellIndex.z * (cellSize.z + gridGap.z) + cellSize.z / 2f
                );

            if (centered)
            {
                Vector3 centerDiff = new Vector3
                {
                    x = gridSize.x * cellSize.x + (gridSize.x - 1) * gridGap.x,
                    y = gridSize.y * cellSize.y + (gridSize.y - 1) * gridGap.y,
                    z = gridSize.z * cellSize.z + (gridSize.z - 1) * gridGap.z
                };
                
                centerDiff /= 2f;

                output -= centerDiff;
            }
            
            return output;
        }
    }
}
