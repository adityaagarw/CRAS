using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRAS
{
    public partial class DeleteReasonForm : Form
    {
        public int totalFaces = 0;
        public int sameFaces = 0;
        public int unidentifiedFaces = 0;
        public int misidentifiedFaces = 0;
        public int accuracy = 0;
        public DeleteReasonForm()
        {
            InitializeComponent();

            DataTable dataTable = pgsql_utilities.GetTableData(MainForm.pgsql_connection, "Session", "WHERE sessionid = '" + MainForm.session + "'", " LIMIT 1");

            int.TryParse(dataTable.Rows[0]["total_faces"].ToString(), out totalFaces);
            int.TryParse(dataTable.Rows[0]["same_faces"].ToString(), out sameFaces);
            int.TryParse(dataTable.Rows[0]["unidentified_faces"].ToString(), out unidentifiedFaces);
            int.TryParse(dataTable.Rows[0]["misidentified_faces"].ToString(), out misidentifiedFaces);
            int.TryParse(dataTable.Rows[0]["accuracy"].ToString(), out accuracy);

        }

        private void UpdateAccuracy()
        {
            if (totalFaces > 0)
            {
                accuracy = ((totalFaces - sameFaces - unidentifiedFaces - misidentifiedFaces) * 100 /totalFaces);
                Console.WriteLine("Accuracy: " + accuracy);
            }

            pgsql_utilities.UpdateSessionAccuracy(MainForm.pgsql_connection, MainForm.session, sameFaces, unidentifiedFaces, misidentifiedFaces, accuracy);
            
            this.Close();
        }

        private void sameFace_Click(object sender, EventArgs e)
        {
            sameFaces++;
            UpdateAccuracy();
        }

        private void misidentifiedFace_Click(object sender, EventArgs e)
        {
            misidentifiedFaces++;
            UpdateAccuracy();
        }

        private void unidentifiedFace_Click(object sender, EventArgs e)
        {
            unidentifiedFaces++;
            UpdateAccuracy();
        }
    }
}
