using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace ManagedUi.GridSystem
{
[ExecuteInEditMode]
public class GrowGridLayout : LayoutGroup
{
    public enum GrowDirection
    {
        Rows,
        Column
    }

    [Header("GridStyle")]
    public Vector2 spacing;

    public GrowDirection direction;


    private int _rows;
    private int _columns;
    public Vector2 _cellSize;


    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        var growthFactor = ComputeGrowthFactor();
        ComputeGridDimensions(growthFactor);

        Vector2 parentSize = new Vector2(rectTransform.rect.width, rectTransform.rect.height);

        float spacingX = (spacing.x/_columns)*(_columns - 1);
        float paddingX = padding.left/(float)_columns + padding.right/(float)_columns;
        float spacingY = (spacing.y/_rows)*(_rows - 1);
        float paddingY = padding.top/(float)_rows + padding.bottom/(float)_rows;

        _cellSize.x = (parentSize.x/_columns - spacingX) - paddingX;
        _cellSize.y = (parentSize.y/_rows - spacingY) - paddingY;

        int counter = 0;
        foreach (var childs in rectChildren)
        {
            int growthFactorChild = 1;
            var layoutGrowable = childs.GetComponent<IGridElement>();
            if (layoutGrowable != null)
            {
                if (layoutGrowable.IgnoreLayout())
                {
                    continue;
                }
                growthFactorChild = direction == GrowDirection.Rows ? layoutGrowable.VerticalLayoutGrowth() : layoutGrowable.HorizontalLayoutGrowth();
            }

            float growWidth = direction == GrowDirection.Rows ? 1 : growthFactorChild;
            float growHeight = direction == GrowDirection.Rows ? growthFactorChild : 1;


            float cellWidth = _cellSize.x*growWidth + (growWidth - 1) * spacingX;
            float cellHeight = _cellSize.y*growHeight + (growHeight - 1) * spacingY;

            int rowCount = counter/_columns;
            int columnCount = counter%_columns;

            float posX = columnCount*(spacing.x + _cellSize.x) + padding.left;
            float posY = rowCount*(spacing.y + _cellSize.y) + padding.top;

            SetChildAlongAxis(childs, 0, posX, cellWidth);
            SetChildAlongAxis(childs, 1, posY, cellHeight);
            counter += growthFactorChild;
        }
    }


    private void ComputeGridDimensions(int requiredElements)
    {
        switch (direction)
        {
            case GrowDirection.Column:
                _rows = 1;
                _columns = Mathf.CeilToInt(requiredElements/(float)_rows);
                break;
            case GrowDirection.Rows:
                _columns = 1;
                _rows = Mathf.CeilToInt(requiredElements/(float)_columns);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private int ComputeGrowthFactor()
    {
        int growFactor = 0;
        foreach (var childs in rectChildren)
        {
            var layoutGrowable = childs.GetComponent<IGridElement>();
            if (layoutGrowable == null)
            {
                // Factor 1 as default
                growFactor++;
                continue;
            }
            if (layoutGrowable.IgnoreLayout())
            {
                continue;
            }
            if (direction == GrowDirection.Column)
            {
                growFactor += layoutGrowable.HorizontalLayoutGrowth();
            }
            else
            {
                growFactor += layoutGrowable.VerticalLayoutGrowth();

            }
        }
        return growFactor;
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