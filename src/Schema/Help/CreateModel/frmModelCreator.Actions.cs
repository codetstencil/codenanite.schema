using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class frmModelCreator
    {
        private bool TestForSpace()
        {
            var fHasSpace = txtTableCode.Text.Contains(" ");
            lblMessage.Text = fHasSpace ? "Spaces not allowed in table names" : string.Empty;
            return true;
        }

        void AutoCreateTable()
        {
            var tablelist = CreateModel.GetListOfLinks(txtTableCode.Text);

            var table = CreateModel.GetTablesInCreator(CreateModel.GetListOfLinks(txtTableCode.Text));


            //string[] lines = txtTableCode.Text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            //for (int i = 0; i < lines.Length; i++)
            //{
            //    var tablelist = CreateModel.GetListOfTables(txtTableCode.Text);
            //}
        }

    }
}
