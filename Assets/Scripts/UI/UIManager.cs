using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    [SerializeField]Camera mainCamera;
    PostProcessVolume postProcessVolume;
    [SerializeField]GameObject window;
    [SerializeField]GameObject skillTree;
    [SerializeField]GameObject rightPanelContent;
    [SerializeField]GameObject leftPanelContent;

    [SerializeField]GameObject skillTree1;
    [SerializeField]GameObject skillTree2;
    [SerializeField]GameObject skillTree3;
    [SerializeField]Sprite skillTreeBarOn;
    [SerializeField]Sprite skillTreeBarOff;
    [SerializeField]GameObject skillTreeBar1;
    [SerializeField]GameObject skillTreeBar2;
    [SerializeField]GameObject skillTreeBar3;
    // Start is called before the first frame update
    void Start()
    {
        postProcessVolume=Camera.main.gameObject.GetComponent<PostProcessVolume>();
        postProcessVolume.enabled=false;
        window.SetActive(false);
        skillTree.SetActive(false);
        rightPanelContent.SetActive(false);
        rightPanelContent.GetComponent<ContentSizeFitter>().enabled=false;

        skillTree1.SetActive(true);
        skillTree2.SetActive(false);
        skillTree3.SetActive(false);
        skillTreeBar1.GetComponent<Image>().sprite=skillTreeBarOn;
        skillTreeBar2.GetComponent<Image>().sprite=skillTreeBarOff;
        skillTreeBar3.GetComponent<Image>().sprite=skillTreeBarOff;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.M)){
            if(skillTree.activeSelf==true){
                skillTree.SetActive(false);
                postProcessVolume.enabled=false;
            }
            else if(skillTree.activeSelf==false){
                skillTree.SetActive(true);
                postProcessVolume.enabled=true;
            }
        }
    }

    public void OpenSettingMenu(){
        window.SetActive(true);
        postProcessVolume.enabled=true;
    }

    public void CloseSettingMenu(){
        window.SetActive(false);
        postProcessVolume.enabled=false;
    }

    public void CloseSkillTree(){
        skillTree.SetActive(false);
        postProcessVolume.enabled=false;
    }

    public void SkillTreeBarClicked(){
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        EventSystem.current.currentSelectedGameObject.GetComponent<Image>().sprite=skillTreeBarOn;
        if(EventSystem.current.currentSelectedGameObject.name==skillTreeBar1.name){
            Debug.Log("skillTreeBar1 Clicked");
            skillTree1.SetActive(true);
            skillTree2.SetActive(false);
            skillTree3.SetActive(false);
            skillTreeBar2.GetComponent<Image>().sprite=skillTreeBarOff;
            skillTreeBar3.GetComponent<Image>().sprite=skillTreeBarOff;
            rightPanelContent.SetActive(false);
            leftPanelContent.transform.localScale=new Vector3(1,1,1);
        }
        else if(EventSystem.current.currentSelectedGameObject.name==skillTreeBar2.name){
            Debug.Log("skillTreeBar2 Clicked");
            skillTree1.SetActive(false);
            skillTree2.SetActive(true);
            skillTree3.SetActive(false);
            skillTreeBar1.GetComponent<Image>().sprite=skillTreeBarOff;
            skillTreeBar3.GetComponent<Image>().sprite=skillTreeBarOff;
            rightPanelContent.SetActive(false);
            leftPanelContent.transform.localScale=new Vector3(1,1,1);
        }
        else if(EventSystem.current.currentSelectedGameObject.name==skillTreeBar3.name){
            Debug.Log("skillTreeBar3 Clicked");
            skillTree1.SetActive(false);
            skillTree2.SetActive(false);
            skillTree3.SetActive(true);
            skillTreeBar1.GetComponent<Image>().sprite=skillTreeBarOff;
            skillTreeBar2.GetComponent<Image>().sprite=skillTreeBarOff;
            rightPanelContent.SetActive(false);
            leftPanelContent.transform.localScale=new Vector3(1,1,1);
        }
    }
}
