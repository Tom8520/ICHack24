using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecondScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKeyDown("space"))
         {
            SceneManager.UnloadSceneAsync("SecondScene");
            SceneManager.LoadScene("AimLab", LoadSceneMode.Additive);
         }
    }
}
