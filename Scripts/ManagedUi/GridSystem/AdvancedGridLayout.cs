using System;
using UnityEngine;
using UnityEngine.UI;

namespace ManagedUi.GridSystem
{
[ExecuteInEditMode]
public class AdvancedGridLayout : LayoutGroup
{
    public enum LayoutFocus
    {
        Uniform,
        RowsFocused,
        RowsFixed,
        ColumnsFocused,
        ColumnsFixed
    }

    [Header("GridStyle")]
    public Vector2 spacing;

    public LayoutFocus priority;


    public int rows;
    public int columns;
    public bool fitX;
    public bool fitY;
    public Vector2 cellSize;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        var validChilds = ComputeValidChildren();
        ComputeGridDimensions(validChilds);

        Vector2 parentSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        float spacingX = (spacing.x/columns)*(columns - 1);
        float paddingX = padding.left/(float)columns + padding.right/(float)columns;
        float cellWidth = parentSize.x/columns - spacingX - paddingX;
        float spacingY = (spacing.y/rows)*(rows - 1);
        float paddingY = padding.top/(float)rows + padding.bottom/(float)rows;
        float cellHeight = parentSize.y/rows - spacingY - paddingY;

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int counter = 0;
        foreach (var childs in rectChildren)
        {
            var layoutGrowable = childs.GetComponent<IGridElement>();
            if (layoutGrowable != null && layoutGrowable.IgnoreLayout())
            {
                continue;
            }
            int rowCount = counter/columns;
            int columnCount = counter%columns;

            float cellSizeScaledX = cellSize.x;
            float cellSizeScaledY = cellSize.y;

            float posX = columnCount*(spacing.x + cellSize.x) + padding.left;
            float posY = rowCount*(spacing.y + cellSize.y) + padding.top;

            SetChildAlongAxis(childs, 0, posX, cellSizeScaledX);
            SetChildAlongAxis(childs, 1, posY, cellSizeScaledY);
            counter++;
        }
    }


    private void ComputeGridDimensions(int validChilds)
    {
        switch (priority)
        {
            case LayoutFocus.RowsFocused:
                rows = columns = Mathf.CeilToInt(Mathf.Sqrt(validChilds));
                rows = Mathf.CeilToInt(validChilds/(float)columns);
                fitX = fitY = true;
                break;
            case LayoutFocus.ColumnsFocused:
                rows = columns = Mathf.CeilToInt(Mathf.Sqrt(validChilds));
                columns = Mathf.CeilToInt(validChilds/(float)rows);
                fitX = fitY = true;
                break;
            case LayoutFocus.ColumnsFixed:
                rows = Mathf.CeilToInt(validChilds/(float)columns);
                break;
            case LayoutFocus.RowsFixed:
                columns = Mathf.CeilToInt(validChilds/(float)rows);
                break;
            case LayoutFocus.Uniform:
                rows = columns = Mathf.CeilToInt(Mathf.Sqrt(validChilds));
                fitX = fitY = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private int ComputeValidChildren()
    {

        int validChilds = 0;
        foreach (var childs in rectChildren)
        {
            var LayoutGrowable = childs.GetComponent<IGridElement>();
            if (LayoutGrowable == null)
            {
                validChilds++;
                continue;
            }
            if (LayoutGrowable.IgnoreLayout())
            {
                continue;
            }
            validChilds++;
        }
        return validChilds;
    }

    public override void SetLayoutHorizontal()
    {
    }
    public override void SetLayoutVertical()
    {
    }
    public override void CalculateLayoutInputVertical()
    {
    }
}
}