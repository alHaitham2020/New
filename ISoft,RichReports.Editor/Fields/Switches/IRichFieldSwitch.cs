namespace ISoft.RichReports.Editor.Fields.Switches
{
    public interface IRichFieldSwitch
    {
        RichSwitchType SwitchType { get;}

        string SwitchKey { get;}

        string SwitchValue { get; }

        bool IsFormatSwitch { get; }

        bool IsDateTimeFormatSwitch { get; }

        bool IsNumericFormatSwitch { get; }

        bool IsValid();

        string Build();

        bool TryBuild(out string result);

        T GetValue<T>(T defaultValue = default);

        IRichFieldSwitch Copy();
    }
}
