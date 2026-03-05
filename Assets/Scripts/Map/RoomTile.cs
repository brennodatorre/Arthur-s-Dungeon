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


        MySceneManager.Instance.openNextScene(roomType);

    }

    public void buildRoom()
    {
        roomState = RoomState.UNDISCOVERED;
        GetComponent<Image>().color = MapSceneManager.Instance.undiscoveredColor;
        GetComponent<Button>().interactable = false;

        roomType = MySceneManager.Instance.getNextScene() ;
        


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
        if (northRoom != null) {
            
            northDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(northRoom);
        }
        if (southRoom != null)
        {
            
            southDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(southRoom);
        }
        if (eastRoom != null)
        {
            eastDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(eastRoom);
        }
        if (westRoom != null)
        {
            westDoor.color = MapSceneManager.Instance.discoveredColor;
            checkDoor(westRoom);
        }

        
    }

    private void checkDoor(RoomTile nextRoom)
    {
        nextRoom.GetComponent<Button>().interactable = true;

        if (DiceRoll.rollTest(PlayerData.Instance.GetTrait(PlayerData.Trait.PERSEPTION)) > 10)
        {
            if (nextRoom.roomType == MySceneManager.SceneType.COMBAT) nextRoom.combatIcon.gameObject.SetActive(true); 
            else if (nextRoom.roomType == MySceneManager.SceneType.EVENT) nextRoom.eventIcon.gameObject.SetActive(true);
            else {Debug.Log("could not check door"); nextRoom.GetComponent<Image>().color = Color.red;}
        }

    }

}
