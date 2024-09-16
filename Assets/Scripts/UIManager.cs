using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]Camera mainCamera;
    PostProcessVolume postProcessVolume;
    [SerializeField]GameObject window;
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume=Camera.main.gameObject.GetComponent<PostProcessVolume>();
        postProcessVolume.enabled=false;
        window.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSettingMenu(){
        window.SetActive(true);
        postProcessVolume.enabled=true;
    }

    public void CloseSettingMenu(){
        window.SetActive(false);
        postProcessVolume.enabled=false;
    }
}
