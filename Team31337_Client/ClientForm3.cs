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

namespace Team31337_Client
{
    public partial class ClientForm3 : Form
    {
        public int myNum;
        public string sortSend;
        ListViewItem clickedItem = new ListViewItem();
        delegate void AppendListViewItemDelegate(Control ctrl, ListViewItem item);
        delegate void SetForeColorLvDelegate(Control ctrl, Color clr);
        delegate void CloseThisFormDelegate(Control ctrl);
        AppendListViewItemDelegate _lvItemAppender;
        SetForeColorLvDelegate _colorSetter;
        CloseThisFormDelegate _formCloser;
        Socket mainSock;
        IPAddress thisAddress;
        SoundClass AnySound = new SoundClass();

        public ClientForm3()
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            _lvItemAppender = new AppendListViewItemDelegate(LvItemAppend);
            _colorSetter = new SetForeColorLvDelegate(LvSetColor);
            _formCloser = new CloseThisFormDelegate(CloseThisForm);
        }

        public ClientForm3(int num)
        {
            InitializeComponent();
            mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            myNum = num;
            _lvItemAppender = new AppendListViewItemDelegate(LvItemAppend);
            _colorSetter = new SetForeColorLvDelegate(LvSetColor);
            _formCloser = new CloseThisFormDelegate(CloseThisForm);
        }

        void LvSetColor(Control ctrl, Color clr)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_colorSetter, ctrl, clr);
            else
            {
                for(int i=0; i<lvMenuList.Items.Count; i++) 
                {
                    if(lvMenuList.Items[i] != null)
                    {
                        if (lvMenuList.Items[i].SubItems[2].Text == "0")
                        {
                            lvMenuList.Items[i].ForeColor = clr;
                        }
                    }
                }
            }
        }

        void LvItemAppend(Control ctrl, ListViewItem item)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_lvItemAppender, ctrl, item);
            else
            {
                lvMenuList.Items.Add(item);
            }
        }

        void CloseThisForm(Control ctrl)
        {
            if (ctrl.InvokeRequired) ctrl.Invoke(_formCloser, ctrl);
            else
            {
                ctrl.Dispose();
            }
        }

        private void ClientForm3_Load(object sender, EventArgs e)
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

            int port = 8081;

            try
            {
                mainSock.Connect(thisAddress, port);
            }
            catch (Exception ex)
            {
                MessageBox.Show("연결에 실패했습니다!", ex.Message, MessageBoxButtons.OK);
                return;
            }

            AsyncObject obj = new AsyncObject(4096);
            obj.WorkingSocket = mainSock;

            byte[] bDts = Encoding.UTF8.GetBytes(myNum.ToString());
            obj.WorkingSocket.Send(bDts);

            mainSock.BeginReceive(obj.Buffer, 0, obj.BufferSize, 0, DataReceived, obj);
        }

        public class ReceivedArray
        {
            public int size;
            public byte[] arr;
            public ReceivedArray(int a)
            {
                size = a;
                arr = new byte[size];
            }
        }

        List<ReceivedArray> byteList = new List<ReceivedArray>();
        void DataReceived(IAsyncResult ar)
        {
            // BeginReceive에서 추가적으로 넘어온 데이터를 AsyncObject 형식으로 변환한다.
            AsyncObject obj = (AsyncObject)ar.AsyncState;

            // 데이터 수신을 끝낸다.
            int received = obj.WorkingSocket.EndReceive(ar);

            if (received > 0)
            {
                string text = Encoding.UTF8.GetString(obj.Buffer);
                //Debug.WriteLine(text);
                text = text.Trim('\0');

                if (text.Contains("ProductList")) //상품 목록 출력
                {
                    text = text.Replace("ProductList", "");

                    string[] receivedString = text.Split('\x02');
                    string[] temp;

                    for (int i = 0; i < receivedString.Length - 1; i++)
                    {
                        temp = receivedString[i].Split('\x01');
                        ListViewItem lvitem = new ListViewItem(temp);
                        LvItemAppend(lvMenuList, lvitem);
                    }
                    LvSetColor(lvMenuList, Color.Red);

                    obj.ClearBuffer();

                    // 수신 대기
                    obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
                }

                else if (text.Contains("OrderSuccessed"))
                {
                    OrderSuccessed();
                }

                else if (text.Contains("OrderFailed"))
                {
                    OrderFailed();
                }

                else //상품 이미지 출력
                {
                    ReceivedArray temp = new ReceivedArray(received);
                    temp.arr = (byte[])obj.Buffer.Clone();

                    byteList.Add(temp);

                    obj.ClearBuffer();

                    // 수신 대기
                    obj.WorkingSocket.BeginReceive(obj.Buffer, 0, 4096, 0, DataReceived, obj);
                }

            }
            // 받은 데이터가 없으면(연결끊어짐) 끝낸다.
            else if (received <= 0)
            {
                obj.WorkingSocket.Close();
                return;
            }
        }

        public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            ms.Position = 0;
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        private void SetProductImage()
        {
            int i = 0;
            foreach (ReceivedArray a in byteList)
            {
                i = i + a.size;
            }
            byte[] sumArr = new byte[i];
            i = 0;
            foreach (ReceivedArray a in byteList)
            {
                Array.Copy(a.arr, 0, sumArr, i, a.size);
                i = i + a.size;
            }

            pictureBox1.Image = byteArrayToImage(sumArr);
            byteList.Clear();
        }


        private void btnAll_Click(object sender, EventArgs e)
        {
            ManageBtnPress(false);
            lvMenuList.Items.Clear();

            if (!mainSock.IsBound)
            {
                MessageBox.Show("연결되지 않았습니다!");
                return;
            }
            sortSend = "All";
            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
            ManageBtnPress(true);
        }

        private void btnRamen_Click(object sender, EventArgs e)
        {
            ManageBtnPress(false);
            lvMenuList.Items.Clear();

            if (!mainSock.IsBound)
            {
                MessageBox.Show("연결되지 않았습니다!");
                return;
            }
            sortSend = "Ramen";
            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
            ManageBtnPress(true);
        }

        private void btnSnack_Click(object sender, EventArgs e)
        {
            ManageBtnPress(false);
            lvMenuList.Items.Clear();

            if (!mainSock.IsBound)
            {
                MessageBox.Show("연결되지 않았습니다!");
                return;
            }
            sortSend = "Snack";
            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
            ManageBtnPress(true);
        }

        private void btnBeverage_Click(object sender, EventArgs e)
        {
            ManageBtnPress(false);
            lvMenuList.Items.Clear();

            if (!mainSock.IsBound)
            {
                MessageBox.Show("연결되지 않았습니다!");
                return;
            }
            sortSend = "Beverage";
            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
            ManageBtnPress(true);
        }

        private void btnETC_Click(object sender, EventArgs e)
        {
            ManageBtnPress(false);
            lvMenuList.Items.Clear();

            if (!mainSock.IsBound)
            {
                MessageBox.Show("연결되지 않았습니다!");
                return;
            }
            sortSend = "ETC";
            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
            ManageBtnPress(true);
        }

        private void btnOrder_Click(object sender, EventArgs e)
        {
            if(lvCalc.Items.Count == 0)
            {
                MessageBox.Show("주문할 상품을 선택해 주세요!");
                return;
            }
            sortSend = "Order" + '\x01' + myNum.ToString() + '\x01';

            foreach(ListViewItem a in lvCalc.Items)
            {
                sortSend = sortSend + a.SubItems[0].Text + '\x01' + 
                    a.SubItems[2].Text + '\x01' + a.SubItems[3].Text + '\x02';
            }

            byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
            mainSock.Send(bDts);
        }

        private void OrderSuccessed()
        {
            AnySound.Ding();
            MessageBox.Show("주문을 받았습니다.\r\n잠시만 기다리시면 곧 자리로 가져다 드리겠습니다.");

            CloseThisForm(this);
        }

        private void OrderFailed()
        {
            AnySound.Beep();
            MessageBox.Show("주문 전송에 실패했습니다!\r\n잠시 후 다시 시도해 주세요.");

            CloseThisForm(this);
        }

        private void lvMenuList_MouseClick(object sender, MouseEventArgs e)
        {
            clickedItem = lvMenuList.GetItemAt(e.X, e.Y);
            if(clickedItem != null)
            {
                if (!mainSock.IsBound)
                {
                    MessageBox.Show("연결되지 않았습니다!");
                    return;
                }
                sortSend = "ProductImage" + '\x01' + clickedItem.SubItems[0].Text;
                byte[] bDts = Encoding.UTF8.GetBytes(sortSend);
                mainSock.Send(bDts);

                Thread.Sleep(300);

                SetProductImage();
                return;
            }
        }

        private void lvMenuList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            clickedItem = lvMenuList.GetItemAt(e.X, e.Y);
            if(clickedItem.SubItems[2].Text == "0")
            {
                MessageBox.Show("해당 상품의 재고가 부족하여 주문이 불가능합니다.");
                return;
            }
            foreach(ListViewItem a in lvCalc.Items)
            {
                if(clickedItem.SubItems[0].ToString() == a.SubItems[0].ToString())
                {
                    if(Convert.ToInt32(a.SubItems[2].Text) >= Convert.ToInt32(clickedItem.SubItems[2].Text))
                    {
                        MessageBox.Show("해당 상품의 재고가 부족하여 주문이 불가능합니다.");
                        return;
                    }
                    a.SubItems[2].Text = (Convert.ToInt32(a.SubItems[2].Text) + 1).ToString();
                    a.SubItems[3].Text = ((Convert.ToInt32(a.SubItems[1].Text) * 
                        Convert.ToInt32(a.SubItems[2].Text)).ToString());
                    tbSum.Text = (Convert.ToInt32(tbSum.Text) + Convert.ToInt32(a.SubItems[1].Text)).ToString();
                    return;
                }
            }

            string[] Temp = new string[4];
            Temp[0] = clickedItem.SubItems[0].Text;
            Temp[1] = clickedItem.SubItems[1].Text;
            Temp[2] = "1";
            Temp[3] = clickedItem.SubItems[1].Text;
            ListViewItem item = new ListViewItem(Temp);
            lvCalc.Items.Add(item);
            SumCalculate();
        }

        private void SumCalculate()
        {
            int i = 0;
            foreach(ListViewItem a in lvCalc.Items)
            {
                i = i + (Convert.ToInt32(a.SubItems[1].Text) * Convert.ToInt32(a.SubItems[2].Text));
            }

            tbSum.Text = i.ToString();
        }

        private void lvCalc_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            clickedItem = lvCalc.GetItemAt(e.X, e.Y);
            if (MessageBox.Show("해당 상품을 삭제하시겠습니까?", "31337", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                lvCalc.Items.Remove(clickedItem);
                SumCalculate();
            }
            else
            {
                return;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void ManageBtnPress(bool b)
        {
            btnAll.Enabled = b;
            btnRamen.Enabled = b;
            btnSnack.Enabled = b;
            btnBeverage.Enabled = b;
            btnETC.Enabled = b;
        }
    }
}
