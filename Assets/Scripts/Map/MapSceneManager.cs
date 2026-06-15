using System;
using System.Collections;
using System.Collections.Generic;


using UnityEngine;

using UnityEngine.UI;

using static RoomTile;
using Random = UnityEngine.Random;


public class MapSceneManager : MonoBehaviour
{


    public enum Direction {NORTH, SOUTH, WEST, EAST, NULL}

    public static MapSceneManager Instance;
    public GameObject roomPrefab;
    public GameObject gridObject;
    public GameObject playerIcon;
    public float roomSpacing = 10f; 

    RoomTile[,] roomGrid;
    public Vector2Int playerLocation;
    
    public List<RoomTile> dungeonRooms;
    public int roomsDiscovered;


    public int mapSizeX;
    public int mapSizeY;
    public int energy;
    public int builders = 1;

    public Color hiddenDoorColor = Color.clear;
    public Color FloorColor = Color.white;
    public Color undiscoveredRoomColor = Color.clear;



    [Space (10)]
    [Header("Movement")]
    public bool isMoving;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0,0,1,1);
    public float moveDuration = 0.5f;  
    public AudioClip footstepAudio; 


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Instance.StartCoroutine(Instance.rebuildMap());

            Destroy(gameObject); // Avoid duplicates
            return;
        }



        makeGrid();
        
    }






   public void makeGrid()
    {
        Debug.Log("Building original grid");

        roomGrid = new RoomTile[mapSizeX, mapSizeY];

        //gets room spacing
        RectTransform prefabRect = roomPrefab.GetComponent<RectTransform>();
        float tileDistance = prefabRect.sizeDelta.x + roomSpacing;
        
        // Create room tiles
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                GameObject newRoom = Instantiate(roomPrefab, gridObject.transform);
                newRoom.name = "";
                RectTransform rect = newRoom.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    x * tileDistance,
                    y * tileDistance
                );

                roomGrid[x, y] = newRoom.GetComponent<RoomTile>();
                roomGrid[x, y].x = x;
                roomGrid[x,y].y = y;

                
                
            }
        }


        // Center the grid on the map
        gridObject.transform.localPosition = -1 * roomGrid[mapSizeX /2, mapSizeY/2].transform.localPosition ;


        // sets first room & player pos
        RoomTile firstRoom = roomGrid[mapSizeX/2, mapSizeY/2];
        firstRoom.roomState = RoomState.DISCOVERED;
        firstRoom.GetComponent<Image>().color = FloorColor;
        firstRoom.GetComponent<Button>().interactable = true;


        playerIcon.transform.position = firstRoom.transform.position;
        playerLocation.x = firstRoom.x;
        playerLocation.y = firstRoom.y;


        // calls the worm builders that dig the rooms
        for (int i = 0; i < builders; i++)
        {
            buildDungeon(firstRoom, energy);
        }


        firstRoom.CheckForDoors();


    }

    /// <summary>
    /// This is what makes the actual dungeon based on the grid, called x times for x builders. 
    /// </summary>
    private void buildDungeon(RoomTile room, int energy, Direction prevDirection = Direction.NULL) 
    {
        ConnectRoom(room);

        if (energy <= 0) { return;}

        // higher chance to stay on same direction
        // for dungeons that are less compact 
        Direction randomDirection;
        if (prevDirection != Direction.NULL)
        {
            // Continue in the same direction
            int rand = UnityEngine.Random.Range(1, 101);
            if (rand <= 50)
            {
                randomDirection = prevDirection; // go back
            }
            else
            {
                randomDirection = GetNewRandDirection(prevDirection);
            }

        } else 
        {
            randomDirection = GetNewRandDirection(prevDirection);
        }




        // Determine the next room to be carved based on the random direction 
        RoomTile nextRoom = null;
        switch (randomDirection){
            case Direction.NORTH:
                if (room.y < mapSizeY - 1) {nextRoom = roomGrid[room.x, room.y + 1];}
                break;
            case Direction.SOUTH: // South
                if (room.y > 0) { nextRoom = roomGrid[room.x, room.y - 1];  }
                break;
            case Direction.WEST: // West
                if (room.x > 0) {    nextRoom = roomGrid[room.x - 1, room.y];   }
                break;
            case Direction.EAST: // East
                if (room.x < mapSizeX - 1) {nextRoom = roomGrid[room.x + 1, room.y];}
                break;
        }


        if (nextRoom == null) // out of grid
        {
            buildDungeon(room, energy ); // try again
            return;
        }




        // Set up the next room
        if (nextRoom.roomState == RoomState.NOTSET)
        {        
            nextRoom.name = nextRoom.x + ", " + nextRoom.y;

            nextRoom.roomState = RoomState.UNDISCOVERED;

            nextRoom.GetComponent<Image>().color = MapSceneManager.Instance.undiscoveredRoomColor;
            nextRoom.GetComponent<Button>().interactable = false;

            nextRoom.roomType = MySceneManager.Instance.getNextScene() ;
            

            dungeonRooms.Add(nextRoom);
        }




        buildDungeon(nextRoom, energy - 1, randomDirection);



    }


    /// <summary>
    /// Connects rooms and set their doors
    /// </summary>
    private void ConnectRoom(RoomTile room)
    {
        int x = room.x;
        int y = room.y;

        Color doorColor = hiddenDoorColor;
       

        // Only set neighboors for rooms that are built
        if (room.roomState == RoomTile.RoomState.NOTSET) return;

        // sets north neighboor and doors if north room is built
        if((y < mapSizeY - 1) && roomGrid[x, y + 1].roomState != RoomTile.RoomState.NOTSET) {
            room.northRoom = roomGrid[x, y + 1];
            room.northRoom.southRoom = room;
            room.northDoor.color = doorColor;
            room.northRoom.southDoor.color = doorColor;
        
        }
        // sets south 
        if (y > 0 && roomGrid[x, y - 1].roomState != RoomTile.RoomState.NOTSET) {
            room.southRoom = roomGrid[x, y - 1];
            room.southRoom.northRoom = room;
            room.southDoor.color = doorColor;
            room.southRoom.northDoor.color = doorColor;
        }
        // sets west
        if (x > 0 && roomGrid[x - 1, y].roomState != RoomTile.RoomState.NOTSET) {
            room.westRoom = roomGrid[x - 1, y];
            room.westRoom.eastRoom = room;
            room.westDoor.color = doorColor;
            room.westRoom.eastDoor.color = doorColor;
        }
        // sets east
        if (x < mapSizeX - 1 && roomGrid[x + 1, y].roomState != RoomTile.RoomState.NOTSET) {
            room.eastRoom = roomGrid[x + 1, y];
            room.eastRoom.westRoom = room;
            room.eastDoor.color = doorColor;
            room.eastRoom.westDoor.color = doorColor;
        }

    }





    private IEnumerator rebuildMap()
    {
        isMoving = false;
        //re assigns grid and player icon
        while (gridObject == null || playerIcon == null)
        {
            yield return null;

            GameObject canvas = GameObject.FindGameObjectWithTag("MainCanvas");

            if (canvas != null)
            {
                Transform grid = canvas.transform.Find("Grid");
                Transform pIcon = canvas.transform.Find("player_lcon");

                if (grid != null)
                {
                    gridObject = grid.gameObject;
                }
                if (pIcon != null)
                {
                    playerIcon = pIcon.gameObject;
                }
            }
        }

        Debug.Log("map rebuild");

        RectTransform prefabRect = roomPrefab.GetComponent<RectTransform>();
        float tileDistance = prefabRect.sizeDelta.x + roomSpacing;

        dungeonRooms.Clear();

        
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                
                GameObject newRoom = Instantiate(roomPrefab, gridObject.transform);
                newRoom.name =  x + ", " + y;
                RectTransform rect = newRoom.GetComponent<RectTransform>();

                rect.anchoredPosition = new Vector2(
                    x * tileDistance,
                    y * tileDistance
                );

                RoomTile newTile = newRoom.GetComponent<RoomTile>();
                
                roomGrid[x,y].CopyRoomTile(newTile);
                roomGrid[x,y] = newTile;

                

                if (newTile.roomState != RoomTile.RoomState.NOTSET)
                {
                    dungeonRooms.Add(roomGrid[x,y]);
                }
                else
                {
                    roomGrid[x,y].GetComponent<Image>().color = undiscoveredRoomColor;
                    newRoom.name =  "";
                }


                
                
            }
        }

        // sets adjacent rooms reference
        foreach (var room in dungeonRooms)
        {
            int x = room.x;
            int y = room.y;

            int maxX = roomGrid.GetLength(0);
            int maxY = roomGrid.GetLength(1);

            if (y + 1 < maxY && roomGrid[x, y + 1] != null)
                room.northRoom = roomGrid[x, y + 1];

            if (y - 1 >= 0 && roomGrid[x, y - 1] != null)
                room.southRoom = roomGrid[x, y - 1];

            if (x + 1 < maxX && roomGrid[x + 1, y] != null)
                room.eastRoom = roomGrid[x + 1, y];

            if (x - 1 >= 0 && roomGrid[x - 1, y] != null)
                room.westRoom = roomGrid[x - 1, y];
        }



        // analyzes set rooms
        foreach (var room in dungeonRooms)
        {
            if (room.roomState == RoomTile.RoomState.DISCOVERED)
            {
                room.GetComponent<Image>().color = FloorColor;
                room.CheckForDoors();
            }
            else if (room.roomState == RoomTile.RoomState.UNDISCOVERED)
            {
                room.GetComponent<Image>().color = undiscoveredRoomColor;
            }
        }



        

        // Center the grid on the map
        gridObject.transform.localPosition = -1 * roomGrid[mapSizeX /2, mapSizeY/2].transform.localPosition ;
        playerIcon.transform.position = roomGrid[playerLocation.x, playerLocation.y].transform.position;
        

    }



    #region "Path Finding"

    public List<RoomTile> FindPath(RoomTile start, RoomTile end)
    {

        foreach (RoomTile room in dungeonRooms)
        {
            room.gCost = int.MaxValue;
            room.parent = null;
        }

        start.gCost = 0;

        List<RoomTile> openList = new List<RoomTile>();
        List<RoomTile> closedList = new List<RoomTile>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            RoomTile current = openList[0];

            foreach (var node in openList)
            {
                if (node.fCost < current.fCost || node.fCost == current.fCost && node.hCost < current.hCost)
                {
                    current = node;
                }
            }

            openList.Remove(current);
            closedList.Add(current);

            if (current == end)
            {
                return CalculatePath(end);
            }

            foreach (RoomTile neighbor in current.getDiscoveredNeighbors())
            {
                if (closedList.Contains(neighbor))
                    continue;

                int tentativeGCost = current.gCost + 1;

                if (!openList.Contains(neighbor) || tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Mathf.Abs(neighbor.x - end.x) + Mathf.Abs(neighbor.y - end.y);
                    neighbor.calculateFCost();
                    neighbor.parent = current;

                    if (!openList.Contains(neighbor))
                        openList.Add(neighbor);
                }
            }
            
        }


        return null;
    }



    private List<RoomTile> CalculatePath(RoomTile endNode)
    {
        List<RoomTile> path = new List<RoomTile>();
        path.Add(endNode);

        RoomTile currentNode = endNode;

        while (currentNode.parent != null)
        {
            path.Add(currentNode.parent);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }


    public IEnumerator MovePlayer(List<RoomTile> path, RoomTile destination, bool firstTimeEntering)
    {


        //plays footssteps audio while moving
        AudioSource audioSource = AudioManager.Instance.CreateAndPlaySound(footstepAudio);

        foreach (RoomTile room in path)
        {
            Vector3 startPos = playerIcon.transform.position;
            Vector3 endPos = room.transform.position;

            float time = 0;

            while (time < moveDuration)
            {
                time += Time.deltaTime;

                float t = time / moveDuration;

                // evaluate curve
                float curveT = moveCurve.Evaluate(t);

                playerIcon.transform.position = Vector3.Lerp(startPos, endPos, curveT);

                yield return null;
            }

            playerIcon.transform.position = endPos;
        }

        Destroy(audioSource); // stop footstep audio after moving
        
        if (firstTimeEntering) MySceneManager.Instance.openNextScene(destination.roomType);

        isMoving = false;

    }

    public RoomTile GetPlayerCurrentRoom()
    {
        return roomGrid[playerLocation.x, playerLocation.y];
    }

    #endregion



    private Direction GetNewRandDirection (Direction oldDirection)
    {

        Direction newD;

        do
        {
            // -1 accounts for NULL
            newD = (Direction) Random.Range( 0, Enum.GetValues(typeof(Direction)).Length -1);
        }
        while (newD == oldDirection);



        return newD;
    }

}
 