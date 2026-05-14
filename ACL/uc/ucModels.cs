using ABL.Config.Ant;
using ACL.business.llm;
using ACL.dao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class ucModels : Form
    {
        LLMModelFactory factory = new LLMModelFactory();
        public ucModels()
        {
            InitializeComponent();
            this.Load += OnDataLoad;
        }

        private async void OnDataLoad(object? sender, EventArgs e)
        {
            LoadDatas();
        }

        private async void LoadDatas()
        {
            tvModels.Nodes.Clear();
            var datas = await factory.GetModelsAsync();
            foreach (var data in datas)
            {
                var node = new TreeNode { Text = data.Name, Tag = data };
                tvModels.Nodes.Add(node);
            }
        }

        private void OnAdd(object sender, EventArgs e)
        {
            tvModels.SelectedNode = null;
            btnAccept.Enabled = true;
            btnCancel.Enabled = false;
            Clear();
        }

        private void OnEdit(object sender, EventArgs e)
        {
            btnAccept.Enabled = true;
            btnCancel.Enabled = true;
            Clear();
            var llm = GetCurrentSelect();
            if (llm == null) return;
            ShowInfo(llm);
        }

        private void OnDelete(object sender, EventArgs e)
        {
            var data = GetCurrentSelect();
            if (data == null) return;

            var datas = tvModels.Nodes.Cast<TreeNode>().Where(x => (x.Tag as LLMModelInfo) != data).Select(x => x.Tag as LLMModelInfo).ToList();
            var ret = factory.SaveModelsAsync(datas).Result;
            LoadDatas();
        }

        private void OnView(object sender, TreeViewEventArgs e)
        {
            btnAccept.Enabled = false;
            btnCancel.Enabled = false;
            Clear();
            var llm = GetCurrentSelect();
            if (llm == null) return;
            ShowInfo(llm);
        }

        private void Clear()
        {
            txtName.Tag = null;
            txtName.Text = string.Empty;
            txtDescription.Text = string.Empty;
            txtAccessUrl.Text = string.Empty;
            txtApiKey.Text = string.Empty;
            txtVersion.Text = string.Empty;
            chkDefault.Checked = false;
        }

        private void ShowInfo(LLMModelInfo llm)
        {
            if (llm == null) return;

            txtName.Tag = llm;
            txtName.Text = llm.Name;
            txtDescription.Text = llm.Description;
            txtAccessUrl.Text = llm.AccessUrl;
            txtApiKey.Text = llm.ApiKey;
            txtVersion.Text = llm.Version;
            if (!string.IsNullOrEmpty(llm.IsDefault) && llm.IsDefault.Equals("1"))
            {
                chkDefault.Checked = true;

            }
            else
            {
                chkDefault.Checked = false;
            }
        }

        private (LLMModelInfo, bool) GetEditedData()
        {
            var flag = false;
            LLMModelInfo? data = txtName.Tag as LLMModelInfo;
            if (data == null)
            {
                data = new LLMModelInfo();
                flag = true;
            }

            data.Name = txtName.Text;
            data.Description = txtDescription.Text;
            data.AccessUrl = txtAccessUrl.Text;
            data.ApiKey = txtApiKey.Text;
            data.Version = txtVersion.Text;
            data.IsDefault = chkDefault.Checked ? "1" : "0";
            return (data, flag);
        }

        private LLMModelInfo? GetCurrentSelect()
        {
            if (tvModels.SelectedNode == null) return null;
            var data = tvModels.SelectedNode.Tag as LLMModelInfo;
            return data;
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            var (data, added) = GetEditedData();
            var datas = tvModels.Nodes.Cast<TreeNode>().Select(x => x.Tag as LLMModelInfo).ToList();
            if (added) datas.Add(data);
            var ret = factory.SaveModelsAsync(datas).Result;
            LoadDatas();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnAccept.Enabled = false;
            btnCancel.Enabled = false;
            Clear();

            var llm = GetCurrentSelect();
            if (llm == null) return;

            ShowInfo(llm);
        }
    }
}