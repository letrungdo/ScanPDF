using System;
using System.IO;

namespace ScanPDF
{
    public interface ISave
    {
        string Save(MemoryStream fileStream);
    }
}
