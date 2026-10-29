//using System.Collections.Generic;
//using System.Linq;

//namespace ISoft.RichReportDesigner.Core.Fields.Data
//{
//    public static class RichDataFieldExtensions
//    {
//        /// <summary>
//        /// Gets the root parent group (topmost ancestor)
//        /// </summary>
//        public static IRichDataFieldGroup GetRootGroup(this IRichDataFieldBase field)
//        {
//            if (field is IRichDataFieldGroup group)
//            {
//                return group.GetParents().LastOrDefault() ?? group;
//            }
//            else if (field is IRichDataField dataField)
//            {
//                return dataField.ParentGroup?.GetParents().LastOrDefault() ?? dataField.ParentGroup;
//            }
//            return null;
//        }

//        /// <summary>
//        /// Gets all ancestor groups for a field
//        /// </summary>
//        public static IEnumerable<IRichDataFieldGroup> GetAncestors(this IRichDataFieldBase field)
//        {
//            if (field is IRichDataFieldGroup group)
//            {
//                foreach (var ancestor in group.GetParents())
//                {
//                    yield return ancestor;
//                }
//            }
//            else if (field is IRichDataField dataField)
//            {
//                var current = dataField.ParentGroup;
//                while (current != null)
//                {
//                    yield return current;
//                    current = current.ParentGroup;
//                }
//            }
//        }

//        /// <summary>
//        /// Checks if a field belongs to a specific group hierarchy
//        /// </summary>
//        public static bool BelongsToGroup(this IRichDataFieldBase field, IRichDataFieldGroup targetGroup)
//        {
//            if (field is IRichDataFieldGroup group)
//            {
//                return group == targetGroup || group.IsDescendantOf(targetGroup);
//            }
//            else if (field is IRichDataField dataField)
//            {
//                return dataField.ParentGroup == targetGroup ||
//                       dataField.ParentGroup?.IsDescendantOf(targetGroup) == true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Gets all fields in the same group hierarchy
//        /// </summary>
//        public static IEnumerable<IRichDataField> GetSiblingFields(this IRichDataField field)
//        {
//            if (field.ParentGroup == null)
//                return Enumerable.Empty<IRichDataField>();

//            return field.ParentGroup.Fields.Where(f => f != field);
//        }

//        /// <summary>
//        /// Gets all fields in the entire group tree
//        /// </summary>
//        public static IEnumerable<IRichDataField> GetAllDescendantFields(this IRichDataFieldGroup group)
//        {
//            var result = new List<IRichDataField>();
//            result.AddRange(group.Fields);

//            foreach (var childGroup in group.Fields.OfType<IRichDataFieldGroup>())
//            {
//                result.AddRange(childGroup.GetAllDescendantFields());
//            }

//            return result;
//        }
//    }
//}
