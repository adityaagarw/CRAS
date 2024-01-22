using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace CRAS
{
    public partial class ConfigManager : Form
    {
        private Dictionary<string, string> appSettings;
        string xmlFilePath = MainForm.workingDirectoryFrontend + "\\CRAS\\Cras.config";
        MainForm mainForm;

        public ConfigManager(MainForm mainFormObj)
        {
            mainForm = mainFormObj;
            InitializeComponent();
        }

        private void LoadXmlData()
        {
            appSettings = new Dictionary<string, string>();

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                XmlNodeList appSettingsNodes = doc.SelectNodes("/configuration/Settings/add");

                foreach (XmlNode node in appSettingsNodes)
                {
                    string key = node.Attributes["key"].Value;
                    string value = node.Attributes["value"].Value;
                    appSettings[key] = value;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading XML file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PopulateDataGridView()
        {
            configDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            configDataGrid.AutoSize = true;
            this.AutoSize = true;

            configDataGrid.Columns.Add("Key", "Key");
            configDataGrid.Columns.Add("Value", "Value");
            

            configDataGrid.Rows.Clear();
            //configDataGrid.AllowUserToAddRows = false;

            foreach (var kvp in appSettings)
            {
                configDataGrid.Rows.Add(kvp.Key, kvp.Value);
            }
        }

        private void SaveChangesToXml()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xmlFilePath);

                XmlNode appSettingsNode = doc.SelectSingleNode("/configuration/Settings");
                appSettingsNode.RemoveAll(); // Clear existing nodes

                foreach (DataGridViewRow row in configDataGrid.Rows)
                {
                    string key = row.Cells[0].Value?.ToString() ?? string.Empty;

                    if (!key.Equals(string.Empty))
                    {
                        string value = row.Cells[1].Value?.ToString() ?? string.Empty;

                        XmlElement element = doc.CreateElement("add");
                        element.SetAttribute("key", key);
                        element.SetAttribute("value", value);
                        appSettingsNode.AppendChild(element);
                    }
                }

                doc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving changes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void configDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            SaveChangesToXml();
        }

        private void ConfigManager_Load(object sender, EventArgs e)
        {
            LoadXmlData();
            PopulateDataGridView();
            
        }

        private void saveRestart_Click(object sender, EventArgs e)
        {
            SaveChangesToXml();
            mainForm.InitiateRestart();

        }
    }
}
