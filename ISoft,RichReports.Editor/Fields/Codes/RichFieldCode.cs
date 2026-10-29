using ISoft.RichReports.Editor.Fields.Arguments;
using ISoft.RichReports.Editor.Fields.Core;
using ISoft.RichReports.Editor.Fields.Regions;
using ISoft.RichReports.Editor.Fields.Switches;
using System;
using System.Linq;
using System.Text;

namespace ISoft.RichReports.Editor.Fields.Codes
{
    public class RichFieldCode : ObjectBase, IRichFieldCode
    {
        #region Fields
        private readonly RichEditFieldInfo _fieldInfo;
        private readonly RichFieldSwitchCollection _switches;
        private readonly RichFieldArgumentCollection _arguments;
        #endregion

        #region Proprties
        public RichEditFieldInfo FieldInfo => _fieldInfo;

        public RichEditFieldType FieldType => FieldInfo.FieldType;

        public string FieldName => FieldInfo.FieldName;

        public string FieldProperty => Arguments.GetFirstValue();

        public RichEditFieldKey FieldKey => RichEditFieldKey.CreateFromInfo(FieldInfo, FieldProperty);

        public string OriginalCode { get; set; }

        public IRichFieldArgumentCollection Arguments => _arguments;

        public IRichFieldSwitchCollection Switches => _switches;

        public bool IsTableStart => FieldType == RichEditFieldType.MergeField && OriginalCode != null && OriginalCode.Contains(TableRegionsName.TableStart);

        public bool IsTableEnd => FieldType == RichEditFieldType.MergeField && OriginalCode != null && OriginalCode.Contains(TableRegionsName.TableEnd);

        public bool IsRegion => IsTableStart || IsTableEnd;

        // for mail merge
        public bool IsDataTableField => FieldType == RichEditFieldType.MergeField && !IsRegion && (FieldProperty?.Contains('.') ?? false);
        #endregion

        #region Constructrors
        public RichFieldCode(RichEditFieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo;
            _switches = new(); // only intiilize it here
            _arguments = new();// only intiilize it here
        }

        public RichFieldCode(string fieldName) 
            : this(RichEditFieldInfoProvider.GetFieldInfoOrEmpty(fieldName))
        {
          
        }

        public RichFieldCode(RichEditFieldType fieldType)
           : this(RichEditFieldInfoProvider.GetFieldInfoByTypeOrEmpty(fieldType))
        {

        }

        public static RichFieldCode CreateEmpty()
        {
            return new RichFieldCode(RichEditFieldInfo.Unknown);
        }

        #endregion

        #region Actions

        /// <summary>
        /// Builds the complete field code string with arguments
        /// </summary>
        public string Build()
        {
            Validate();

            var sb = new StringBuilder(FieldName);

            // Append arguments (quoted values)
            Arguments.BuildTo(sb);

            // Append switches (quoted values)
            Switches.BuildTo(sb);

            return sb.ToString();
        }

        public bool TryBuild(out string result)
        {
            if (!IsValid())
            {
                result = string.Empty;
                return false;
            }

            result = Build();
            return true;
        }

        public bool IsValid()
        {
            return FieldInfo.IsValid();
        }

        // for mail merge
        public string GetDataTableName()
        {
            var property = FieldProperty;

            if (FieldType != RichEditFieldType.MergeField || string.IsNullOrEmpty(property) || IsRegion)
            {
                return string.Empty;
            }

            // Check if property starts with collection prefix
            if (!property.Contains('.'))
            {
                return string.Empty;
            }

            var parts = property.Split('.');
            return string.Join(".", parts.Take(parts.Length - 1));
        }

        private static string GetDataTableName(string dataPath)
        {
            if (string.IsNullOrWhiteSpace(dataPath)) return null;
            int lastDot = dataPath.LastIndexOf('.');
            if (lastDot <= 0) return null;
            return dataPath.Substring(0, lastDot);
        }

        public void Validate()
        {
            if (!IsValid())
            {
                throw new InvalidOperationException($"Field not found for type {FieldType}");
            }
        }

        protected void CopyArgumentsTo(IRichFieldCode fieldCode)
        {
            var arguments = Arguments.ToList();
            for (int i = 0; i < arguments.Count; i++)
            {
                IRichFieldArgument argument = arguments[i].Copy();
                fieldCode.Arguments.Add(argument);
            }
        }

        protected void CopySwitchesTo(IRichFieldCode fieldCode)
        {
            var switches = Switches.ToList();
            for (int i = 0; i < switches.Count; i++)
            {
                IRichFieldSwitch s = switches[i].Copy();
                fieldCode.Switches.Add(s);
            }
        }

        /// <summary>
        /// Clones the field code
        /// </summary>
        public override object Clone()
        {
            // dont use base.Clone()
            RichFieldCode clone = new(FieldInfo)
            {
                OriginalCode = OriginalCode,
            };

            CopyArgumentsTo(clone);
            CopySwitchesTo(clone);
            //clone.Switches.AddClonedRange(Switches);
            return clone;
        }

        IRichFieldCode IRichFieldCode.Copy() => this.CreateClone();
        #endregion
    }
}
