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
    public partial class ClientForm1 : Form
    {
        int myNum;
        delegate void AppendTextDelegate(Control ctrl, string s);
        delegate void GetFormDelegate(Control ctrl, string formName);
        AppendTextDelegate _textAppender;
        GetFormDelegate _formGetter;
        Socket mainSock;
        public bool formMadeByRcv;
        IPAddress thisAddress;
        public IntPtr formHandle = IntPtr.Zero;
        SoundClass AnySound = new SoundClass();
        String TempStr;

        public ClientForm1()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            _formGetter = new GetFormDelegate(GetChatForm);
            this.txtPort.Text = "8080";
        }

        private void ClientForm1_Load(object sender, EventArgs e)
        {
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());
            // 처음으로 발견되는 ipv4 주소를 사용한다.
            foreach (IPAddress addr in he.AddressList)
            {
                if (addr.AddressFamily == AddressFamily.InterNetwork)
                {
                    thisAddress = addr;
                    break;
                }
            }
            txtAddress.Text = thisAddress.ToString();
        }

        public String ChatHistory
        {
            get { return TempStr;}
            set { this.TempStr = value; }
        }

        public IntPtr setHandle
        {
            set { this.formHandle = value; }
        }

        void GetChatForm(Control ctrl, string formName)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(_formGetter, ctrl, formName);
            }
            else
            {
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.Name == formName)
                    {
                        frm.Activate();
                        return;
                    }
                }
                ClientForm2 dlgchat = new ClientForm2(this);
                dlgchat.Show();
                if(formMadeByRcv == true)
                {
                    AnySound.spMsgRcv.Play();
                }
                dlgchat.txtTTS.Focus();
            }
        }

        void AppendText(Control ctrl, string s)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_textAppender, ctrl, s);
            else
            {
                string source = ctrl.Text;
                ctrl.Text = source + Environment.NewLine + s;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            Socket tempSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

            if(txtID.Text == "" || txtPW.Text == "")
            {
                MessageBox.Show("아이디와 비밀번호를 기입해 주십시오.");
                return;
            }

            int port;
            if (!int.TryParse(txtPort.Text, out port))
            {
                MessageBox.Show("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.");
                txtPort.Focus();
                txtPort.SelectAll();
                return;
            }

            try
            {
                tempSock.Connect(txtAddress.Text, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("연결에 실패했습니다!", ex.Message, MessageBoxButtons.OK);
                return;
            }

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = tempSock;

            byte[] check = new byte[5000];
            check = Encoding.UTF8.GetBytes(txtID.Text + '\x01' + txtPW.Text);
            tempSock.Send(check);

            byte[] isgood = new byte[100];
            tempSock.Receive(isgood);

            if (Encoding.UTF8.GetString(isgood).Contains("GoodToLogin"))
            {
                mainSock = tempSock;
                byte[] bDts = new byte[5000];
                mainSock.Receive(bDts);

                myNum = Convert.ToInt32(Encoding.UTF8.GetString(bDts));

                mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);

                btnOrder.Enabled = true;
                btnQuit.Enabled = true;
                btnOpenChat.Enabled = true;
                btnConnect.Enabled = false;
                btnSignUp.Enabled = false;
                txtAddress.Enabled = false;
                txtID.Enabled = false;
                txtPort.Enabled = false;
                txtPW.Enabled = false;
            }

            else if(Encoding.UTF8.GetString(isgood).Contains("LoginDisabled"))
            {
                MessageBox.Show("로그인이 불가능합니다.");
                tempSock.Dispose();
            }
        }

        void DataReceived(IAsyncResult ar)
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 데이터 수신을 끝낸다.
            int received = obj.WorkingSocket.EndReceive(ar);

            // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
            if (received <= 0)
            {
                obj.WorkingSocket.Close();
                return;
            }
           

            formMadeByRcv = true;
            GetChatForm(this, "ClientForm2");
            

            string text = Encoding.UTF8.GetString(obj.Buffer);

            Control ctrl = Control.FromHandle(this.formHandle);
            ClientForm2 clf2 = (ClientForm2)ctrl;
            clf2.AppendText(string.Format("관리자: {0}", text));
           
           
            obj.ClearBuffer();

            // 수신 대기
            obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
        }


        public void SendToServer()
        {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound)
            {
                MessageBox.Show("서버가 실행되고 있지 않습니다!");
                return;
            }

            // 보낼 텍스트
            string tts = TempStr;

            // 문자열을 utf8 형식의 바이트로 변환한다.
            byte[] bDts = Encoding.UTF8.GetBytes(tts);

            // 서버에 전송한다.
            mainSock.Send(bDts);
        }

        private void btnOpenChat_Click(object sender, EventArgs e)
        {
            // 서버가 대기중인지 확인한다.
            if (!mainSock.IsBound)
            {
                MessageBox.Show("서버가 실행되고 있지 않습니다!");
                return;
            }
            formMadeByRcv = false;
            GetChatForm(this, "ClientForm2");
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            ClientForm3 OrderDlg = new ClientForm3(myNum);
            OrderDlg.Show();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            ClientForm4 signupDlg = new ClientForm4();
            signupDlg.ShowDialog();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
