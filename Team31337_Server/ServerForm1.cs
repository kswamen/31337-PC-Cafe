using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using MySql.Data.MySqlClient;


namespace Team31337_Server
{
    public partial class ServerForm1 : Form
    {
        public Socket mainSock;
        public Socket OrderSock;
        public Socket memberSock;
        public int accessNum;
        public bool formMadeByRcv;
        private bool OrderBool = true;
        IPAddress thisAddress;
        SoundClass AnySound = new SoundClass();
        delegate void AppendTextDelegate(Control ctrl, string s);
        delegate void GetFormDelegate(Control ctrl, string formName, int num, string text);
        delegate void SetDlgNameDelegate(Control ctrl, int dlgNum);
        delegate void SetOrderNumDelegate(Control ctrl, int num, Socket sock);
        delegate void SetOrderListViewDelegate(Control ctrl, int i, string[] list);
        delegate void DeductProductStockDelegate(Control ctrl, int count, string productName);
        AppendTextDelegate _textAppender;
        GetFormDelegate _formGetter;
        SetDlgNameDelegate _dlgNameSetter;
        SetOrderNumDelegate _orderNumSetter;
        SetOrderListViewDelegate _orderLvSetter;
        DeductProductStockDelegate _stockDeductor;
        String TempStr;

        public ServerForm1()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            OrderSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            memberSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _textAppender = new AppendTextDelegate(AppendText);
            _formGetter = new GetFormDelegate(GetChatForm);
            _dlgNameSetter = new SetDlgNameDelegate(SetDlgName);
            _orderNumSetter = new SetOrderNumDelegate(SetOrderNum);
            _orderLvSetter = new SetOrderListViewDelegate(SetOrderList);
            _stockDeductor = new DeductProductStockDelegate(DeductProductStock);

            foreach (Control x in splitContainer1.Panel1.Controls) //패널에 위치한 컨트롤들 중 picturebox들을 찾는다
            {
                if (typeof(System.Windows.Forms.PictureBox) == x.GetType())
                {
                    PictureBox a = (PictureBox)x;
                    a.Enabled = false;
                }
            }
            this.txtPort.Text = "8080";
        }

        public String ChatHistory
        {
            get { return TempStr; }
            set { this.TempStr = value; }
        }

        void GetChatForm(Control ctrl, string formName, int num, string text)
        {
            if (ctrl.InvokeRequired)
            {
                ctrl.Invoke(_formGetter, ctrl, formName, num, text);
            }
            else
            {
                foreach (Form frm in Application.OpenForms)
                {
                    if (frm.Text == formName)
                    {
                        frm.Activate();
                        ServerForm2 tempfrm = (ServerForm2)frm;
                        if(formMadeByRcv == true)
                        {
                            tempfrm.AppendText(num.ToString() + string.Format("번 자리: {0}", text));
                        }
                        return;
                    }
                }
                ServerForm2 dlgchat = new ServerForm2(this);
                dlgchat.Show();
                dlgchat.myNum = num;
                dlgchat.txtTTS.Focus();
                if(formMadeByRcv == true)
                {
                    AnySound.spMsgRcv.Play();
                    dlgchat.AppendText(num.ToString() + string.Format("번 자리: {0}", text));
                }
                SetDlgName(dlgchat, num);
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

        void SetDlgName(Control ctrl, int s)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_dlgNameSetter, ctrl, s);
            else
            {
                ctrl.Text = s.ToString() + "번 자리";
            }
        }

        void SetOrderNum(Control ctrl, int num, Socket sock)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_orderNumSetter, ctrl, num, sock);
            else
            {
                foreach (ClientStatus client in connectedOrderClients)
                {
                    if (sock == client.mySocket)
                    {
                        client.myNum = num;
                    }
                }
            }
        }

        void SetOrderList(Control ctrl, int i, string[] list)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_orderLvSetter, ctrl, i, list);
            else
            {
                ListView a = new ListView();
                a = (ListView)ctrl;
                int sum = 0;
                string MenuSum = "";
                string[] temp = new string[3];
                temp[0] = i.ToString();

                for (int j=0; j<list.Length - 1; j++)
                {
                    MenuSum = MenuSum + list[j].Split('\x01')[0] + ' ' + list[j].Split('\x01')[1] + "개, ";
                    DeductProductStock(this, Convert.ToInt32(list[j].Split('\x01')[1]), list[j].Split('\x01')[0]);
                    if(OrderBool == false)
                    {
                        return;
                    }
                    sum = sum + Convert.ToInt32(list[j].Split('\x01')[2]);
                }

                temp[1] = MenuSum;
                temp[2] = sum.ToString();

                ListViewItem item = new ListViewItem(temp);
                item.ToolTipText = MenuSum;

                a.Items.Add(item);
                AnySound.OrderSound();
            }
        }

        void DeductProductStock(Control ctrl, int count, string productName)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_stockDeductor, ctrl, count, productName);
            else
            {
                string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                MySqlConnection conn = new MySqlConnection(connstr);
                MySqlCommand cmd = conn.CreateCommand();
                string sql = "UPDATE product SET Stock = Stock - " + count.ToString() + " WHERE NAME = '" + productName + "'";
                cmd.CommandText = sql;
                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    OrderBool = true;
                }
                catch (Exception ex)
                {
                    OrderBool = false;
                }
                conn.Dispose();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
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

            // 주소가 없다면..
            if (thisAddress == null)
                // 로컬호스트 주소를 사용한다.
                thisAddress = IPAddress.Loopback;

            txtAddress.Text = thisAddress.ToString();
            
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            int portChat;
            int portOrder = 8081;
            int portMember = 8082;

            if (!int.TryParse(txtPort.Text, out portChat))
            {
                MessageBox.Show("포트 번호가 잘못 입력되었거나 입력되지 않았습니다.", "Error");
                txtPort.Focus();
                txtPort.SelectAll();
                return;
            }

            IPEndPoint ChatserverEP = new IPEndPoint(thisAddress, portChat);
            IPEndPoint OrderserverEP = new IPEndPoint(thisAddress, portOrder);
            IPEndPoint MemberServerEP = new IPEndPoint(thisAddress, portMember);

            mainSock.Bind(ChatserverEP);
            mainSock.Listen(10);

            OrderSock.Bind(OrderserverEP);
            OrderSock.Listen(10);

            memberSock.Bind(MemberServerEP);
            memberSock.Listen(10);

            MessageBox.Show("서버를 실행합니다.");
            btnStart.Enabled = false;
            txtPort.ReadOnly = true;
            // 비동기적으로 클라이언트의 연결 요청을 받는다.
            mainSock.BeginAccept(ChatAcceptCallback, null);
            OrderSock.BeginAccept(OrderAcceptCallback, null);
            memberSock.BeginAccept(MemberAcceptCallback, null);
        }

        List<ClientStatus> connectedChatClients = new List<ClientStatus>();
        void ChatAcceptCallback(IAsyncResult ar)
        {
            int tempNum;
            string tempstr = null;
            bool isOK = false;
            // 클라이언트의 연결 요청을 수락한다.
            ClientStatus client = new ClientStatus();
            client.mySocket = mainSock.EndAccept(ar);

            // 또 다른 클라이언트의 연결을 대기한다.
            mainSock.BeginAccept(ChatAcceptCallback, null);

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = client.mySocket;

            byte[] checkString = new byte[5000];
            obj.WorkingSocket.Receive(checkString);
            string[] MemberInfo = new string[2];
            MemberInfo[0] = Encoding.UTF8.GetString(checkString).Split('\x01')[0];
            MemberInfo[1] = Encoding.UTF8.GetString(checkString).Split('\x01')[1];

            MemberInfo[0] = MemberInfo[0].Trim('\0');
            MemberInfo[1] = MemberInfo[1].Trim('\0');

            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();
            string sql = "Select * from member";
            cmd.CommandText = sql;
            try
            {
                conn.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                String[] arr = new String[2];
                arr[0] = reader["ID"].ToString();
                arr[1] = reader["Password"].ToString();
                tempstr = arr[0];
                if (arr[0].Equals(MemberInfo[0]) && arr[1].Equals(MemberInfo[1]))
                {
                    isOK = true;
                    foreach(ClientStatus a in connectedChatClients)
                    {
                        if(arr[0].Equals(a.myID))
                        {
                            isOK = false;
                            break;
                        }
                        else
                        {
                            isOK = true;
                        }
                    }
                    break;
                }
            }

            conn.Dispose();

            if (isOK)
            {
                byte[] b = Encoding.UTF8.GetBytes("GoodToLogin");
                client.myID = tempstr;
                obj.WorkingSocket.Send(b);
            }

            else
            {
                byte[] b = Encoding.UTF8.GetBytes("LoginDisabled");
                obj.WorkingSocket.Send(b);
                return;
            }

            Random r = new Random();
            for (; ; )
            {
                int check = 0;
                tempNum = r.Next(1, 36);
                for (int i = 0; i < connectedChatClients.Count; i++)
                {
                    if (tempNum == connectedChatClients[i].myNum)
                    {
                        check = 1;
                        break;
                    }
                }
                if (check == 1)
                    continue;
                else
                {
                    client.myNum = tempNum;
                    break;
                }
            }
            obj.WorkingNum = client.myNum;

            byte[] bDts = Encoding.UTF8.GetBytes(obj.WorkingNum.ToString());
            obj.WorkingSocket.Send(bDts);

            // 연결된 클라이언트 리스트에 추가해준다.
            connectedChatClients.Add(client);
            string tempString = "pictureBox" + connectedChatClients.Last().myNum.ToString();
            foreach (Control x in splitContainer1.Panel1.Controls) //번호에 해당하는 픽쳐박스를 찾는다.
            {
                if (x.GetType() == typeof(System.Windows.Forms.PictureBox))
                {
                    PictureBox a = (PictureBox)x;
                    if (a.Name.Substring(10) == connectedChatClients.Last().myNum.ToString()) //pictureBox 문자열을 제거하고 뒤의 숫자만 비교
                    {
                        a.BackColor = Color.FromArgb(255, 255, 255);
                    }
                }
            }
            AnySound.PlaySoundClientLogin(Convert.ToString(client.myNum));

            client.mySocket.BeginReceive(obj.Buffer, 0, 4096, 0, ChatDataReceived, obj);
        }


        void ChatDataReceived(IAsyncResult ar)
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                int received = obj.WorkingSocket.EndReceive(ar);

                if (received <= 0)
                {
                    obj.WorkingSocket.Close();

                    return;
                }
                // 텍스트로 변환한다.

                string text = Encoding.UTF8.GetString(obj.Buffer);

                formMadeByRcv = true;
                GetChatForm(this, obj.WorkingNum.ToString() + "번 자리", obj.WorkingNum, text);

             
                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, ChatDataReceived, obj);
            }
            catch
            {
                AsyncObject objtemp = (AsyncObject)ar.AsyncState;
                foreach (Control x in splitContainer1.Panel1.Controls)
                {
                    if (x.GetType() == typeof(System.Windows.Forms.PictureBox))
                    {
                        PictureBox a = (PictureBox)x;
                        if (a.Name.Substring(10) == objtemp.WorkingNum.ToString()) //pictureBox 문자열을 제거하고 뒤의 숫자만 비교
                        {
                            a.BackColor = Color.Black;
                            foreach(ClientStatus cs in connectedChatClients)
                            {
                                if(cs.myNum.Equals(objtemp.WorkingNum))
                                {
                                    connectedChatClients.Remove(cs);
                                    break;
                                }
                            }
                        }
                    }
                }
                AnySound.PlaySoundClientLogout(Convert.ToString(objtemp.WorkingNum));
            }
        }

        List<ClientStatus> connectedOrderClients = new List<ClientStatus>();
        void OrderAcceptCallback(IAsyncResult ar)
        {
            // 클라이언트의 연결 요청을 수락한다.
            ClientStatus client = new ClientStatus();
            client.mySocket = OrderSock.EndAccept(ar);

            // 또 다른 클라이언트의 연결을 대기한다.
            OrderSock.BeginAccept(OrderAcceptCallback, null);

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = client.mySocket;

            byte[] bDts = new byte[100];
            client.mySocket.Receive(bDts);

            client.myNum = Convert.ToInt32(Encoding.UTF8.GetString(bDts));

            // 연결된 클라이언트 리스트에 추가해준다.
            connectedOrderClients.Add(client);

            client.mySocket.BeginReceive(obj.Buffer, 0, 4096, 0, OrderDataReceived, obj);
        }

        List<ClientStatus> connectedMemberClients = new List<ClientStatus>();
        void MemberAcceptCallback(IAsyncResult ar)
        {
            // 클라이언트의 연결 요청을 수락한다.
            ClientStatus client = new ClientStatus();
            client.mySocket = memberSock.EndAccept(ar);

            // 또 다른 클라이언트의 연결을 대기한다.
            memberSock.BeginAccept(MemberAcceptCallback, null);

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = client.mySocket;

            // 연결된 클라이언트 리스트에 추가해준다.
            connectedMemberClients.Add(client);

            client.mySocket.BeginReceive(obj.Buffer, 0, 4096, 0, MemberDataReceived, obj);
        }

        void MemberDataReceived(IAsyncResult ar)
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                int received = obj.WorkingSocket.EndReceive(ar);

                if (received <= 0)
                {
                    obj.WorkingSocket.Close();

                    return;
                }

                string text = Encoding.UTF8.GetString(obj.Buffer);

                if(text.Contains("MemberIDCheck") == true)
                {
                    bool isOK = true;
                    text = text.Replace("MemberIDCheck" + '\x01', "");
                    text = text.Trim('\0');

                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from member";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String arr = reader["ID"].ToString();
                        if(arr.Equals(text))
                        {
                            isOK = false;
                        }
                    }

                    if(isOK)
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("SignUpSuccess");
                        obj.WorkingSocket.Send(bDts);
                    }
                    else
                    {
                        byte[] bDts = Encoding.UTF8.GetBytes("SignUpFail");
                        obj.WorkingSocket.Send(bDts);
                    }
                    conn.Dispose();
                }

                else if(text.Contains("SetToMember") == true)
                {
                    text = text.Replace("SetToMember" + '\x02', "");
                    text = text.Trim('\0');
                    String[] arr = text.Split('\x01');

                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Insert Into member Values('" + arr[0] + "', '" + arr[1] + "', '" +
                        arr[2] + "', '" + arr[3] + "')";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    cmd.ExecuteNonQuery();
                    conn.Dispose();
                }

                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, MemberDataReceived, obj);
            }
            catch (Exception e)
            {
            }
        }

        void OrderDataReceived(IAsyncResult ar)
        {
            try
            {
                AsyncObject obj = (AsyncObject)ar.AsyncState;

                int received = obj.WorkingSocket.EndReceive(ar);

                if (received <= 0)
                {
                    obj.WorkingSocket.Close();

                    return;
                }

                string text = Encoding.UTF8.GetString(obj.Buffer);
                string strToSend = "ProductList";

                if (text.Contains("All") == true)
                {
                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from product";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String[] arr = new String[3];
                        arr[0] = reader["Name"].ToString();
                        arr[1] = reader["Price"].ToString();
                        arr[2] = reader["Stock"].ToString();

                        strToSend = strToSend + arr[0] + '\x01' + arr[1] + '\x01' + arr[2] + '\x02';
                    }
                    byte[] bDts = Encoding.UTF8.GetBytes(strToSend);
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("Ramen") == true)
                {
                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from product Where Sort = 'Ramen'";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String[] arr = new String[3];
                        arr[0] = reader["Name"].ToString();
                        arr[1] = reader["Price"].ToString();
                        arr[2] = reader["Stock"].ToString();

                        strToSend = strToSend + arr[0] + '\x01' + arr[1] + '\x01' + arr[2] + '\x02';
                    }
                    byte[] bDts = Encoding.UTF8.GetBytes(strToSend);
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("Snack") == true)
                {
                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from product Where Sort = 'Snack'";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String[] arr = new String[3];
                        arr[0] = reader["Name"].ToString();
                        arr[1] = reader["Price"].ToString();
                        arr[2] = reader["Stock"].ToString();

                        strToSend = strToSend + arr[0] + '\x01' + arr[1] + '\x01' + arr[2] + '\x02';
                    }
                    byte[] bDts = Encoding.UTF8.GetBytes(strToSend);
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("Beverage") == true)
                {
                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from product Where Sort = 'Beverage'";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String[] arr = new String[3];
                        arr[0] = reader["Name"].ToString();
                        arr[1] = reader["Price"].ToString();
                        arr[2] = reader["Stock"].ToString();

                        strToSend = strToSend + arr[0] + '\x01' + arr[1] + '\x01' + arr[2] + '\x02';
                    }
                    byte[] bDts = Encoding.UTF8.GetBytes(strToSend);
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("ETC") == true)
                {
                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();
                    string sql = "Select * from product Where Sort = 'ETC'";
                    cmd.CommandText = sql;
                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        String[] arr = new String[3];
                        arr[0] = reader["Name"].ToString();
                        arr[1] = reader["Price"].ToString();
                        arr[2] = reader["Stock"].ToString();

                        strToSend = strToSend + arr[0] + '\x01' + arr[1] + '\x01' + arr[2] + '\x02';
                    }
                    byte[] bDts = Encoding.UTF8.GetBytes(strToSend);
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("ProductImage") == true)
                {
                    string receivedString = text.Split('\x01')[1];
                    receivedString = receivedString.Trim('\0'); //소켓 전송시 문자열 뒤에 \0 문자가 꽉 차 있음. 그거를 제거.

                    string arr = "";

                    string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                    MySqlConnection conn = new MySqlConnection(connstr);
                    MySqlCommand cmd = conn.CreateCommand();

                    string sql = "Select * from product Where Name = '" + receivedString + "'";
                    cmd.CommandText = sql;

                    try
                    {
                        conn.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        arr = reader["Image"].ToString();
                    }

                    byte[] bDts = ImageToByteArray(Image.FromFile(@"C:\31337\image\" + arr));
                    obj.WorkingSocket.Send(bDts);

                    conn.Dispose();
                }

                else if (text.Contains("Order") == true)
                {
                    text = text.Replace("Order" + '\x01', "");
                    text = text.Trim('\0');
                    int i = Convert.ToInt32(text.Split('\x01')[0].ToString());
                    text = text.Replace(i.ToString() + '\x01', "");
                    string[] receivedString = text.Split('\x02');

                    string temp = "OrderSuccessed";
                    byte[] bDts = Encoding.UTF8.GetBytes(temp);
                    SetOrderList(lvOrderList, i, receivedString);

                    if (OrderBool == true)
                    {
                        obj.WorkingSocket.Send(bDts);
                    }
                    else if (OrderBool == false)
                    {
                        temp = "OrderFailed";
                        bDts = Encoding.UTF8.GetBytes(temp);
                        obj.WorkingSocket.Send(bDts);
                    }
                }

                // 데이터를 받은 후엔 다시 버퍼를 비워주고 같은 방법으로 수신을 대기한다.
                obj.ClearBuffer();

                // 수신 대기
                obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, OrderDataReceived, obj);
            }
            catch (Exception e)
            {
            }
        }


        public byte[] ImageToByteArray(System.Drawing.Image image)
        {
            MemoryStream ms = new MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            ms.Position = 0;
            return ms.ToArray();
        }

        public void SendToClient(int num)
        {
            foreach (ClientStatus client in connectedChatClients)
            {
                if (client.mySocket.IsBound)
                {
                    if (client.myNum == num)
                    {
                        Socket tempSock = client.mySocket;
                        string tts = TempStr;

                        // 문자열을 utf8 형식의 바이트로 변환한다.
                        byte[] bDts = Encoding.UTF8.GetBytes(tts);

                        // 서버에 전송한다.
                        client.mySocket.Send(bDts);
                        return;
                    }
                }
            }
        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e) //패널 위 클라이언트를 클릭했을 때
        {
            if (e.Button == MouseButtons.Right)
            {
                foreach(Control x in splitContainer1.Panel1.Controls)
                {
                    if(x.GetType() == typeof(System.Windows.Forms.PictureBox))
                    {
                        PictureBox a = (PictureBox)x;
                        if (a.Bounds.Contains(e.Location))
                        {
                            a.Enabled = true;
                            a.ContextMenuStrip = Cms;
                            Cms.Show(PointToScreen(e.Location));
                            a.Enabled = false;
                            Int32.TryParse(a.Name.Substring(10), out accessNum);
                        }
                    }
                }
            }
        }

        private void 채팅창열기ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formMadeByRcv = false;
            GetChatForm(this, accessNum.ToString() + "번 자리", accessNum, "");
        }

        private void btnStockManage_Click(object sender, EventArgs e)
        {
            ServerForm3 frmStockManage = new ServerForm3();
            frmStockManage.Show();
        }

        private void lvOrderList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            lvOrderList.Items.Remove(lvOrderList.GetItemAt(e.X, e.Y));
        }

        private void btnMemberManage_Click(object sender, EventArgs e)
        {
            ServerForm4 frmMemberManage = new ServerForm4();
            frmMemberManage.ShowDialog();
        }
    }
}
