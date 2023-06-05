using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class RoomGenerator : MonoBehaviour
{

    public GameObject mapGrid;
    public GameObject floorTilemap;
    [SerializeField]
    protected TileBase floorTile;

    public List<RoomPrefab> roomPrefabs;

    public int spaceHeight;
    public int spaceWidth;
    [SerializeField]
    protected int minimumRoomWidth;
    [SerializeField]
    protected int minimumRoomHeight;
    int padding = 1;
    List<Room> rooms = new List<Room>();
    
    //TEST
    private void Start()
    {

        //List<RoomPrefab> temp = GenerateRoom();
        //visualizeRoom(temp);
        //int[,] temp1 = roomFloor(temp);
        //roomPaint(temp1);
    }

    public void roomPaint(int[,] roomFloor)
    {
        for (int i = 0; i < spaceHeight; i++)
        {
            for (int j = 0; j < spaceWidth; j++)
            {
                if (roomFloor[i, j] != 0)
                {
                    paintRoomTiles(new Vector2Int(i, j), floorTile, floorTilemap.GetComponent<Tilemap>());
                }
            }
        }
    }
    //END OF TEST

    private void paintRoomTiles(Vector2Int position, TileBase tile, Tilemap tilemap)
    {
        var tilePosition = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePosition, tile);
    }

    //END OF TEST

    public bool validParameter()
    {
        if ((roomPrefabs.Count * minimumRoomWidth * minimumRoomHeight) > (spaceHeight * spaceWidth))
            return false;
        return true;
    }


    public int[,] roomFloor(List<RoomPrefab> inputRoomPrefabs)
    {
        int[,] result = new int[spaceWidth, spaceHeight];
        Debug.Log(result.Length);

        for (int i = 0; i < inputRoomPrefabs.Count; i++)
        {            
            Debug.Log(inputRoomPrefabs[i].thisRoom.topLeft);
            Debug.Log(inputRoomPrefabs[i].thisRoom.bottomRight);
            //Debug.Log(inputRoomPrefabs[i].thisRoom.topLeft + new Vector2Int(spaceHeight / 2, spaceWidth / 2));
            //Debug.Log(inputRoomPrefabs[i].thisRoom.bottomRight + new Vector2Int(spaceHeight / 2, spaceWidth / 2));
            for (int j = inputRoomPrefabs[i].thisRoom.topLeft.y; j >= inputRoomPrefabs[i].thisRoom.bottomRight.y; j--)
            {
                for (int k = inputRoomPrefabs[i].thisRoom.topLeft.x; k <= inputRoomPrefabs[i].thisRoom.bottomRight.x; k++)
                {
                    result[k, j] = i + 1;
                }
            }
        }
        return result;
    }

    public List<RoomPrefab> GenerateRoom()
    {
        int roomCount = roomPrefabs.Count;
        bool flag = true;
        List<RoomPrefab> result = new List<RoomPrefab>();
        int limiter = 0;
        int limiter1 = 0;
        while (rooms.Count != roomCount && limiter <= 2000)
        {
            limiter1 = 0;
            rooms = BinarySpacePartitionRecursive(new Room
            {
                topLeft = new Vector2Int(0, 0),
                bottomRight = new Vector2Int(spaceWidth, spaceHeight),
                roomSize = new Vector2Int(spaceWidth, spaceHeight),
                roomFlatArea = (spaceWidth) * (spaceHeight)
            }, Mathf.CeilToInt(Mathf.Log(roomCount, 2)));
            while (rooms.Count != roomCount && limiter1 < (roomCount - rooms.Count))
            {
                for (int i = 0; i < rooms.Count; i++)
                {
                    if (rooms[i].roomSize.x >= minimumRoomWidth * 2 || rooms[i].roomSize.y >= minimumRoomHeight * 2)
                    {
                        Room tempRoom = rooms[i];
                        rooms.RemoveAt(i);
                        rooms.AddRange(BinarySpacePartitionRecursive(tempRoom, 1));
                    }
                }
                limiter1++;
            }
            limiter++;
        }
        rooms = sortRoom(rooms);
        if (roomCount != rooms.Count)
            Debug.Log("Try Adding spaceWidth and spaceHeight Parameter Value");

        for (int i = 0; i < roomCount; i++)
        {
            createRoom(roomPrefabs[i], new Vector2Int(rooms[i].topLeft.x + rooms[i].roomSize.x / 2, rooms[i].topLeft.y + rooms[i].roomSize.y / 2));
            result.Add(roomPrefabs[i]);
        }
        return result;
    }

    public void visualizeRoom(List<RoomPrefab> inputRoomPrefabs)
    {
        foreach (var item in inputRoomPrefabs)
        {
            RoomPrefab tempPrefab = Instantiate(item);
            tempPrefab.transform.parent = mapGrid.transform;
            tempPrefab.transform.position = new Vector3(item.RoomCenter.x, item.RoomCenter.y, 0);
        }
    }

    private List<Room> BinarySpacePartitionRecursive(Room inputRoom, int index)
    {
        List<Room> result = new List<Room>();
        List<Room> temp = new List<Room>();
        if (index <= 0 || (inputRoom.roomSize.x < minimumRoomWidth * 2 && inputRoom.roomSize.y < minimumRoomHeight * 2))
        {
            result.Add(inputRoom);
            return result;
        }

        if (inputRoom.roomSize.y >= minimumRoomHeight && inputRoom.roomSize.x >= minimumRoomWidth)
        {
            if (UnityEngine.Random.value > 0.5f)
            {
                if (inputRoom.roomSize.y >= minimumRoomHeight * 2)
                {
                    temp = splitRoom(inputRoom, true);
                }
                else if (inputRoom.roomSize.x >= minimumRoomWidth * 2)
                {
                    temp = splitRoom(inputRoom, false);
                }
                else if (inputRoom.roomSize.y >= minimumRoomHeight && inputRoom.roomSize.x >= minimumRoomWidth)
                {
                    result.Add(inputRoom);
                    return result;
                }
            }
            else
            {
                if (inputRoom.roomSize.x >= minimumRoomWidth * 2)
                {
                    temp = splitRoom(inputRoom, false);
                }
                else if (inputRoom.roomSize.y >= minimumRoomHeight * 2)
                {
                    temp = splitRoom(inputRoom, true);
                }
                else if (inputRoom.roomSize.y >= minimumRoomHeight && inputRoom.roomSize.x >= minimumRoomWidth)
                {
                    result.Add(inputRoom);
                    return result;
                }
            }
        }

        foreach (Room i in temp)
        {
            result.AddRange(BinarySpacePartitionRecursive(i, index - 1));
        }
        return result;
    }

    public List<Room> splitRoom(Room inputRoom, bool horizontal)
    {
        List<Room> result = new List<Room>();
        Vector2Int temp1 = Vector2Int.zero;
        Vector2Int temp2 = Vector2Int.zero;
        if(horizontal)
        {
            temp1.Set(inputRoom.bottomRight.x, UnityEngine.Random.Range(inputRoom.topLeft.y + minimumRoomHeight, inputRoom.bottomRight.y - minimumRoomHeight));
            temp2.Set(inputRoom.topLeft.x, temp1.y);
        }
        else
        {
            temp1.Set(UnityEngine.Random.Range(inputRoom.topLeft.x + minimumRoomWidth, inputRoom.bottomRight.x - minimumRoomWidth ), inputRoom.bottomRight.y);
            temp2.Set(temp1.x, inputRoom.topLeft.y);
        }
        result.Add(new Room
        {
            topLeft = inputRoom.topLeft,
            bottomRight = temp1,
            roomSize = new Vector2Int(temp1.x - inputRoom.topLeft.x, temp1.y - inputRoom.topLeft.y),
            roomFlatArea = (temp1.x - inputRoom.topLeft.x) * (temp1.y - inputRoom.topLeft.y)
        });
        result.Add(new Room
        {
            topLeft = temp2,
            bottomRight = inputRoom.bottomRight,
            roomSize = new Vector2Int(inputRoom.bottomRight.x - temp2.x, inputRoom.bottomRight.y - temp2.y),
            roomFlatArea = (inputRoom.bottomRight.x - temp2.x) * (inputRoom.bottomRight.y - temp2.y)
        });
        return result;
    }

    public List<Room> sortRoom(List<Room> rooms)
    {
        List<Room> result = rooms;
        for(int i = 0; i < result.Count; i++)
        {
            for(int j=0; j < result.Count - 1; j++)
            {
                if (result[j].roomFlatArea > result[j + 1].roomFlatArea)
                {
                    Room temp = result[j];
                    result[j] = result[j + 1];
                    result[j + 1] = temp;
                }
            }
        }
        return result;
    }

    private void createRoom(RoomPrefab inputRoomPrefab, Vector2Int roomCenter)
    {
        inputRoomPrefab.RoomCenter = roomCenter + new Vector2Int(spaceWidth / -2, spaceHeight / -2);
        Vector2Int tempTopLeft = roomCenter + inputRoomPrefab.TopLeft;
        Vector2Int tempBottomRight = roomCenter + inputRoomPrefab.BottomRight;
        inputRoomPrefab.thisRoom = new Room
        {
            topLeft = tempTopLeft,
            bottomRight = tempBottomRight,
            roomSize = tempBottomRight - tempTopLeft,
            roomFlatArea = (tempBottomRight.x - tempTopLeft.x) * (tempBottomRight.y - tempTopLeft.y)
        };
    }
}
