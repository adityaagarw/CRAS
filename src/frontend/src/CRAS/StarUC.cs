using CRAS.Properties;
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
    public partial class StarUC : UserControl
    {
        public event EventHandler<int> StarSelected;
        public enum starState
        {
            UNSELECTED,
            HOVERED,
            SELECTED
        }
        public starState state = starState.UNSELECTED;

        public StarUC()
        {
            InitializeComponent();
            starPictureBox.BackgroundImage = Resources.StarUnselected;
            
        }

        private void starPictureBox_Click(object sender, EventArgs e)
        {
            SelectStar();
            int index = Parent.Controls.IndexOf(this);

            StarSelected?.Invoke(this, index);

        }

        private void starPictureBox_MouseEnter(object sender, EventArgs e)
        {
            starPictureBox.BackgroundImage = Resources.StarHover;

            if (state == starState.UNSELECTED) state = starState.HOVERED;

        }

        private void starPictureBox_MouseLeave(object sender, EventArgs e)
        {
            if (state == starState.SELECTED)
            {
                SelectStar();
            }
            if (state == starState.HOVERED)
            {
                UnselectStar();
            }
        }

        public void SelectStar()
        {
            state = starState.SELECTED;
            starPictureBox.BackgroundImage = Resources.StarSelected;
        }

        public void UnselectStar()
        {
            state = starState.UNSELECTED;
            starPictureBox.BackgroundImage = Resources.StarUnselected;
        }

        public void UnselectAllStars(FlowLayoutPanel panel)
        {
            foreach (StarUC star in panel.Controls)
            {
                star.UnselectStar();
            }
        }
        public void SelectStars(FlowLayoutPanel panel, int starRating)
        {
            UnselectAllStars(panel);

            int i = -1;
            foreach (StarUC star in panel.Controls)
            {
                i++;
                if (i >= 0 && i <= starRating)
                {
                    star.SelectStar();
                }
            }
        }

    }
}
