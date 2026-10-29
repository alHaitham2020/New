namespace ISoft.RichReports.Editor.Fields.Instructions
{
    //public abstract class RichFieldInstructionsBuilderBase<TOptions> : IRichFieldInstructionsBuilder where TOptions : class, IRichFieldOptions
    //{
    //    private readonly TOptions _options;

    //    protected TOptions Options => _options;

    //    protected RichFieldInstructionsBuilderBase(TOptions options)
    //    {
    //        _options = options;
    //    }

    //    public abstract void BuildFieldArguments(IRichFieldArgumentCollection arguments);

    //    public abstract void BuildFieldSwitches(IRichFieldSwitchCollection switches, IRichFieldSwitchBuilder switchBuilder);

    //    public void BuildInstructions(IRichFieldCode fieldCode, IRichFieldSwitchBuilder switchBuilder)
    //    {
    //        BuildFieldArguments(fieldCode.Arguments);
    //        BuildFieldSwitches(fieldCode.Switches, switchBuilder);
    //    }
    //}

    //public class RichMailMergeFieldInstructionsBuilder : RichFieldInstructionsBuilderBase<RichMergeFieldOptions>
    //{
    //    public RichMailMergeFieldInstructionsBuilder(RichMergeFieldOptions options) : base(options)
    //    {

    //    }

    //    public override void BuildFieldArguments(IRichFieldArgumentCollection arguments)
    //    {
    //        if (!string.IsNullOrEmpty(Options.DataFieldPath))
    //        {
    //            arguments.AddValueAtIndex(0, Options.DataFieldPath);
    //        }
    //    }

    //    public override void BuildFieldSwitches(IRichFieldSwitchCollection switches, IRichFieldSwitchBuilder switchBuilder)
    //    {
    //        if (!string.IsNullOrEmpty(Options.StringBefore))
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFieldSpecificSwitch(RichFieldSwitchKeys.MailMerge.Before, Options.StringBefore));
    //        }

    //        if (!string.IsNullOrEmpty(Options.StringAfter))
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFieldSpecificSwitch(RichFieldSwitchKeys.MailMerge.After, Options.StringAfter));
    //        }

    //        if (Options.BarcodeType != FieldBarcodeType.None)
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFieldSpecificSwitch(RichFieldSwitchKeys.MailMerge.Barcode, $"{Options.BarcodeType}"));
    //        }

    //        if (Options.ImageWidth.HasValue)
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFieldSpecificSwitch(RichFieldSwitchKeys.ImageBasedField.ImageWidth, Options.ImageWidth.Value.ToString()));
    //        }

    //        if (Options.ImageHeight.HasValue)
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFieldSpecificSwitch(RichFieldSwitchKeys.ImageBasedField.ImageHeight, Options.ImageHeight.Value.ToString()));
    //        }

    //        if (!RichFieldFormat.IsNullOrEmpty(Options.Format))
    //        {
    //            switches.AddOrUpdate(switchBuilder.CreateFormatSwitch(Options.Format.Value));
    //        }
    //    }
    //}
}
