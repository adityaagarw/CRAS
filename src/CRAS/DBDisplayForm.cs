using Npgsql;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CRAS
{
    public partial class DBDisplayForm : Form
    {
        NpgsqlConnection connection;
        public DBDisplayForm()
        {
            InitializeComponent();
        }

        private void DBDisplayForm_Load(object sender, EventArgs e)
        {
            selectDBCombo.Items.Clear();
            selectDBCombo.Items.Add("Redis");
            selectDBCombo.Items.Add("PGSQL");
            connection = pgsql_utilities.ConnectToPGSQL();
        }

        private void selectDBCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(selectDBCombo.SelectedIndex == 0)
            {

            }
            
            else if(selectDBCombo.SelectedIndex == 1)
            {
                //GET LIST OF PGSQL TABLES

                List<string> tables = new List<string>();
                tables = pgsql_utilities.GetListofTables(connection);

                if (tables.Count > 0) { selectTableCombo.Items.AddRange(tables.ToArray()); }
            }
        }

        private void selectTableCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            tableDataGridView.Columns.Clear();

            string table_name = selectTableCombo.Text;

            string db_name = selectDBCombo.Text;

            if (db_name.Equals("PGSQL"))
            {
                tableDataGridView.DataSource = pgsql_utilities.GetTableData(connection, table_name);
            }

            else if(db_name.Equals("Redis"))
            {

            }
        }
    }
}
