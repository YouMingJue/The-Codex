using System;
using System.Collections.Generic;
using System.IO;

public interface ICsvReaderWriter
{
    // ��ȡ�������ݣ���������CSV�������
    List<List<string>> ReadAll(string filePath);

    // ��ȡ�ض�����
    List<string> ReadRow(string filePath, int rowIndex);

    // ��ȡ�ض�����
    List<string> ReadColumn(string filePath, int columnIndex);

    // ��ȡ�ض���Ԫ��
    string ReadCell(string filePath, int rowIndex, int columnIndex);

    // д���������ݵ�CSV�ļ�
    void WriteAll(string filePath, List<List<string>> data);

    // д���ض����е�CSV�ļ�
    void WriteRow(string filePath, List<string> rowData);

    // д���ض����е�CSV�ļ������׷�ӵ��ļ�ĩβ��
    void WriteColumn(string filePath, List<string> columnData);
}

public class CsvReaderWriter : ICsvReaderWriter
{
    // ��ȡ����CSV�ļ��������ض�ά�б�
    public List<List<string>> ReadAll(string filePath)
    {
        var table = new List<List<string>>();
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var row = new List<string>(line.Split(','));
                    table.Add(row);
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file {filePath} was not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return table;
    }

    // ��ȡ�ض�����
    public List<string> ReadRow(string filePath, int rowIndex)
    {
        List<string> row = null;
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int currentIndex = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (currentIndex == rowIndex)
                    {
                        row = new List<string>(line.Split(','));
                        break;
                    }
                    currentIndex++;
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file {filePath} was not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return row;
    }

    // ��ȡ�ض�����
    public List<string> ReadColumn(string filePath, int columnIndex)
    {
        var column = new List<string>();
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var row = line.Split(',');
                    if (columnIndex < row.Length)
                    {
                        column.Add(row[columnIndex]);
                    }
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file {filePath} was not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return column;
    }

    // ��ȡ�ض��ĵ�Ԫ��
    public string ReadCell(string filePath, int rowIndex, int columnIndex)
    {
        string cell = null;
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int currentIndex = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    if (currentIndex == rowIndex)
                    {
                        var row = line.Split(',');
                        if (columnIndex < row.Length)
                        {
                            cell = row[columnIndex];
                        }
                        break;
                    }
                    currentIndex++;
                }
            }
        }
        catch (FileNotFoundException)
        {
            Console.WriteLine($"Error: The file {filePath} was not found.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return cell;
    }

    // д������CSV����
    public void WriteAll(string filePath, List<List<string>> data)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var row in data)
                {
                    writer.WriteLine(string.Join(",", row));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // д��һ�е�CSV�ļ�ĩβ
    public void WriteRow(string filePath, List<string> rowData)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, append: true))
            {
                writer.WriteLine(string.Join(",", rowData));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // д��һ�е�CSV�ļ���ע�⣺�Ḳ���ļ���
    public void WriteColumn(string filePath, List<string> columnData)
    {
        try
        {
            var data = ReadAll(filePath);
            for (int i = 0; i < data.Count && i < columnData.Count; i++)
            {
                data[i].Add(columnData[i]);
            }

            WriteAll(filePath, data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
