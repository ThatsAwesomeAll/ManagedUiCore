namespace ManagedUi.GridSystem
{
public interface IManagedGridLayoutElement
{
    public int VerticalLayoutGrowth();
    public int HorizontalLayoutGrowth();
    public bool IgnoreLayout();
}
}