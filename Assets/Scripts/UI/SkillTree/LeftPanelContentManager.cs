using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LeftPanelContentManager : MonoBehaviour, IScrollHandler
{
    [SerializeField]TMP_Text rightPanelText;
    [SerializeField]GameObject rightPanelContent;

    private Vector3 initialScale;
    private float zoomSpeed=0.1f;
    private float maxZoom=2f;
    // Start is called before the first frame update

    private void Awake(){
        initialScale=transform.localScale;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnScroll(PointerEventData eventData){
        Debug.Log("Scrolling");
        var delta=Vector3.one*(eventData.scrollDelta.y*zoomSpeed);
        var desiredScale=transform.localScale+delta;
        desiredScale=ClampDesiredScale(desiredScale);
        transform.localScale=desiredScale;
    }

    private Vector3 ClampDesiredScale(Vector3 desiredScale){
        desiredScale=Vector3.Max(initialScale,desiredScale);
        desiredScale=Vector3.Min(initialScale*maxZoom,desiredScale);
        return desiredScale;
    }

    public void SkillBallClicked(){
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        rightPanelContent.SetActive(true);
        rightPanelText.text=EventSystem.current.currentSelectedGameObject.GetComponent<SkillBallManager>().skillBallInfo.skillBallDescription;
        rightPanelContent.GetComponent<ContentSizeFitter>().enabled=true;
    }
}
