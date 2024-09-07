using System;
using System.Collections.Generic;
using System.IO;

public interface ICsvReaderWriter
{
    // 读取所有数据，返回整个CSV表格内容
    List<List<string>> ReadAll(string filePath);

    // 读取特定的行
    List<string> ReadRow(string filePath, int rowIndex);

    // 读取特定的列
    List<string> ReadColumn(string filePath, int columnIndex);

    // 读取特定单元格
    string ReadCell(string filePath, int rowIndex, int columnIndex);

    // 写入所有数据到CSV文件
    void WriteAll(string filePath, List<List<string>> data);

    // 写入特定的行到CSV文件
    void WriteRow(string filePath, List<string> rowData);

    // 写入特定的列到CSV文件（这会追加到文件末尾）
    void WriteColumn(string filePath, List<string> columnData);
}

public class CsvReaderWriter : ICsvReaderWriter
{
    // 读取整个CSV文件，并返回二维列表
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

    // 读取特定的行
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

    // 读取特定的列
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

    // 读取特定的单元格
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

    // 写入整个CSV数据
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

    // 写入一行到CSV文件末尾
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

    // 写入一列到CSV文件（注意：会覆盖文件）
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
