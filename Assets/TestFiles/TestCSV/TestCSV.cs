using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestCVS : MonoBehaviour
{
    ICsvReaderWriter csvReaderWriter = new CsvReaderWriter();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private string displayText = "初始显示的文字"; // 初始文字内容

    void OnGUI()
    {
        // 垂直布局开始
        GUILayout.BeginVertical();

        // 显示当前的文本内容
        GUILayout.Label(displayText);

        // 当按钮被点击时，修改 displayText 的内容
        if (GUILayout.Button("读取文件全部内容"))
        {
            List<string> list = csvReaderWriter.ReadAll("Assets/TestFiles/TestCSV/test1.csv").SelectMany(x => x).ToList();
            displayText = string.Join(" ", list);
        }

        if (GUILayout.Button("读取怪物名字"))
        {
            List<string> list = csvReaderWriter.ReadColumn("Assets/TestFiles/TestCSV/test1.csv", 0);
            displayText = string.Join(" ",list);
        }


        // 垂直布局结束
        GUILayout.EndVertical();
    }
}
