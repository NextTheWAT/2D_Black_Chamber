using UnityEngine;

[CreateAssetMenu(fileName = "New Sheet", menuName = "Data/Sheet")]
public class Sheet : ScriptableObject
{
    public string[,] cells;

    public int RowCount => cells?.GetLength(0) ?? 0;
    public int ColumnCount => cells?.GetLength(1) ?? 0;

    public string this[int row, int col]
    {
        get
        {
            TryGetCell(row, col, out string value);
            return value;
        }
    }

    public void SetData(string[,] data)
        => cells = data;

    public bool TryGetCell(int row, int col, out string value)
    {
        value = null;

        if (cells == null) return false;
        if (row < 0 || row >= RowCount || col < 0 || col >= ColumnCount) return false;

        value = cells[row, col];
        return true;
    }

    public string[] GetRow(int row)
    {
        if (row < 0 || row >= RowCount) return null;

        string[] result = new string[ColumnCount];
        for (int c = 0; c < ColumnCount; c++)
            result[c] = cells[row, c];

        return result;
    }

    public string[] GetColumn(int col)
    {
        if (col < 0 || col >= ColumnCount) return null;

        string[] result = new string[RowCount];
        for (int r = 0; r < RowCount; r++)
            result[r] = cells[r, col];

        return result;
    }
}
