// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class LoadingManager : MonoBehaviour
// {

//     public static LoadingManager Instance;

//     public bool isReady = false;    
//     public List<GameObject> objects = new List<GameObject>();


 
//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//             //DontDestroyOnLoad(gameObject); // Persist across scenes
//         }
//         else
//         {
//             Destroy(gameObject); // Avoid duplicates
//         }

        
//     }

   

//     // Update is called once per frame
//     void Update()
//     {
//         bool nullFound = false;
//         foreach (var obj in objects)
//         {
//             if (obj == null) { nullFound = true;  break;}

            
//         }

//         if (!nullFound) isReady = true;
//     }
// }
