// ***********************************************************************
// Assembly:XLY.SF.Project.Domains.Contract.Display
// Author:Songbing
// Created:2017-06-08 14:33:22
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Domains
{
    public static class DisplayAttributeHelper
    {
        private static Dictionary<string, List<DisplayAttribute>> DisPlayAttCache;

        static DisplayAttributeHelper()
        {
            DisPlayAttCache = new Dictionary<string, List<DisplayAttribute>>();
        }

        public static List<DisplayAttribute> FindDisplayAttributes(Type dataType)
        {
            var dataTypeName = dataType.FullName;

            lock (DisPlayAttCache)
            {
                if (!DisPlayAttCache.Keys.Contains(dataTypeName))
                {
                    List<DisplayAttribute> list = new List<DisplayAttribute>();
                    var pis = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    foreach (var pi in pis)
                    {
                        var das = pi.GetCustomAttributes(typeof(DisplayAttribute), true);
                        if (das.Any())
                        {
                            DisplayAttribute da = das.First() as DisplayAttribute;
                            da.Owner = pi;
                            if (string.IsNullOrEmpty(da.Key))
                            {
                                da.Key = pi.Name;
                            }
                            if (string.IsNullOrEmpty(da.Text))
                            {
                                da.Text = pi.Name;
                            }
                            list.Add(da);
                        }
                    }
                    DisPlayAttCache.Add(dataTypeName, list);
                }
            }

            return DisPlayAttCache[dataTypeName];
        }
    }
}
