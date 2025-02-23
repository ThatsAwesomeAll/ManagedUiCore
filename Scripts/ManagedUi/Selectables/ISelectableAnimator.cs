namespace ManagedUi.Selectables
{
public interface ISelectableAnimator
{

    public enum Mode
    {
        Default,
        Selected,
        Confirmed
    }

    public void SetEnabled(bool enabled);

    public void LerpTo(Mode mode, float currentValue);
}
}