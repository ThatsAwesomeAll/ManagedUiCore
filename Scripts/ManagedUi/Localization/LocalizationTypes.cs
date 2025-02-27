
namespace ManagedUi.Localization
{

public static class LocalizationType
{
    public enum Table
    {
        UIMenu,
        Dialog
    }

    public static string GetTableFileName(Table table)
    {
        switch (table)
        {
            case Table.UIMenu:
                return "MenuStrings";
            case Table.Dialog:
                return "Dialog";
        }
        return "";
    }
}
}