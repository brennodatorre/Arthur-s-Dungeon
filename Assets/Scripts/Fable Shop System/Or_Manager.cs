using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Or_Manager : MonoBehaviour
{

    public GameObject shop;
    public GameObject shop_background;

    public int currentPage = 0;
    
    public float delay;
    public List<GameObject> pages = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject pg in pages){ pg.SetActive(false);} // claers the pages at the beggining
        pages[0].SetActive(true); //starts the pages
        pages[1].SetActive(true); //starts first page
    }

    private IEnumerator lateStart()
    { 
        yield return null; // wait for the next frame
    }

    public void doNextPage()
    {
        currentPage++;

        if (currentPage == 1) { StartCoroutine(doPage1()); }
        if (currentPage == 2) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 3) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 4) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 5) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 6) { StartCoroutine(doNextSimplePage(0)); } 
        if (currentPage == 7) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 8) { StartCoroutine(doNextSimplePage(0)); }
        if (currentPage == 0) { leaveOutsideReaderDomain(); }


    }

    public void openShop(){
        pages[currentPage].SetActive(false);

        shop.SetActive(true);
        shop_background.SetActive(true);


    }

    public void closeShop(){
        shop.SetActive(false);
        shop_background.SetActive(false);

        doNextPage();
    }

    private IEnumerator doPage1(){

        
        yield return new WaitForSeconds(delay) ;

        

        pages[currentPage].SetActive(true);

    }

  


    private IEnumerator doNextSimplePage(float pause){

        pages[currentPage - 1].SetActive(false);

        yield return new WaitForSeconds(pause) ;


        pages[currentPage].SetActive(true);

    }


    public void leaveOutsideReaderDomain(){
        PlayerData.Instance.saveJsonData();
        Debug.Log(Application.persistentDataPath);
        StartCoroutine(MySceneManager.Instance.openSceneWithTransition(MySceneManager.SceneType.NEXT));
    }
}
