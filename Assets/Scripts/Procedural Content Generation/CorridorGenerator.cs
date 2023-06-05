using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CorridorGenerator : MonoBehaviour
{
    [SerializeField]
    protected WallGenerator myWallGenerator;
    [SerializeField]
    protected RoomGenerator myRoomGenerator;
    [SerializeField]
    protected LSystemGenerator lSystem;

    public GameObject corridorTilemap;
    [SerializeField]
    protected TileBase floorTile;
    public int iterationLimit;

    [SerializeField]
    protected bool randomStartPosition;

    protected List<Vector3> positions = new List<Vector3>();

    public int Length = 5;
    public int LengthMin = 5;
    public int Width = 3;
    public float angle = 60;

    public List<string> lSystemString = new List<string>();

    public HashSet<Vector2Int> generateCorridor()
    {
        lSystemString = lSystem.getSentence(lSystem.rootSentence, iterationLimit);
        string sequence = lSystemString[iterationLimit];
        return visualizer(sequence);
    }

    public void visualizeCorridor(HashSet<Vector2Int> floorPosition)
    {
        foreach (var item in floorPosition)
        {
            paintCorridorTiles(item + new Vector2Int(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceHeight / -2), floorTile, corridorTilemap.GetComponent<Tilemap>());
        }
    }

    public void generateWalls(List<RoomPrefab> roomPrefabs, HashSet<Vector2Int> floorPosition)
    {
        int k = 0;
        foreach (var item in roomPrefabs)
        {
            k++;
            for (int i = item.thisRoom.topLeft.x; i <= item.thisRoom.bottomRight.x; i++)
            {
                for (int j = item.thisRoom.topLeft.y; j >= item.thisRoom.bottomRight.y; j--)
                {
                    if (!floorPosition.Contains(new Vector2Int(i, j)))
                        floorPosition.Add(new Vector2Int(i, j));
                }
            }
        }
        myWallGenerator.CreateWalls(floorPosition);
        myWallGenerator.wallTilemap.transform.position = new Vector3(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceHeight / -2, 0);
        myWallGenerator.topWallTilemap.transform.position = new Vector3(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceHeight / -2, 0);
    }

    public bool checkConnectedRooms(RoomPrefab roomPrefabs, HashSet<Vector2Int> floorPosition)
    {
        Vector2Int TopLeft = roomPrefabs.thisRoom.topLeft;
        Vector2Int BottomRight = roomPrefabs.thisRoom.bottomRight;
        TopLeft += new Vector2Int(-1, 1);
        BottomRight += new Vector2Int(1, -1);
        for (int i = TopLeft.x + 2; i <= BottomRight.x - 2; i++)
        {
            //Debug.Log(new Vector2Int(i, TopLeft.y));
            if (floorPosition.Contains(new Vector2Int(i, TopLeft.y)))
                if (floorPosition.Contains(new Vector2Int(i + 1, TopLeft.y)) || floorPosition.Contains(new Vector2Int(i - 1, TopLeft.y)))
                    return true;
        }
        for (int i = TopLeft.y - 2; i >= BottomRight.y + 2; i--)
        {
            if (floorPosition.Contains(new Vector2Int(TopLeft.x, i)))
                if (floorPosition.Contains(new Vector2Int(TopLeft.x, i + 1)) || floorPosition.Contains(new Vector2Int(TopLeft.x, i - 1)))
                    return true;
        }
        for (int i = TopLeft.x + 2; i <= BottomRight.x - 2; i++)
        {
            if (floorPosition.Contains(new Vector2Int(i, BottomRight.y)))
                if (floorPosition.Contains(new Vector2Int(i + 1, BottomRight.y)) || floorPosition.Contains(new Vector2Int(i - 1, BottomRight.y)))
                    return true;
        }
        for (int i = TopLeft.y - 2; i >= BottomRight.y + 2; i--)
        {
            if (floorPosition.Contains(new Vector2Int(BottomRight.x, i)))
                if (floorPosition.Contains(new Vector2Int(BottomRight.x, i + 1)) || floorPosition.Contains(new Vector2Int(BottomRight.x, i - 1)))
                    return true;
        }
        return false;
    }

    private HashSet<Vector2Int> visualizer(string sequence)
    {
        HashSet<Vector2Int> floorPosition = new HashSet<Vector2Int>();
        Stack<AgentParameter> savePoints = new Stack<AgentParameter>();
        Vector3 currentPosition;
        if (randomStartPosition)
            currentPosition = new Vector3(Random.Range(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceWidth / 2), Random.Range(myRoomGenerator.spaceHeight / -2, myRoomGenerator.spaceHeight / 2), 0);
        else
            currentPosition = new Vector3(myRoomGenerator.spaceWidth / 2, myRoomGenerator.spaceHeight / 2, 0);
        Vector3 direction = new Vector3(1, 0, 0);
        Vector3 tempPosition = Vector3.zero;
        int i = 0;

        positions.Add(currentPosition);
        foreach (var symbol in sequence)
        {
            EncodeLetter encoding = (EncodeLetter)symbol;
            switch (encoding)
            {
                case EncodeLetter.save:
                    if(Length < LengthMin)
                    {
                        savePoints.Push(new AgentParameter
                        {
                            position = currentPosition,
                            direction = direction,
                            length = LengthMin
                        });
                    }
                    else
                    {
                        savePoints.Push(new AgentParameter
                        {
                            position = currentPosition,
                            direction = direction,
                            length = Length - 1
                        });
                    }
                    break;
                case EncodeLetter.load:
                    if (savePoints.Count > 0)
                    {
                        var agentParameter = savePoints.Pop();
                        Length = agentParameter.length;
                    }
                    else
                    {
                        throw new System.Exception("Dont have point in our stack");
                    }
                    break;
                case EncodeLetter.forward:
                    tempPosition = currentPosition;
                    currentPosition += direction * Length;
                    if (!inBound(currentPosition))
                    {
                        direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, -1)) * direction;
                        currentPosition = direction * Length + tempPosition;
                        if (!inBound(currentPosition))
                        {
                            direction = Quaternion.AngleAxis(angle * -2, new Vector3(0, 0, -1)) * direction;
                            currentPosition = direction * Length + tempPosition;
                        }
                    }
                    drawCorridor(tempPosition, currentPosition, floorTile, floorPosition);
                    positions.Add(currentPosition);
                    break;
                case EncodeLetter.turnRight:
                    direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, -1)) * direction;
                    break;
                case EncodeLetter.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, new Vector3(0, 0, -1)) * direction;
                    break;
            }
            i++;
        }
        return floorPosition;
    }

    public void connectedRoom(int[,] roomFloor, string sequence)
    {
        string result = "";
        Stack<AgentParameter> savePoints = new Stack<AgentParameter>();
        Vector3 currentPosition = new Vector3(myRoomGenerator.spaceWidth / 2, myRoomGenerator.spaceHeight / 2, 0);
        Vector3 direction = new Vector3(1, 0, 0);
        Vector3 tempPosition = Vector3.zero;
        int i = 0;

        foreach (var symbol in sequence)
        {
            result += symbol;
            EncodeLetter encoding = (EncodeLetter)symbol;
            switch (encoding)
            {
                case EncodeLetter.forward:
                    tempPosition = currentPosition;
                    currentPosition += direction * Length;
                    if (!inBound(currentPosition))
                    {
                        direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, -1)) * direction;
                        currentPosition = direction * Length + tempPosition;
                        if (!inBound(currentPosition))
                        {
                            direction = Quaternion.AngleAxis(angle * -2, new Vector3(0, 0, -1)) * direction;
                            currentPosition = direction * Length + tempPosition;
                        }
                    }
                    //Debug.Log(tempPosition + " " + currentPosition);
                    //if (roomFloor[(int)currentPosition.x, (int)currentPosition.y] != 0)
                    //{
                    //    result += roomFloor[(int)currentPosition.x, (int)currentPosition.y].ToString();
                    //}
                    positions.Add(currentPosition);
                    break;
                case EncodeLetter.turnRight:
                    direction = Quaternion.AngleAxis(angle, new Vector3(0, 0, -1)) * direction;
                    //Debug.Log(direction + " " + i);
                    break;
                case EncodeLetter.turnLeft:
                    direction = Quaternion.AngleAxis(-angle, new Vector3(0, 0, -1)) * direction;
                    //Debug.Log(direction + " " + i);
                    break;
            }
            i++;
        }
        Debug.Log(result);
    }


    private void drawCorridor(Vector3 startPosition, Vector3 endPosition, TileBase floorTile, HashSet<Vector2Int> floorPosition)
    {
        int X0 = (int)startPosition.x;
        int Y0 = (int)startPosition.y;
        int X1 = (int)endPosition.x;
        int Y1 = (int)endPosition.y;

        int dx = X1 - X0;
        int dy = Y1 - Y0;

        int steps = Mathf.Abs(dx) > Mathf.Abs(dy) ? Mathf.Abs(dx) : Mathf.Abs(dy);

        int Xinc = dx / steps;
        int Yinc = dy / steps;

        int X = X0;
        int Y = Y0;

        Vector3Int tempPosition = Vector3Int.zero;
        for (int i = 0; i <= steps; i++)
        {
            tempPosition.x = X;
            tempPosition.y = Y;
            paintCorridor(tempPosition, floorTile, corridorTilemap.GetComponent<Tilemap>(), floorPosition);
            X += Xinc;
            Y += Yinc;
        }
    }

    private void paintCorridor(Vector3Int position, TileBase tile, Tilemap tilemap, HashSet<Vector2Int> floorPosition)
    {
        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                floorPosition.Add(new Vector2Int(position.x + i, position.y + j));
            }
        }
    }

    private void paintCorridorTiles(Vector2Int position, TileBase tile, Tilemap tilemap)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    private bool inBound(Vector3 curPosition)
    {
        if (curPosition.x >= myRoomGenerator.spaceWidth || curPosition.x <= 0)
            return false;
        if (curPosition.y >= myRoomGenerator.spaceHeight || curPosition.y <= 0)
            return false;
        return true;
    }

    private enum EncodeLetter
    {
        unknown = '1',
        save = '[',
        load = ']',
        forward = 'F',
        turnRight = '+',
        turnLeft = '-'
    }
}
