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
    public partial class ClientForm4 : Form
    {
        Socket mainSock;
        IPAddress thisAddress;
        bool isOK = false;

        public ClientForm4()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

        }


        private void ClientForm4_Load(object sender, EventArgs e)
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

            if (mainSock.Connected)
            {
                MessageBox.Show("이미 연결되어 있습니다!");
                return;
            }

            int port = 8082;

            try
            {
                mainSock.Connect(thisAddress, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("연결에 실패했습니다!", ex.Message, MessageBoxButtons.OK);
                this.Dispose();
            }

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = mainSock;

            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
        }

        void DataReceived(IAsyncResult ar)
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 데이터 수신을 끝낸다.
            int received = obj.WorkingSocket.EndReceive(ar);

            if (received > 0)
            {
                string text = Encoding.UTF8.GetString(obj.Buffer);

                if(text.Contains("SignUpFail"))
                {
                    MessageBox.Show("이미 존재하는 아이디입니다.");
                    isOK = false;
                }

                else if(text.Contains("SignUpSuccess"))
                {
                    MessageBox.Show("사용 가능한 아이디입니다.");
                    isOK = true;
                }

                else
                {
                    MessageBox.Show("서버가 아이디를 인식하지 못했습니다.");
                }

                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);

            }
            // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
            else if (received <= 0)
            {
                obj.WorkingSocket.Close();
                return;
            }
        }


        private void btnIDCheck_Click(object sender, EventArgs e)
        {
            if (tbID.Text == "")
            {
                MessageBox.Show("아이디를 기입해 주세요.");
            }
            else
            {
                byte[] bDts = Encoding.UTF8.GetBytes("MemberIDCheck" + '\x01' + tbID.Text);
                mainSock.Send(bDts);
            }
        }

        private void tbPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!(char.IsDigit(e.KeyChar) || e.KeyChar == Convert.ToChar(Keys.Back)))
            {
                e.Handled = true;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            if(isOK == true && tbID.Text != "" && tbPW.Text != ""
                && tbName.Text != "" && tbPhone.Text != "")
            {
                byte[] bDts = Encoding.UTF8.GetBytes("SetToMember" + '\x02' + tbID.Text + '\x01' +
                    tbPW.Text + '\x01' + tbName.Text + '\x01' + tbPhone.Text);
                mainSock.Send(bDts);
                MessageBox.Show("회원가입을 완료했습니다.");
                this.Dispose();
            }
            else
            {
                MessageBox.Show("인적사항을 빠짐없이 기입해 주세요.\r\n아이디 중복 체크를 했는지 확인하세요.");
            }
        }

        private void tbID_TextChanged(object sender, EventArgs e)
        {
            isOK = false;
        }
    }
}
