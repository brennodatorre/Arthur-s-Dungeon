using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;



public class RoomTile : MonoBehaviour
{
    public enum RoomState
    {
        UNDISCOVERED,
        NOTSET, 
        DISCOVERED

    }


    [Space (10)]
    [Header("Doors")]
    public Image northDoor;
    public Image southDoor;
    public Image westDoor;
    public Image eastDoor;

    [Space (10)]
    [Header("Icons")]
    public Image combatIcon;
    public Image eventIcon;
    public bool hasBeenPeaked = false; 



    [Space (10)]
    [Header("Adjacent Rooms")]
    public RoomTile northRoom;
    public RoomTile southRoom;
    public RoomTile westRoom;
    public RoomTile eastRoom;


    public RoomState roomState = RoomState.NOTSET;
    public MySceneManager.SceneType roomType ;

    
    [Space (10)]
    [Header("Grid Data")]
    public int x;
    public int y;
    public RoomTile parent;
    [Tooltip ("distance from start")]
    public int gCost;
    [Tooltip ("estimated distance to goal")]
    public int hCost;
    [Tooltip ("g+h")]
    public int fCost;





    /// <summary>
    /// Walks to roomn clicked, with walk "animation" and 
    /// Enters Room clicked, discovers, analyse and set rooms around it.
    /// </summary>
    public void discoverRoom()
    {
        if (MapSceneManager.Instance.isMoving) return;
        MapSceneManager.Instance.isMoving = true;

        bool firstTimeEntering = false;

        if (roomState != RoomState.DISCOVERED)  
        {
            roomState = RoomState.DISCOVERED;

            GetComponent<Image>().color = MapSceneManager.Instance.discoveredColor;

            analyzeRoom();

            MapSceneManager.Instance.roomsDiscovered++;
            firstTimeEntering = true;
        }

        RoomTile currentRoom = MapSceneManager.Instance.GetPlayerCurrentRoom();

        List<RoomTile> path = MapSceneManager.Instance.FindPath(currentRoom, this);

        MapSceneManager.Instance.playerLocation.x = x;
        MapSceneManager.Instance.playerLocation.y = y;

        if (path != null)
        {
            MapSceneManager.Instance.StartCoroutine(
                MapSceneManager.Instance.MovePlayer(path, this, firstTimeEntering)
            );
        }


        

    }

    public void buildRoom()
    {
        this.name = x + ", " + y;
        roomState = RoomState.UNDISCOVERED;
        GetComponent<Image>().color = MapSceneManager.Instance.undiscoveredColor;
        GetComponent<Button>().interactable = false;

        roomType = MySceneManager.Instance.getNextScene() ;
        

        MapSceneManager.Instance.dungeonRooms.Add(this);
        


    }

    public RoomTile buildFirstRoom()
    {
        roomState = RoomState.DISCOVERED;
        GetComponent<Image>().color = MapSceneManager.Instance.discoveredColor;
        GetComponent<Button>().interactable = true;

        return this;

    }

    public void  analyzeRoom()
    {

        if (northRoom != null && northRoom.roomState != RoomState.NOTSET) {

            northDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(northRoom);
        }
        if (southRoom != null && southRoom.roomState != RoomState.NOTSET)
        {
            southDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(southRoom);
        }
        if (eastRoom != null && eastRoom.roomState != RoomState.NOTSET)
        {  
            eastDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(eastRoom);
        }
        if (westRoom != null && westRoom.roomState != RoomState.NOTSET)
        {     
            westDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(westRoom);
        }

        
    }

    public List<RoomTile>  getDiscoveredNeighbors()
    {
        List <RoomTile> neighbors = new List<RoomTile>();

        if (northRoom != null && northRoom.roomState == RoomState.DISCOVERED ) { 
            neighbors.Add(northRoom);
        }
        if (southRoom != null && southRoom.roomState == RoomState.DISCOVERED)
        {
            neighbors.Add(southRoom);;
        }
        if (eastRoom != null && eastRoom.roomState == RoomState.DISCOVERED)
        {
            neighbors.Add(eastRoom);
        }
        if (westRoom != null && westRoom.roomState == RoomState.DISCOVERED)
        {
            neighbors.Add(westRoom);
        }

        return neighbors;
    }


    private void checkDoor(RoomTile nextRoom)
    {
        nextRoom.GetComponent<Button>().interactable = true;

        if (DiceRoll.rollTest(PlayerData.Instance.GetTrait(PlayerData.Trait.PERSEPTION)) > 10 || nextRoom.hasBeenPeaked)
        {
            
            if (nextRoom.roomType == MySceneManager.SceneType.COMBAT) nextRoom.combatIcon.gameObject.SetActive(true); 
            else if (nextRoom.roomType == MySceneManager.SceneType.EVENT) nextRoom.eventIcon.gameObject.SetActive(true);
            //else {Debug.Log("could not check door"); nextRoom.GetComponent<Image>().color = Color.red;}
        }

        nextRoom.hasBeenPeaked = true;

    }

    public void CopyRoomTile(RoomTile toCopy)
    {
        toCopy.x = this.x;
        toCopy.y = this.y;

        toCopy.roomState = this.roomState;
        toCopy.roomType = this.roomType;

        toCopy.northDoor.color = this.northDoor.color;
        toCopy.southDoor.color = this.southDoor.color;
        toCopy.eastDoor.color = this.eastDoor.color;
        toCopy.westDoor.color = this.westDoor.color;

        toCopy.hasBeenPeaked = this.hasBeenPeaked; 

        


        
    }

    public void calculateFCost()
    {
        fCost = gCost + hCost;
    }

}
