using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ZeraSystems.CodeNanite.Schema
{
    public partial class frmModelCreator : Form
    {
        public string TableCode
        {
            get => txtTableCode.Text;
            set => txtTableCode.Text = value;
        }

        public string ColumnCode
        {
            get => txtColumns.Text;
            set => txtColumns.Text = value;
        }

        public string DataTypes
        {
            get => txtDataTypes.Text;
            set => txtDataTypes.Text = value;
        }

        public string SchemaText
        {
            get => txtSchema.Text;
            set => txtSchema.Text = value;
        }

        public frmModelCreator() => InitializeComponent();

        private void BtnOk_Click(object sender, EventArgs e) => Close();

        private void TxtTableCode_TextChanged(object sender, EventArgs e)
        {
            
            //TestForSpace();
            if (TestForSpace())
                Blink(lblMessage);
        }

        private async void Blink(Label label)
        {
            while (true)
            {
                await Task.Delay(500);
                label.ForeColor = label.ForeColor == Color.Black ? Color.CadetBlue : Color.Black;

                //if (label.BackColor == Color.LemonChiffon)
                //    label.BackColor = Color.Aquamarine;
                //else
                //    label.BackColor = Color.LemonChiffon;

                //if (label.BackColor == Color.Red)
                //    label.BackColor = Color.Green;
                //else
                //    label.BackColor = Color.Red;
            }
        }

        private void frmModelCreator_Load(object sender, EventArgs e)
        {
            tabPage1.Visible = false;
        }

        private void txtColumns_Enter(object sender, EventArgs e)
        {
            AutoCreateTable();
        }
    }
}
