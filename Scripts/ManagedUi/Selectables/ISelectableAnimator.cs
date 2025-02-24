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

    public void SetEnabled(ISelectableAnimator.Mode mode, bool enableAnimation = true);

    public void LerpTo(Mode mode, float currentValue);
}
}