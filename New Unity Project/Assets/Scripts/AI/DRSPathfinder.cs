using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class DRSPathfinder
{
    WorldGrid worldGrid;
    float[,] gridCosts;
    public TileType[,] gridMap;

    public List<Vector2> pathData;
    
    float maxCost;
    Vector2 movedPos;
    Vector2 positionToTry;
    Vector2 polarAsCart;

    float lengthOfSearchSq;
    float searchLength;
    float refineLength;
    float rotationInDegrees;
    float stepSize;
    double radian180;

    bool setup;

    // Start is called before the first frame update
    public DRSPathfinder()
    {
        setup = false;
    }

    public void SetupPathFinder()
    {
        if (!setup)
        {
            worldGrid = GameObject.Find("WorldGrid").GetComponent<WorldGrid>();
            gridCosts = new float[Convert.ToInt32(worldGrid.GridHeight), Convert.ToInt32(worldGrid.GridWidth)];
            gridMap = new TileType[Convert.ToInt32(worldGrid.GridHeight), Convert.ToInt32(worldGrid.GridWidth)];
            PreSearchMapCopy();
            searchLength = worldGrid.GridSquareSize * 1.1f;
            refineLength = worldGrid.GridSquareSize * 0.9f;
            lengthOfSearchSq = (worldGrid.GridSquareSize * worldGrid.GridSquareSize) + (worldGrid.GridSquareSize * worldGrid.GridSquareSize);
            rotationInDegrees = 15.0f;
            stepSize = rotationInDegrees * Mathf.Deg2Rad;
            radian180 = 180 * Mathf.Deg2Rad;
            setup = true;
        }
    }

    void PreSearchMapCopy()
    {
        for (int x = 0; x < worldGrid.GridWidth; x++)
        {
            for (int y = 0; y < worldGrid.GridHeight; y++)
            {
                gridMap[x, y] = worldGrid.gridMap[x, y];
            }
        }
    }

    public bool FindPath(Vector2 startPosition, Vector2 endPosition)
    {
        PreSearchMapCopy();

        pathData = new List<Vector2>();
        ResetCollisionMap();
        ResetCostArray();

        List<Vector2> pathSearch = new List<Vector2>();

        bool success = Search(startPosition, endPosition, true, ref pathSearch);

        if (!(pathSearch.Count == 0))
        {
            pathSearch.Add(startPosition);
            
            pathSearch = EnhancePath(pathSearch);

            pathData = RefinePath(pathSearch);
        }

        return success;
    }

    bool Search(Vector2 currentPosition, Vector2 targetPosition, bool orderOfRotation, ref List<Vector2> path)
    {
        // Check if the cell is valid, if not exit out 
        if (!IsCellValid(currentPosition) || !IsCellValid(targetPosition))
        {
            return false;
        }

        currentPosition.x = Convert.ToInt32(currentPosition.x / worldGrid.GridSquareSize + (worldGrid.GridSquareSize / 2));
        currentPosition.y = Convert.ToInt32(currentPosition.y / worldGrid.GridSquareSize + (worldGrid.GridSquareSize / 2));

        // Check if we ahave reached the target position
        Vector2 search = targetPosition - currentPosition;
        if (search.sqrMagnitude <= lengthOfSearchSq)
        {
            // Ensure we can move onto the end location
            if (IsMoveAllowed(currentPosition, targetPosition))
            {
                // Add the target position and the current position to the path that needs travelling
                path.Add(targetPosition);
                path.Add(currentPosition);
                // Find the maximum cost we will allow
                SetMaxCost();

                // Return that we have succeeded
                return true;
            }
        }

        // Not at the end yet, keep searching.
        search.Normalize();

        Vector2 unitVectorToTarget = search;
        Vector2 vectorToTarget = search * searchLength;
        movedPos = currentPosition + vectorToTarget;

        // Ensure we can make this move
        if (IsPositionInMapBounds(movedPos) && IsMoveAllowed(currentPosition, movedPos))
        {
            // Try and search to this new position
            if (Search(movedPos, targetPosition, !orderOfRotation, ref path))
            {
                if (gridCosts[Convert.ToInt32(currentPosition.x / worldGrid.GridSquareSize), Convert.ToInt32(currentPosition.y / worldGrid.GridSquareSize)] <= maxCost)
                {
                    path.Add(currentPosition);
                }

                // Begin to unwind as we have found the end.
                return true;
            }
        }

        Vector2 polarVectorPositive = new Vector2(0.0f, searchLength);

        float dotRight = Vector2.Dot(unitVectorToTarget, new Vector2(1.0f, 0.0f));
        float dotUp = Vector2.Dot(unitVectorToTarget, new Vector2(0.0f, 1.0f));
        float startRadians = 3.14f;

        // Rotate our vector by the start radians
        if (dotUp > 0.0f)
        {
            polarVectorPositive.x += startRadians;
        }
        else
        {
            polarVectorPositive.x -= startRadians;
        }

        Vector2 polarVecNegative = polarVectorPositive;

        bool rotationComplete = false;
        bool positiveComplete = false;
        bool negativeComplete = false;

        while (!rotationComplete)
        {
            if (orderOfRotation)
            {
                polarVectorPositive.x += stepSize;

                if (!positiveComplete && polarVectorPositive.x < radian180)
                {
                    polarAsCart = new Vector2(polarVectorPositive.y * Mathf.Cos(polarVectorPositive.x), polarVectorPositive.y * Mathf.Sin(polarVectorPositive.x));

                    positionToTry = currentPosition + polarAsCart;

                    if (IsPositionInMapBounds(positionToTry) && IsMoveAllowed(currentPosition, positionToTry))
                    {
                        if (Search(positionToTry, targetPosition, !orderOfRotation, ref path))
                        {
                            if (gridCosts[Convert.ToInt32(currentPosition.x / worldGrid.GridSquareSize), Convert.ToInt32(currentPosition.y / worldGrid.GridSquareSize)] <= maxCost)
                            {
                                path.Add(currentPosition);
                            }

                            return true;
                        }
                    }
                }
                else
                {
                    positiveComplete = true;
                }

                orderOfRotation = false;
            }

            if (!orderOfRotation)
            {
                polarVecNegative.x -= stepSize;

                polarAsCart = new Vector2(polarVecNegative.y * Mathf.Cos(polarVecNegative.x), polarVecNegative.y * Mathf.Sin(polarVecNegative.x));

                positionToTry = currentPosition + polarAsCart;

                if (!negativeComplete && polarVecNegative.x > -radian180 - startRadians)
                {
                    if (IsPositionInMapBounds(positionToTry) && IsMoveAllowed(currentPosition, positionToTry))
                    {
                        if (Search(positionToTry, targetPosition, !orderOfRotation, ref path))
                        {
                            if (gridCosts[Convert.ToInt32(currentPosition.x / worldGrid.GridSquareSize), Convert.ToInt32(currentPosition.y / worldGrid.GridSquareSize)] <= maxCost)
                            {
                                path.Add(currentPosition);
                            }

                            return true;
                        }
                    }
                }
                else
                {
                    negativeComplete = true;
                }

                orderOfRotation = true;
            }

            rotationComplete = positiveComplete && negativeComplete;
        }

        // No path here
        return false;
    }

    public void RenderDRSPath()
    {
        for (int i = 0; i < pathData.Count -1; i++)
        {
            Vector3 start = new Vector3(pathData[i].x, 0, pathData[i].y);
            Vector3 end = new Vector3(pathData[i + 1].x, 0, pathData[i + 1].y); 
            Debug.DrawLine(start, end, Color.red);
        }
    }

    List<Vector2> EnhancePath(List<Vector2> tempPathData)
    {
        return tempPathData;
    }

    List<Vector2> RefinePath(List<Vector2> pathDataEnhanced)
    {
        return pathDataEnhanced;
    }

    bool IsCellValid(Vector2 currentPosition)
    {
        int x = Convert.ToInt32(currentPosition.x / worldGrid.GridSquareSize);
        int y = Convert.ToInt32(currentPosition.y / worldGrid.GridSquareSize);

        // We've hit a blocked space and should exit out
        if (gridMap[x, y] == TileType.TILETYPE_BLOCKED)
        {
            return false;
        }

        // We've already checked here, add a cost for rechecking and exit out 
        if (gridMap[x, y] == TileType.TILETYPE_EMPTY_CHECKED)
        {
            gridCosts[x, y] += 0.5f;
            return false;
        }

        gridCosts[x, y]++;
        return true;
    }

    bool IsPositionInMapBounds(Vector2 position)
    {
        return (position.x < worldGrid.GridWidth &&
            position.x > 0.0f &&
            position.y < worldGrid.GridHeight &&
            position.y > 0.0f);
    }

    bool IsMoveAllowed(Vector2 fromPos, Vector2 toPos)
    {
        int toX = Convert.ToInt32(toPos.x / worldGrid.GridSquareSize);
        int toY = Convert.ToInt32(toPos.y / worldGrid.GridSquareSize);

        if (gridMap[toX, toY] == TileType.TILETYPE_BLOCKED)
        {
            return false;
        }

        return true;
    }

    void SetMaxCost()
    {
        float totalCost = 0.0f;
        int numOfCosts = 0;

        for (int x = 0; x < worldGrid.GridWidth; x++)
        {
            for (int y = 0; y < worldGrid.GridHeight; y++)
            {
                if (gridCosts[x, y] > 0)
                {
                    numOfCosts++;
                    totalCost += gridCosts[x, y];
                }
            }
        }

        maxCost = (totalCost / numOfCosts);
    }

    void OutputCosts(Vector2 startPosition, Vector2 endPosition)
    {

    }

    void ResetCostArray()
    {
        for (int x = 0; x < worldGrid.GridWidth; x++)
        {
            for (int y = 0; y < worldGrid.GridHeight; y++)
            {
                gridCosts[x, y] = 0.0f;
            }
        }
    }

    void ResetCollisionMap()
    {
        for (int x = 0; x < worldGrid.GridWidth; x++)
        {
            for (int y = 0; y < worldGrid.GridHeight; y++)
            {
                if (gridMap[x, y] != TileType.TILETYPE_BLOCKED)
                {
                    gridMap[x, y] = TileType.TILETYPE_EMPTY;
                }
            }
        }
    }

    void ResetCostArrayAndCollisionMap()
    {
        for (int x = 0; x < worldGrid.GridWidth; x++)
        {
            for (int y = 0; y < worldGrid.GridHeight; y++)
            {
                if (gridMap[x, y] != TileType.TILETYPE_BLOCKED)
                {
                    gridMap[x, y] = TileType.TILETYPE_EMPTY;
                    gridCosts[x, y] = 0.0f;
                }
            }
        }
    }

    bool RayCheck(Vector2 currentPos, Vector2 goalPos)
    {
        Vector2 betweenDirections = goalPos - currentPos;
        Vector2 positionToCheck = currentPos;
        float distance = betweenDirections.magnitude;
        Vector2 searchedDistance = new Vector2();
        betweenDirections.Normalize();
        while (distance > searchedDistance.magnitude)
        {
            positionToCheck += (betweenDirections * 0.7f);
            if (!IsMoveAllowed(positionToCheck, positionToCheck))
            {
                return false;
            }

            searchedDistance = positionToCheck - currentPos;
        }

        return true;
    }
}
