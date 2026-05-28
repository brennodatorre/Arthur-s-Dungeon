using System;
using System.Collections;
using System.Collections.Generic;

using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;


public class MapSceneManager : MonoBehaviour
{

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

    public Color startingDoorColor = Color.clear;
    public Color discoveredColor = Color.white;
    public Color undiscoveredColor = Color.clear;

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
        RoomTile firstRoom = roomGrid[mapSizeX/2, mapSizeY/2].buildFirstRoom();
        playerIcon.transform.position = firstRoom.transform.position;
        playerLocation.x = firstRoom.x;
        playerLocation.y = firstRoom.y;
        
        for (int i = 0; i < builders; i++)
        {
            buildDungeon(firstRoom, energy);
        }

        firstRoom.analyzeRoom();
    }






   public void makeGrid()
    {
        Debug.Log("Building original grid");

        roomGrid = new RoomTile[mapSizeX, mapSizeY];

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
    }

    /// <summary>
    /// This is what makes the actual dungeon based on the grid, called x times for x builders
    /// </summary>
    private void buildDungeon(RoomTile room, int energy, int previousRoom = -1) 
    {
        if (energy <= 0) {lookForNeighboors(room); return;}

        int randomDirection = -1;
        

        // higher chance to stay on same direction
        if (previousRoom != -1)
        {
            // Continue in the same direction
            randomDirection = UnityEngine.Random.Range(1, 101);
            if (randomDirection <= 50)
            {
                randomDirection = previousRoom; // Continue in the same direction
            }
            else
            {
                randomDirection = UnityEngine.Random.Range(1, 5); // Choose a new direction
            }

        } else 
        {
            randomDirection = UnityEngine.Random.Range(1, 5); // Choose a new direction
        }


        RoomTile nextRoom = null;

        // Determine the next room based on the random direction
        switch (randomDirection)
        {
            case 1: // North
                if (room.y < mapSizeY - 1) 
                {
                    nextRoom = roomGrid[room.x, room.y + 1];
                }
                break;

            case 2: // South
                if (room.y > 0) 
                {
                    nextRoom = roomGrid[room.x, room.y - 1];  
                }
                break;

            case 3: // West
                if (room.x > 0) 
                {
                    nextRoom = roomGrid[room.x - 1, room.y];   
                }
                break;

            case 4: // East
                if (room.x < mapSizeX - 1) 
                {
                    nextRoom = roomGrid[room.x + 1, room.y];
                }
                break;

        }
        
        lookForNeighboors(room);
        
        if (nextRoom == null) 
        {
            buildDungeon(room, energy ); // Try a different direction
            return;
        }

        

        if (nextRoom.roomState == RoomTile.RoomState.NOTSET) 
        {
            nextRoom.buildRoom();

            buildDungeon(nextRoom, energy - 1, randomDirection); // Continue in same direction
        } else 
        {
            buildDungeon(nextRoom, energy - 1,  randomDirection); // Try a different direction
        }




    }

    private void lookForNeighboors(RoomTile room)
    {
        int x = room.x;
        int y = room.y;

        Color doorColor = startingDoorColor;
       

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
                    roomGrid[x,y].GetComponent<Image>().color = undiscoveredColor;
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
                    room.GetComponent<Image>().color = discoveredColor;
                    room.analyzeRoom();
                }
                else if (room.roomState == RoomTile.RoomState.UNDISCOVERED)
                {
                    room.GetComponent<Image>().color = undiscoveredColor;
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

}
 