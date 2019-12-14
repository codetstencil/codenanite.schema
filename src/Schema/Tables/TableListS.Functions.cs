using System.Linq;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class TableListS
    {
        private void MainFunction()
        {
            OutputList = GetTables()
                .Where(e => string.IsNullOrEmpty(e.ColumnType))
                .Where(t => t.IsChecked)
                .Select(c => Pluralize(c.TableName))
                .ToList();
        }
    }
}