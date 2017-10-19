using System;
using System.Collections.Generic;

namespace XLY.XDD.Control.ReadToTxt
{
    public interface IOfficeFile
    {
        Dictionary<String, String> DocumentSummaryInformation { get; }

        Dictionary<String, String> SummaryInformation { get; }
    }
}