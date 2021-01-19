using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Team31337_Server
{
    public partial class ServerForm2 : Form
    {
        public int myNum;
        ServerForm1 dlgMain = new ServerForm1();
        delegate void AppendTextDelegate(string s);
        delegate void setMyNumDelegate(Control ctrl, int num);

        AppendTextDelegate _textAppender;
        setMyNumDelegate _myNumSetter;

        public ServerForm2()
        {
            InitializeComponent();
            _textAppender = new AppendTextDelegate(AppendText);
            _myNumSetter = new setMyNumDelegate(setMyNum);
        }

        public ServerForm2(ServerForm1 _form)
        {
            InitializeComponent();
            _textAppender = new AppendTextDelegate(AppendText);
            dlgMain = _form;
        }

        public void setMyNum(Control ctrl, int a)
        {
            if(ctrl.InvokeRequired)
            {
                ctrl.Invoke(_myNumSetter, ctrl, a);
            }
            else
            {
                myNum = a;
            }
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
            dlgMain.SendToClient(myNum);
            AppendText("관리자: " + string.Format(tts));
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
