using System;
using System.Windows.Forms;

namespace Timer {
    public partial class DoneBox : Form {
        public DoneBox() {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
