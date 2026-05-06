using ACL.business;
using ACL.dao;
using McpOrchestrator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Channels;
using System.Windows.Forms;

namespace ACL.uc
{
    public partial class SessionDataList : UserControl
    {
        private Channel<string> result;
        private DataStore store;

        public SessionDataList()
        {
            InitializeComponent();
            try
            {
                result = Channel.CreateBounded<string>(100);
                store = new DataStore();
            }
            catch { }
            this.Load += SessionDataList_Load;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RichTextBox Content { get; set; }

        private void SessionDataList_Load(object? sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var item = await result.Reader.ReadAsync();
                    Content.Invoke(new Action(() =>
                    {
                        WriteResponse(item);
                    }));
                }
            });

            LoadSessions();
            Context.Instance.AgentChanged += OnAgentChanged;
        }

        private void LoadSessions()
        {
            tvSessions.Nodes.Clear();
            var sessions = store.QuerySessions();
            TreeNode? last = null;
            tvSessions.AfterSelect -= tvSessions_AfterSelect;
            foreach (var session in sessions)
            {
                var node = new TreeNode()
                {
                    Text = session.Description,
                    ToolTipText = session.Description,
                    Tag = session
                };

                tvSessions.Nodes.Add(node);
                last = node;
            }

            tvSessions.AfterSelect += tvSessions_AfterSelect;
            if (last != null)
            {
                tvSessions.SelectedNode = last;
            }
        }

        private void LoadSessionItems(Session session)
        {
            var items = store.QuerySessionItems(session.Id);
            foreach (var item in items)
            {
                switch (item.SessionType)
                {
                    case SessionType.User:
                        WriteSend(item.Description);
                        break;
                    default:
                        WriteResponse(item.Description);
                        break;
                }
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string SelectedSession
        {
            get
            {
                var node = this.tvSessions.SelectedNode as TreeNode;
                if (node == null) return string.Empty;

                var tag = node.Tag as Session;
                if (tag == null) return string.Empty;

                return tag.Id.ToString();
            }
            set
            {
                if (string.IsNullOrEmpty(value)) return;

                foreach (TreeNode node in tvSessions.Nodes)
                {
                    var tag = node.Tag as Session;
                    if (tag == null) continue;

                    if (tag.Id.ToString() == value)
                    {
                        tvSessions.SelectedNode = node;
                        return;
                    }
                }

            }
        }

        private void txtAsk_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter && (ModifierKeys & Keys.Control) == Keys.Control)
            {
                e.Handled = true;
                btnSend.PerformClick();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            WriteSend(txtAsk.Text);
            _ = Context.Instance.Agent.Chat(txtAsk.Text);
            txtAsk.Text = string.Empty;
        }

        private void OnCancelAsk(object sender, EventArgs e)
        {
            Context.Instance.Agent.Cancel();
        }

        private void WriteSend(string text)
        {
            this.Invoke(() =>
            {
                Content.ForeColor = Color.Black;
                int len = Content.TextLength;
                Content.AppendText(text + Environment.NewLine);
                Content.Select(len, text.Length);
                Content.SelectionColor = Color.Black;
                Content.SelectionLength = 0;
            });
        }


        private void OnCreateNewSession(object sender, EventArgs e)
        {
            var frmSession = new NewSession();
            if (frmSession.ShowDialog() == DialogResult.OK)
            {
                var session = new Session()
                {
                    Id = DateTime.Now.Ticks,
                    Description = frmSession.Session,
                    State = ABL.Object.EnumEntityState.Added
                };


                if (store.Save(session))
                {
                    pnlAsk.Visible = true;
                    pnlAsk.BringToFront();

                    txtAsk.Text = string.Empty;
                    LoadSessions();
                }
            }
        }

        private void OnDeleteNewSession(object sender, EventArgs e)
        {
            var node = tvSessions.SelectedNode;
            if (node == null) return;

            var session = node.Tag as Session;
            if (session == null) return;

            session.State = ABL.Object.EnumEntityState.Deleted;
            store.Save(session);

            LoadSessions();
        }


        private void tvSessions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var node = e.Node;
            if (node == null) return;

            var session = node.Tag as Session;
            if (session == null) return;

            Context.Instance.CurrentSession = session;
            pnlAsk.Visible = true;
            pnlAsk.BringToFront();

            LoadSessionItems(session);
        }

        private void WriteResponse(string text)
        {
            this.Invoke(() =>
            {
                int len = Content.TextLength;
                Content.AppendText(text);
                Content.Select(len, text.Length);
                Content.SelectionColor = Color.Blue;
                Content.SelectionLength = 0;
                Content.ForeColor = Color.Black;
                if (text.Contains('\r') || text.Contains('\n'))
                {
                    Content.ScrollToCaret();
                }
            });
        }

        private void OnConnectClick(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                await Context.Instance.Agent.Serve(result);
            });
        }


        private void OnAgentChanged(business.mcp.AgentChangedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Context.Instance.Agent?.Serve(result);
            });
        }

    }
}
