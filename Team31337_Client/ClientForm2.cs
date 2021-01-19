using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Team31337_Client
{
    public partial class ClientForm2 : Form
    {
        
        ClientForm1 dlgMain = new ClientForm1();
        delegate void AppendTextDelegate(string s);
        
        AppendTextDelegate _textAppender;
        

        public ClientForm2()
        {
            InitializeComponent();
            _textAppender = new AppendTextDelegate(AppendText);
        }

        public ClientForm2(ClientForm1 _form)
        {
            InitializeComponent();
            _textAppender = new AppendTextDelegate(AppendText);
            dlgMain = _form;
        }

        private void ClientForm2_Load(object sender, EventArgs e)
        {
            dlgMain.setHandle = this.Handle;
        }

        public void AppendText(string s)
        {
            if (this.txtHistory.InvokeRequired) this.txtHistory.Invoke(_textAppender, s);
            else
            {
                string source = this.txtHistory.Text;
                this.txtHistory.Text = source + Environment.NewLine + s;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            // 보낼 텍스트
            string tts = txtTTS.Text.Trim();
            if (string.IsNullOrEmpty(tts))
            {
                 txtTTS.Focus();
                return;
            }

            dlgMain.ChatHistory = tts;

            // 전송 완료 후 텍스트박스에 추가하고, 원래의 내용은 지운다.
            dlgMain.SendToServer();
            AppendText("사용자: " + string.Format(tts));
            txtTTS.Clear();
            txtTTS.Focus();
        }

        private void txtTTS_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, e);
            }

            else if (e.KeyCode == Keys.Escape)
            {
                this.Dispose();
            }
        }
    }
}
