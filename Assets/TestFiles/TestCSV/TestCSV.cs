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

    private string displayText = "��ʼ��ʾ������"; // ��ʼ��������

    void OnGUI()
    {
        // ��ֱ���ֿ�ʼ
        GUILayout.BeginVertical();

        // ��ʾ��ǰ���ı�����
        GUILayout.Label(displayText);

        // ����ť�����ʱ���޸� displayText ������
        if (GUILayout.Button("��ȡ�ļ�ȫ������"))
        {
            List<string> list = csvReaderWriter.ReadAll("Assets/TestFiles/TestCSV/test1.csv").SelectMany(x => x).ToList();
            displayText = string.Join(" ", list);
        }

        if (GUILayout.Button("��ȡ��������"))
        {
            List<string> list = csvReaderWriter.ReadColumn("Assets/TestFiles/TestCSV/test1.csv", 0);
            displayText = string.Join(" ",list);
        }


        // ��ֱ���ֽ���
        GUILayout.EndVertical();
    }
}
