using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public MySceneManager mySceneManager;

    public int currentPage = 0;
    public GameObject offButton;
    public float delay;
    public List<GameObject> pages = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void doNextPage(){
        currentPage++;

        if(currentPage == 1) { StartCoroutine(doPage1());}
        if(currentPage == 2) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 3) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 4) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 5) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 6) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 7) {StartCoroutine(doPage7());}
        if(currentPage == 8) {StartCoroutine(doNextSimplePage(0));}
        if(currentPage == 9) {StartCoroutine(doNextSimplePage(0));}

    }

    private IEnumerator doPage1(){

        offButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        offButton.GetComponent<Button>().interactable = false;
        yield return new WaitForSeconds(delay) ;

        offButton.SetActive(false);

        pages[currentPage].SetActive(true);

    }

        private IEnumerator doPage7(){

        pages[currentPage - 1].SetActive(false);
        
        yield return new WaitForSeconds(delay);

        pages[currentPage].SetActive(true);

        yield return new WaitForSeconds(delay);

        pages[currentPage].transform.Find("Dialogue").gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        pages[currentPage].transform.Find("Arthur").gameObject.SetActive(true);

        yield return new WaitForSeconds(delay);

        mySceneManager.openNextScene(MySceneManager.SceneType.NEXT, 1f);




    }


    private IEnumerator doNextSimplePage(float pause){

        pages[currentPage - 1].SetActive(false);

        yield return new WaitForSeconds(pause) ;


        pages[currentPage].SetActive(true);

    }
}
