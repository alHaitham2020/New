using System;
using System.Collections;

namespace ISoft.RichReports.Core.Models.Data
{
    public class DataObjectNameCollection : CollectionBase
    {
        public DataObjectName this[int index] => List[index] as DataObjectName;

        public void CopyFrom(DataObjectNameCollection source)
        {
            Clear();
            AddRange(source);
        }

        public int Add(string name, string displayName)
        {
            return Add(new DataObjectName(name, displayName));
        }

        public void AddRange(DataObjectNameCollection items)
        {
            foreach (DataObjectName item in items)
            {
                Add(item);
            }
        }

        public int Add(DataObjectName item)
        {
            if (List.Contains(item))
            {
                return List.IndexOf(item);
            }

            return List.Add(item);
        }

        public int IndexOf(string displayName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].DisplayName == displayName)
                {
                    return i;
                }
            }

            return -1;
        }

        public int IndexOfByName(string name)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name == name)
                {
                    return i;
                }
            }

            return -1;
        }

        public DataObjectName GetItemByName(string name)
        {
            IEnumerator enumerator = GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DataObjectName DataObjectName = (DataObjectName)enumerator.Current;
                    if (DataObjectName.Name == name)
                    {
                        return DataObjectName;
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            return null;
        }

        public void Sort(IComparer comparer)
        {
            InnerList.Sort(comparer);
        }
    }

}
