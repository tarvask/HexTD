using System;
using System.Collections.Generic;
using Tools;
using UnityEngine;

namespace Match.Field.Services
{
    //TODO: rewrite for hex pathfinder based on points from MapEditor
    /*
    public static class RoadWayPointsFinder
    {
        public static Vector3[] GetWaypointsFromField(FieldCellType[,] cells)
        {
            Vector2Int castlePosition = GetCastlePosition(cells);
            
            // we only know castle cell, so find path starting from it
            List<Vector2Int> waypointsFromCastle = new List<Vector2Int>((MatchShortParameters.FieldHeight + MatchShortParameters.FieldWidth) * 2);
            // it will help to process repeating entries
            HashSet<int> waypointsHashSet = new HashSet<int>();
            // add starting point
            waypointsFromCastle.Add(new Vector2Int(castlePosition.x, castlePosition.y));

            bool isFieldEnterFound = false;

            do
            {
                Vector2Int currentWayPoint = waypointsFromCastle[waypointsFromCastle.Count - 1];

                if (GetNewRoadNearCell(currentWayPoint,
                    cells, waypointsHashSet, out Vector2Int newRoadCell))
                {
                    waypointsFromCastle.Add(newRoadCell);
                    waypointsHashSet.Add(newRoadCell.GetHashCode(MatchShortParameters.FieldWidth));
                }
                else if (GetAnyRoadNearCellExceptLastWayPoint(currentWayPoint,
                    cells, waypointsFromCastle, out Vector2Int repeatingRoadCell))
                {
                    waypointsFromCastle.Add(repeatingRoadCell);
                    waypointsHashSet.Add(repeatingRoadCell.GetHashCode(MatchShortParameters.FieldWidth));
                }
                else
                    isFieldEnterFound = true;
            }
            while (!isFieldEnterFound);
            
            waypointsFromCastle = RemoveRedundantWayPoints(waypointsFromCastle);

            return RevertPathAndAdjustEdgePoints(waypointsFromCastle);
        }

        private static Vector2Int GetCastlePosition(FieldCellType[,] cells)
        {
            for (int cellY = 0; cellY < MatchShortParameters.FieldHeight; cellY++)
            {
                for (int cellX = 0; cellX < MatchShortParameters.FieldWidth; cellX++)
                {
                    if (cells[cellY, cellX] == FieldCellType.Castle)
                        return new Vector2Int(cellX, cellY);
                }
            }
            
            throw new ArgumentException("No castle on field");
        }

        // try to find road near cell that is not already presented in waypointsHashSet
        private static bool GetNewRoadNearCell(Vector2Int currentCell, FieldCellType[,] cells, HashSet<int> waypointsHashSet, out Vector2Int result)
        {
            result = -Vector2Int.one;
            
            // left cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.x > 0
                && cells[currentCell.y, currentCell.x - 1] == FieldCellType.Road
                && !waypointsHashSet.Contains(
                    new Vector2Int(currentCell.x - 1, currentCell.y).GetHashCode(MatchShortParameters.FieldWidth)))
            {
                result = new Vector2Int(currentCell.x - 1, currentCell.y);
                return true;
            }
                
            // right cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.x + 1 < MatchShortParameters.FieldWidth
                && cells[currentCell.y, currentCell.x + 1] == FieldCellType.Road
                && !waypointsHashSet.Contains(
                    new Vector2Int(currentCell.x + 1, currentCell.y).GetHashCode(MatchShortParameters.FieldWidth)))
            {
                result = new Vector2Int(currentCell.x + 1, currentCell.y);
                return true;
            }
                
            // bottom cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.y > 0
                && cells[currentCell.y - 1, currentCell.x] == FieldCellType.Road
                && !waypointsHashSet.Contains(
                    new Vector2Int(currentCell.x, currentCell.y - 1).GetHashCode(MatchShortParameters.FieldWidth)))
            {
                result = new Vector2Int(currentCell.x, currentCell.y - 1);
                return true;
            }

            // top cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.y + 1 < MatchShortParameters.FieldHeight
                && cells[currentCell.y + 1, currentCell.x] == FieldCellType.Road
                && !waypointsHashSet.Contains(
                    new Vector2Int(currentCell.x, currentCell.y + 1).GetHashCode(MatchShortParameters.FieldWidth)))
            {
                result = new Vector2Int(currentCell.x, currentCell.y + 1);
                return true;
            }

            return false;
        }
        
        // try to find any road near cell that is not
        private static bool GetAnyRoadNearCellExceptLastWayPoint(Vector2Int currentCell, FieldCellType[,] cells, List<Vector2Int> waypointsList, out Vector2Int result)
        {
            result = -Vector2Int.one;

            // left cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.x > 0
                && cells[currentCell.y, currentCell.x - 1] == FieldCellType.Road
                && (waypointsList.Count == 1
                    ||
                    waypointsList.Count > 1
                    && waypointsList[waypointsList.Count - 2].x != currentCell.x - 1
                    && waypointsList[waypointsList.Count - 2].y != currentCell.y))
            {
                result = new Vector2Int(currentCell.x - 1, currentCell.y);
                return true;
            }
                
            // right cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.x + 1 < MatchShortParameters.FieldWidth
                && cells[currentCell.y, currentCell.x + 1] == FieldCellType.Road
                && (waypointsList.Count == 1 
                    ||
                    waypointsList.Count > 1
                    && waypointsList[waypointsList.Count - 2].x != currentCell.x + 1
                    && waypointsList[waypointsList.Count - 2].y != currentCell.y))
            {
                result = new Vector2Int(currentCell.x + 1, currentCell.y);
                return true;
            }
                
            // bottom cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.y > 0
                && cells[currentCell.y - 1, currentCell.x] == FieldCellType.Road
                && (waypointsList.Count == 1
                    ||
                    waypointsList.Count > 1
                    && waypointsList[waypointsList.Count - 2].x != currentCell.x
                    && waypointsList[waypointsList.Count - 2].y != currentCell.y - 1))
            {
                result = new Vector2Int(currentCell.x, currentCell.y - 1);
                return true;
            }

            // top cell
            // take it if the cell is new to waypointsHashSet
            if (currentCell.y + 1 < MatchShortParameters.FieldHeight
                && cells[currentCell.y + 1, currentCell.x] == FieldCellType.Road
                && (waypointsList.Count == 1
                    ||
                    waypointsList.Count > 1
                    && waypointsList[waypointsList.Count - 2].x != currentCell.x
                    && waypointsList[waypointsList.Count - 2].y != currentCell.y + 1))
            {
                result = new Vector2Int(currentCell.x, currentCell.y + 1);
                return true;
            }

            return false;
        }

        private static List<Vector2Int> RemoveRedundantWayPoints(List<Vector2Int> wayPointsList)
        {
            List<Vector2Int> cleanWayPointsList = new List<Vector2Int>(wayPointsList.Count);

            for (int wayPointIndex = 0; wayPointIndex < wayPointsList.Count; wayPointIndex++)
            {
                // simply add first and second points
                if (wayPointIndex < 2)
                {
                    cleanWayPointsList.Add(wayPointsList[wayPointIndex]);
                    continue;
                }

                if (wayPointsList[wayPointIndex].x == cleanWayPointsList[cleanWayPointsList.Count - 1].x
                    && wayPointsList[wayPointIndex].x == cleanWayPointsList[cleanWayPointsList.Count - 2].x
                    ||
                    wayPointsList[wayPointIndex].y == cleanWayPointsList[cleanWayPointsList.Count - 1].y
                    && wayPointsList[wayPointIndex].y == cleanWayPointsList[cleanWayPointsList.Count - 2].y)
                {
                    cleanWayPointsList[cleanWayPointsList.Count - 1] = wayPointsList[wayPointIndex];
                }
                else
                {
                    cleanWayPointsList.Add(wayPointsList[wayPointIndex]);
                }
            }

            return cleanWayPointsList;
        }

        private static Vector3[] RevertPathAndAdjustEdgePoints(List<Vector2Int> waypointsFromCastle)
        {
            Vector3[] waypointsToCastle = new Vector3[waypointsFromCastle.Count];

            // fill array in inverted order
            for (int wayPointIndex = 0; wayPointIndex < waypointsFromCastle.Count; wayPointIndex++)
            {
                Vector2 currentWayPoint = waypointsFromCastle[waypointsFromCastle.Count - wayPointIndex - 1];
                waypointsToCastle[wayPointIndex] = currentWayPoint;
            }

            Vector3 shiftedFieldEnterPoint = ShiftPointToEdge(waypointsFromCastle[waypointsToCastle.Length - 1]);
            Vector3 shiftedCastlePoint = ShiftPointToEdge(waypointsFromCastle[0]);

            waypointsToCastle[0] = shiftedFieldEnterPoint;
            waypointsToCastle[waypointsToCastle.Length - 1] = shiftedCastlePoint;

            return waypointsToCastle;
        }

        private static Vector3 ShiftPointToEdge(Vector2Int point)
        {
            if (point.x == 0)
                return new Vector3(-0.5f, point.y);
            
            if (point.x == MatchShortParameters.FieldWidth - 1)
                return new Vector3(MatchShortParameters.FieldWidth - 1 - 0.5f, point.y);
            
            if (point.y == 0)
                return new Vector3(point.x, -0.5f);
            
            if (point.y == MatchShortParameters.FieldHeight - 1)
                return new Vector3(point.x, MatchShortParameters.FieldHeight - 1 - 0.5f);
            
            return new Vector3(point.x, point.y);
        }
    }
    */
}