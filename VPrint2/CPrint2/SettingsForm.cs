using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using CPrint2.Controls;

namespace CPrint2
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
            highS.Tag = lblhighS;
            minWidth.Tag = lblminWidth;
            minHeight.Tag = lblminHeight;
        }

        protected override void OnLoad(EventArgs e)
        {
            int _highS = 0;
            int _minWidth = 0;
            int _minHeight = 0;
            VCamLib.ReadSettings(ref _highS, ref _minWidth, ref _minHeight);

            highS.Value = _highS;
            minWidth.Value = _minWidth;
            minHeight.Value = _minHeight;

            base.OnLoad(e);
        }

        private void Any_ValueChanged(object sender, EventArgs e)
        {
            VCamLib.SaveSettings(highS.Value, minWidth.Value, minHeight.Value);
            TrackBar tb = (TrackBar)sender;
            string text = string.Format("{0} .value = {1}", tb.Name, tb.Value);
            Label lbl = (Label)tb.Tag;
            lbl.Text = text;
            toolTip1.SetToolTip(tb, text);
        }

        private void Radio_ValueChanged(object sender, EventArgs e)
        {
            CameraControl.ShowMode mode = CameraControl.ShowMode.Normal;
            if (sender == radioButton1)
                mode = CameraControl.ShowMode.ShowOrigin;
            else if (sender == radioButton2)
                mode = CameraControl.ShowMode.Thresholded;
            else if (sender == radioButton3)
                mode = CameraControl.ShowMode.HSV;

            CameraControl.Mode = mode;
        }
    }
}
