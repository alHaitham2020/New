namespace ISoft.RichReports.Editor.Fields.Switches
{
    public interface IRichFieldSwitchCollection : IRichFieldSwitchReadOnlyCollection, IUniqueItemsCache<string, IRichFieldSwitch>
    {
        void AddOrUpdateSwitch(string switchCode, string switchValue);

        bool TryUpdateSwitch(string switchCode, string switchValue);

        void RemoveTypeSwitches(RichSwitchType switchType);
    }
}
