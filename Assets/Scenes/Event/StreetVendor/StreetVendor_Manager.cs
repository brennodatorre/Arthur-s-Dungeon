using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StreetVendor_Manager : MonoBehaviour
{

    public GameObject shop;
    public ItemDatabase itemDatabase;
    public GameObject walletText;


    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in shop.transform)
        {
            if (!child.CompareTag("ItemOnShop")) {continue;}

            Item item = itemDatabase.getRandom();


            child.GetChild(1).GetComponent<TooltipManager>().description = item.description;
            child.GetChild(1).GetComponentInChildren<Image>().sprite = item.sprite;

            child.GetChild(1).GetComponent<Button>().onClick.AddListener(() => {

                if(PlayerData.Instance.getIlhas() >= item.value) {
                    PlayerData.Instance.addItemToInventory(item);
                    PlayerData.Instance.changeIlhas(- item.value);
                    child.GetChild(1).GetComponent<Button>().interactable = false;
                    child.GetChild(1).GetComponentInChildren<Image>().color = Color.black;
                } else
                {
                    MySceneManager.Instance.StartCoroutine(MySceneManager.Instance.doPopUp("Not enough Ilhas!"));
                }
            });

            child.GetChild(2).GetComponent<TextMeshProUGUI>().text = "I = " +item.value.ToString();

        }
    }

    // Update is called once per frame
    void Update()
    {
        walletText.GetComponent<TextMeshProUGUI>().text = "Ilhas = " + PlayerData.Instance.getIlhas().ToString();
    }
}
