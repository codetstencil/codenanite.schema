using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class TableListLowerCase
    {
        private void MainFunction()
        {
            OutputList = GetTables()
                .Where(e => string.IsNullOrEmpty(e.ColumnType))
                .Where(t=>t.IsChecked)
                .Select(c => (c.TableName).ToLower())
                .ToList();
        }
    }
}