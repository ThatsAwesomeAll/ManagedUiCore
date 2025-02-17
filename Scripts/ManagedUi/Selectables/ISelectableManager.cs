namespace ManagedUi.Selectables
{
    public interface ISelectableManager
    {
        public void TriggerExternalSelect(SelectableParent selectable);
        public void TriggerExternalDeSelect(SelectableParent selectable);
        public void TriggerExternalConfirm(SelectableParent selectable);
    }
}