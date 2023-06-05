using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class ProceduralContentGeneration : MonoBehaviour
{
    protected GameObject mapGrid;
    [SerializeField]
    protected CorridorGenerator myCorridorGenerator;
    [SerializeField]
    protected RoomGenerator myRoomGenerator;
    [SerializeField]
    protected WallGenerator myWallGenerator;

    private void Awake()
    {
        mapGrid = new GameObject();
        mapGrid.gameObject.name = "MapGrid";
        mapGrid.AddComponent<Grid>();

        //Tilemap initialitation for room generator
        myRoomGenerator.mapGrid = mapGrid;
        myRoomGenerator.floorTilemap = new GameObject();
        myRoomGenerator.floorTilemap.gameObject.name = "Floor";
        myRoomGenerator.floorTilemap.AddComponent<Tilemap>();
        myRoomGenerator.floorTilemap.AddComponent<TilemapRenderer>();
        myRoomGenerator.floorTilemap.transform.parent = mapGrid.transform;

        //Tilemap initialitation for corridor generator
        myCorridorGenerator.corridorTilemap = new GameObject();
        myCorridorGenerator.corridorTilemap.gameObject.name = "Corridor";
        myCorridorGenerator.corridorTilemap.AddComponent<Tilemap>();
        myCorridorGenerator.corridorTilemap.AddComponent<TilemapRenderer>();
        myCorridorGenerator.corridorTilemap.transform.parent = GameObject.Find("MapGrid").transform;

        //Inisialisasi tilemap pada wall generator
        myWallGenerator.wallTilemap = new GameObject();
        myWallGenerator.wallTilemap.gameObject.name = "CorridorWall";
        myWallGenerator.wallTilemap.gameObject.layer = LayerMask.NameToLayer("Blocking");
        myWallGenerator.wallTilemap.AddComponent<Tilemap>();
        myWallGenerator.wallTilemap.AddComponent<TilemapRenderer>();
        myWallGenerator.wallTilemap.GetComponent<TilemapRenderer>().sortingLayerName = "floor";
        myWallGenerator.wallTilemap.GetComponent<TilemapRenderer>().sortingOrder = 1;
        myWallGenerator.wallTilemap.AddComponent<TilemapCollider2D>();
        myWallGenerator.wallTilemap.GetComponent<TilemapCollider2D>().usedByComposite = true;
        myWallGenerator.wallTilemap.AddComponent<Rigidbody2D>();
        myWallGenerator.wallTilemap.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        myWallGenerator.wallTilemap.AddComponent<CompositeCollider2D>();
        myWallGenerator.wallTilemap.transform.parent = mapGrid.gameObject.transform;

        myWallGenerator.topWallTilemap = new GameObject();
        myWallGenerator.topWallTilemap.gameObject.name = "CorridorTopWall";
        myWallGenerator.topWallTilemap.AddComponent<Tilemap>();
        myWallGenerator.topWallTilemap.AddComponent<TilemapRenderer>();
        myWallGenerator.topWallTilemap.GetComponent<TilemapRenderer>().sortingLayerName = "actor";
        myWallGenerator.topWallTilemap.GetComponent<TilemapRenderer>().sortingOrder = 1;
        myWallGenerator.topWallTilemap.transform.parent = mapGrid.gameObject.transform;


        List<RoomPrefab> tempRoomPrefabs = new List<RoomPrefab>();
        HashSet<Vector2Int> tempFloorPosition = new HashSet<Vector2Int>();
        if (!myRoomGenerator.validParameter())
            return;
        tempRoomPrefabs = myRoomGenerator.GenerateRoom();

        int limiter = 0;
        while (true && limiter <= 1000)
        {
            bool flag = true;
            tempFloorPosition = myCorridorGenerator.generateCorridor();
            foreach (var item in tempRoomPrefabs)
            {
                if (!myCorridorGenerator.checkConnectedRooms(item, tempFloorPosition))
                    flag = false;
            }
            if (flag)
                break;
            limiter++;
        }

        for(int i=0;i< tempRoomPrefabs.Count; i++)
        {
            Debug.Log(i);
            Debug.Log(tempRoomPrefabs[i].RoomCenter);
            Debug.Log(tempRoomPrefabs[i].thisRoom.topLeft + new Vector2Int(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceHeight / -2));
            Debug.Log(tempRoomPrefabs[i].thisRoom.bottomRight + new Vector2Int(myRoomGenerator.spaceWidth / -2, myRoomGenerator.spaceHeight / -2));
        }
        for (int i = 0; i <= myCorridorGenerator.iterationLimit; i++)
        {
            Debug.Log(myCorridorGenerator.lSystemString[i]);
        }

        myCorridorGenerator.visualizeCorridor(tempFloorPosition);
        myRoomGenerator.visualizeRoom(tempRoomPrefabs);
        myCorridorGenerator.generateWalls(tempRoomPrefabs, tempFloorPosition);
    }

    public void resetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
