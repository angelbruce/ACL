using ACL.business.project;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class NewFile : Form
    {
        public NewFile()
        {
            InitializeComponent();
        }

        public enum Type
        {
            Directory,
            File
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Type FileType { get; set; }

        private string GetFileName()
        {
            var name = textBox1.Text;
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("必须输入文件名称");
                return string.Empty;
            }

            return name.Trim();
        }

        private void OnOkClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            var name = GetFileName();
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            var dir = ProjectConfig.Current.Directory;
            var file = Path.Combine(dir, name);

            if (FileType == Type.Directory)
            {
                Directory.CreateDirectory(file);
                return;
            }

            var newDir = new DirectoryInfo(file);
            if (!newDir.Exists)
            {
                newDir.Create();
            }

            File.Create(file,0,FileOptions.RandomAccess);


            this.Close();
        }

        private void OnCancelClick(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
