using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSetter : MonoBehaviour
{
    
    public GameObject level1;

    [Space]
    public int lv2_treshhold;
    public GameObject level2;
    [Space]
    public int lv3_treshhold;
    public GameObject level3;
    



    public void openLevel()
    {
        level1.SetActive(false);
        level2.SetActive(false);
        level3.SetActive(false);

        int kc = PlayerData.Instance.kill_counter;
        if ( kc < lv2_treshhold ){level1.SetActive(true);}
        else if ( kc <= lv2_treshhold ){level2.SetActive(true);}
        else if ( kc <= lv3_treshhold ){level3.SetActive(true);}
        
    }


}
