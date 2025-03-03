using ManagedUi.Selectables;
using ManagedUi.SystemInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ManagedUi.GridSystem
{
public class GridSelection : MonoBehaviour, ISelectableManager
{
    public UiInputManager inputManager;

    Vector2Int _currentSelectedIndex = Vector2Int.zero;
    SelectableParent _currentSelected;

    enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    Vector2Int GetVector2IntFromDirection(Direction dir)
    {
        Vector2Int direction = Vector2Int.zero;

        switch (dir)
        {
            case Direction.Up:
                direction = new Vector2Int(0, 1);
                break;
            case Direction.Down:
                direction = new Vector2Int(0, -1);
                break;
            case Direction.Left:
                direction = new Vector2Int(-1, 0);
                break;
            case Direction.Right:
                direction = new Vector2Int(1, 0);
                break;
            default:
                break;
        }

        return direction;
    }

    private SelectableParent[] _selectables = Array.Empty<SelectableParent>();
    private Dictionary<SelectableParent, Vector2Int> _grid = new Dictionary<SelectableParent, Vector2Int>();

    public void OnEnable()
    {
        SetupGrid(true);
        if (inputManager != null)
        {
            inputManager.OnDown += () =>
            {
                MoveSelection(Direction.Down);
            };

            inputManager.OnUp += () =>
            {
                MoveSelection(Direction.Up);
            };

            inputManager.OnLeft += () =>
            {
                MoveSelection(Direction.Left);
            };

            inputManager.OnRight += () =>
            {
                MoveSelection(Direction.Right);
            };

            inputManager.OnConfirm += () =>
            {
                Confirmed();
            };
        }
    }
    private void OnDisable()
    {
        if (inputManager != null)
        {
            inputManager.OnDown -= () =>
            {
                MoveSelection(Direction.Down);
            };

            inputManager.OnUp -= () =>
            {
                MoveSelection(Direction.Up);
            };

            inputManager.OnLeft -= () =>
            {
                MoveSelection(Direction.Left);
            };

            inputManager.OnRight -= () =>
            {
                MoveSelection(Direction.Right);
            };

            inputManager.OnConfirm -= () =>
            {
                Confirmed();
            };
        }
        StopCoroutine(SetUp(true));
    }

    public void SetupGrid(bool setDefault)
    {
        _selectables = GetComponentsInChildren<SelectableParent>();
        if (_selectables.Length == 0)
        {
            return;
        }
        StartCoroutine(SetUp(setDefault));
    }

    IEnumerator SetUp(bool setDefault)
    {
        yield return new WaitForEndOfFrame();
        DeselectGrid();
        SetUpGrid();
        if (_currentSelected != null)
        {
            _currentSelected.SetSelected(true);
            _currentSelectedIndex = _currentSelected.GridPosition;
        }
        if (!setDefault) yield break;
        SelectDefault();
    }

    public void SelectDefault()
    {
        // Set most center element to be default
        Vector2Int currentMostCenter = new Vector2Int(0, 0);
        var currentTarget = _grid.FirstOrDefault(x => x.Value == currentMostCenter);
        if (currentTarget.Key != null)
        {
            currentTarget.Key.SetSelected(true);
            return;
        }

        float maxDistance = float.MaxValue;
        KeyValuePair<SelectableParent, Vector2Int> target = _grid.FirstOrDefault(x => x.Value.magnitude > 0);
        foreach (var selectable in _grid)
        {
            currentMostCenter = selectable.Value;
            if (currentMostCenter.magnitude < maxDistance)
            {
                maxDistance = currentMostCenter.magnitude;
                target = selectable;
            }
        }
        target.Key.SetSelected(true);
    }

    void DeselectGrid()
    {
        foreach (SelectableParent selectable in _selectables)
        {
            selectable.SetSelected(false);
        }
    }

    void SetUpGrid()
    {
        Vector2 minSize = GetMinimalSize(_selectables);
        _grid.Clear();
        // reduce grid size for unique positions
        minSize *= 0.5f;
        foreach (var selectable in _selectables)
        {
            selectable.GridPosition = new Vector2Int((int)(selectable.ScreenPosition.x/minSize.x), (int)(selectable.ScreenPosition.y/minSize.y));
            _grid.Add(selectable, selectable.GridPosition);
        }
    }

    Vector2 GetMinimalSize(SelectableParent[] selectables)
    {
        Vector2 minSize = Vector2.one*float.MaxValue;
        // compute the minimal size of all selectables
        foreach (SelectableParent selectable in selectables)
        {
            var currentSize = selectable.Size;
            minSize = minSize.magnitude > currentSize.magnitude ? currentSize : minSize;
        }
        return minSize;
    }


    public SelectableParent GetMatchingElementDirection(Vector2Int current, Vector2Int direction)
    {
        SelectableParent nextBest = null;
        int minDistance = int.MaxValue;

        foreach (var element in _grid)
        {
            Vector2Int diff = element.Value - current;
            float angle = Vector2.Angle(direction, diff);
            if (angle <= 45)
            {
                int distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
                if (distance > 0 && distance < minDistance)
                {
                    minDistance = distance;
                    nextBest = element.Key;
                }
            }
        }
        return nextBest;
    }

    private void Confirmed()
    {
        if (_currentSelected == null)
        {
            return;
        }
        _currentSelected.SetConfirmed();
    }

    private void MoveSelection(Direction dir)
    {
        var selectionDirection = GetVector2IntFromDirection(dir);
        SelectableParent nextBest = GetMatchingElementDirection(_currentSelectedIndex, selectionDirection);
        if (nextBest != null)
        {
            DeselectGrid();
            _currentSelected = nextBest;
            _currentSelected.SetSelected(true);
            _currentSelectedIndex = _grid[nextBest];
        }
    }

    public void TriggerExternalSelect(SelectableParent selectable)
    {
        if (selectable == _currentSelected)
        {
            return;
        }
        DeselectGrid();
        _currentSelected = selectable;
        _currentSelectedIndex = selectable.GridPosition;
        _currentSelected.SetSelected(true);
    }
    public void TriggerExternalDeSelect(SelectableParent selectable)
    {
        DeselectGrid();
        _currentSelected = null;
        _currentSelectedIndex = new Vector2Int(0, 0);
    }

    public void TriggerExternalConfirm(SelectableParent selectable)
    {
        if (selectable == _currentSelected)
        {
            return;
        }
        DeselectGrid();
        _currentSelected = selectable;
        _currentSelected.SetConfirmed();
        _currentSelectedIndex = selectable.GridPosition;
    }
}
}