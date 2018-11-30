using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bharat.DevExtreme.Helpers.DataSource
{
    public interface ISQLColumnProvider
    {
        string GetDBColumnName(string columnName);
        string GetDBValueConversion(string columnName, string condition, string value);
    }
}
