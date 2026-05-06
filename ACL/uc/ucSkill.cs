using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class ucSkill : UserControl
    {
        public event KeyEventHandler NameChanged;
        public ucSkill()
        {
            InitializeComponent();
            txtName.KeyDown += TxtName_KeyDown;
        }

        private void TxtName_KeyDown(object? sender, KeyEventArgs e)
        {
            if(NameChanged != null)
            {
                NameChanged(txtName, e);   
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SkillName
        {
            get
            {
                return this.txtName.Text;
            }
            set
            {
                this.txtName.Text = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SkillDescription
        {
            get
            {
                return txtDesc.Text;
            }
            set
            {
                txtDesc.Text = value;
            }
        }


    }
}
