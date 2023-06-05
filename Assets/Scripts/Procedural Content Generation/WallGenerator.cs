using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WallGenerator : MonoBehaviour
{
    public GameObject wallTilemap;
    public GameObject topWallTilemap;

    public TileBase wallDiagonalCornerDownRight;
    public TileBase wallInnerCornerDownLeft;
    public TileBase wallInnerCornerDownRight;
    public TileBase wallInnerCornerTopLeft;
    public TileBase wallInnerCornerTopRight;
    public TileBase wallDiagonalCornerDownLeft;
    public TileBase wallDiagonalCornerUpRight;
    public TileBase wallDiagonalCornerUpRightPair;
    public TileBase wallDiagonalCornerUpLeft;
    public TileBase wallDiagonalCornerUpLeftPair;
    public TileBase wallFull;
    public TileBase wallBottom;
    public TileBase wallTop;
    public TileBase wallSideRight;
    public TileBase wallSideLeft;

    public TileBase topWall;
    public TileBase topWallCornerRight;
    public TileBase topWallCornerLeft;
    public TileBase topWallCornerLeftTop;
    public TileBase topWallCornerRightTop;

    public void CreateWalls(HashSet<Vector2Int> floorPositions)
    {
        var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionsList);
        var cornerWallPositions = FindWallsInDirections(floorPositions, Direction2D.diagonalDirectionsList);
        CreateBasicWall(basicWallPositions, floorPositions);
        CreateCornerWalls(cornerWallPositions, floorPositions);
    }

    private void CreateCornerWalls(HashSet<Vector2Int> cornerWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in cornerWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.eightDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            PaintSingleCornerWall(position, neighboursBinaryType);
        }
    }

    private void PaintSingleCornerWall(Vector2Int position, string neighboursBinaryType)
    {
        int typeASInt = Convert.ToInt32(neighboursBinaryType, 2);
        TileBase tile = null;
        TileBase tilePair = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeASInt))
        {
            tilePair = topWallCornerLeft;
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeASInt))
        {
            tilePair = topWallCornerRight;
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallInnerCornerTopLeft.Contains(typeASInt))
        {
            tilePair = topWallCornerLeftTop;
            tile = wallInnerCornerTopLeft;
        }
        else if (WallTypesHelper.wallInnerCornerTopRight.Contains(typeASInt))
        {
            tilePair = topWallCornerRightTop;
            tile = wallInnerCornerTopRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeASInt))
        {
            tilePair = wallDiagonalCornerUpRightPair;
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeASInt))
        {
            tilePair = wallDiagonalCornerUpLeftPair;
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeASInt))
        {
            tilePair = topWall;
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeASInt))
        {
            tilePair = topWall;
            tile = wallBottom;
        }

        if (tile != null) { 
            PaintSingleTile(wallTilemap.GetComponent<Tilemap>(), tile, position);
            //PaintSingleTile(colliderTilemap, tile, position);
        }
        if (tilePair != null)
            PaintSingleTile(topWallTilemap.GetComponent<Tilemap>(), tilePair, new Vector2Int(position.x, position.y + 1));
    }

    private void CreateBasicWall(HashSet<Vector2Int> basicWallPositions, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPositions)
        {
            string neighboursBinaryType = "";
            foreach (var direction in Direction2D.cardinalDirectionsList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition))
                {
                    neighboursBinaryType += "1";
                }
                else
                {
                    neighboursBinaryType += "0";
                }
            }
            PaintSingleBasicWall(position, neighboursBinaryType);
        }
    }

    private void PaintSingleBasicWall(Vector2Int position, string neighboursBinaryType)
    {
        int typeAsInt = Convert.ToInt32(neighboursBinaryType, 2);
        TileBase tile = null;
        TileBase tilePair = null;

        if (WallTypesHelper.wallTop.Contains(typeAsInt))
        {
            tilePair = topWall;
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideRight.Contains(typeAsInt))
        {
            tile = wallSideRight;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(typeAsInt))
        {
            tile = wallSideLeft;
        }
        else if (WallTypesHelper.wallBottom.Contains(typeAsInt))
        {
            tilePair = topWall;
            tile = wallBottom;
        }
        else if (WallTypesHelper.wallFull.Contains(typeAsInt))
        {
            tilePair = topWall;
            tile = wallFull;
        }

        if (tile != null)
        {
            PaintSingleTile(wallTilemap.GetComponent<Tilemap>(), tile, position);
            //PaintSingleTile(colliderTilemap, tile, position);
        }
        if (tilePair != null)
            PaintSingleTile(topWallTilemap.GetComponent<Tilemap>(), tilePair, new Vector2Int(position.x, position.y + 1));
    }

    public HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neighbourPosition = position + direction;
                if (floorPositions.Contains(neighbourPosition) == false)
                    wallPositions.Add(neighbourPosition);
            }
        }
        return wallPositions;
    }

    private void PaintSingleTile(Tilemap tilemap, TileBase tile, Vector2Int position)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    public static class Direction2D
    {
        public static List<Vector2Int> cardinalDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0,1), //UP
            new Vector2Int(1,0), //RIGHT
            new Vector2Int(0, -1), // DOWN
            new Vector2Int(-1, 0) //LEFT
        };

        public static List<Vector2Int> diagonalDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(1,1), //UP-RIGHT
            new Vector2Int(1,-1), //RIGHT-DOWN
            new Vector2Int(-1, -1), // DOWN-LEFT
            new Vector2Int(-1, 1) //LEFT-UP
        };

        public static List<Vector2Int> eightDirectionsList = new List<Vector2Int>
        {
            new Vector2Int(0,1), //UP
            new Vector2Int(1,1), //UP-RIGHT
            new Vector2Int(1,0), //RIGHT
            new Vector2Int(1,-1), //RIGHT-DOWN
            new Vector2Int(0, -1), // DOWN
            new Vector2Int(-1, -1), // DOWN-LEFT
            new Vector2Int(-1, 0), //LEFT
            new Vector2Int(-1, 1) //LEFT-UP

        };

        public static Vector2Int GetRandomCardinalDirection()
        {
            return cardinalDirectionsList[UnityEngine.Random.Range(0, cardinalDirectionsList.Count)];
        }
    }
}
