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
        Dictionary<string, HashEntry[]> hashes = new Dictionary<string, HashEntry[]>();
        List<string> redis_tables = new List<string> ();
        List<DataTable> redisTables = new List<DataTable> ();
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
            selectTableCombo.Items.Clear();


            if(selectDBCombo.SelectedIndex == 0)
            {
                redisTables.Clear();

                hashes = redis_utilities.GetAllRedisData(MainForm.redisConnection);

                foreach(string hashKey in hashes.Keys)
                {
                    string table_name = hashKey.Split(':')[0];
                    if(!redis_tables.Contains(table_name)) redis_tables.Add(table_name);
                }


                selectTableCombo.Items.AddRange(redis_tables.ToArray());

                foreach(string table_name in  redis_tables)
                {
                    DataTable dt = new DataTable();
                    int columns_added = 0;
                    foreach (string hashKey in hashes.Keys)
                    {
                        if (hashKey.Contains(table_name))
                        {
                            //CREATE COLUMS IF NOT CREATED
                            if (columns_added == 0)
                            {
                                foreach (var column in hashes[hashKey])
                                {
                                    if (column.Name.ToString().Equals("image"))
                                    {
                                        dt.Columns.Add("image", typeof(byte[]));
                                        continue;
                                    }
                                    dt.Columns.Add(column.Name.ToString());
                                }
                                columns_added = 1;
                            }

                            DataRow row = dt.NewRow();

                            foreach (var column in hashes[hashKey])
                            {
                                if (!(column.Name.Equals("encoding")||column.Name.Equals("image"))) row[column.Name] = column.Value.ToString();
                                if(column.Name.Equals("image"))
                                {
                                    row[column.Name] = (byte[])column.Value;
                                }
                            }
                            dt.Rows.Add(row);
                        }
                    }

                    redisTables.Add(dt);
                }

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
                /*DataTable dt = new DataTable();
                int columns_added = 0;
                foreach(string hashKey in hashes.Keys)
                {
                    if(hashKey.Contains(table_name))
                    {
                        //CREATE COLUMS IF NOT CREATED
                        if(columns_added == 0) 
                        {
                            foreach(var column in hashes[hashKey])
                            {
                                dt.Columns.Add(column.Name.ToString());
                            }
                            columns_added = 1;
                        }

                        DataRow row = dt.NewRow();

                        foreach (var column in hashes[hashKey])
                        {
                            if(!column.Name.Equals("encoding")) row[column.Name.ToString()] = column.Value.ToString();
                        }
                        dt.Rows.Add(row);
                    }
                }*/

                tableDataGridView.DataSource = redisTables[selectTableCombo.SelectedIndex];
                //((DataGridViewImageColumn)tableDataGridView.Columns["image"]).ImageLayout = DataGridViewImageCellLayout.Zoom;
            }
        }
    }
}
