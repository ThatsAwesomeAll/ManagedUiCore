namespace ManagedUi.GridSystem
{
public interface IGridElement
{
    public int VerticalLayoutGrowth();
    public int HorizontalLayoutGrowth();
    public bool IgnoreLayout();
}
}