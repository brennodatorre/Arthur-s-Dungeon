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

    

    public int x;
    public int y;





    public void discoverRoom()
    {
        if (roomState== RoomState.DISCOVERED) return;

        roomState = RoomState.DISCOVERED;

        GetComponent<Image>().color = MapSceneManager.Instance.discoveredColor;

        analyzeRoom();

        MapSceneManager.Instance.roomsDiscovered++;


        MySceneManager.Instance.openNextScene(roomType);

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

    public void analyzeRoom()
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

}
