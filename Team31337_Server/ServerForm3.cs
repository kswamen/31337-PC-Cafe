using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;

namespace Team31337_Server
{
    public partial class ServerForm3 : Form
    {
        ListViewItem clickedItem = new ListViewItem();
        string ImagePath;
        string PathForSave= "";
        private int sortColumn = -1;

        public ServerForm3()
        {
            InitializeComponent();
        }

        private void ServerForm3_Load(object sender, EventArgs e)
        {
            GetProductDataFromDB();
        }

        public void GetProductDataFromDB()
        {
            listView1.Items.Clear();

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
                String[] arr = new String[5];
                arr[0] = reader["Name"].ToString();
                arr[1] = reader["Price"].ToString();
                arr[2] = reader["Stock"].ToString();
                arr[3] = reader["Sort"].ToString();
                arr[4] = reader["Image"].ToString();

                ListViewItem temp = new ListViewItem(arr);

                listView1.Items.Add(temp);
            }

            for(int i=0; i<listView1.Items.Count; i++)
            {
                if(listView1.Items[i].SubItems[2].Text == "0")
                {
                    listView1.Items[i].ForeColor = Color.Red ;
                }
            }

            conn.Dispose();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            clickedItem = listView1.GetItemAt(e.X, e.Y);
            tbName.Text = clickedItem.SubItems[0].Text;
            tbPrice.Text = clickedItem.SubItems[1].Text;
            tbStock.Text = clickedItem.SubItems[2].Text;
            if(tbStock.Text == "0")
            {
                btnReject.Text = "판매 불가 해제";
            }
            else
            {
                btnReject.Text = "판매 불가 설정";
            }
            for (int i = 0; i < cbSort.Items.Count; i++)
            {
                if (cbSort.Items[i].ToString() == clickedItem.SubItems[3].Text)
                {
                    cbSort.SelectedIndex = i;
                }
            }

            ImagePath = @"C:\31337\image\" + clickedItem.SubItems[4].Text;
            PathForSave = clickedItem.SubItems[4].Text;

            pbImg.Image = Bitmap.FromFile(ImagePath);
        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();


            string sql = "Update product Set Name='" + tbName.Text + "', Price=" +
                    tbPrice.Text + ", Stock=" + tbStock.Text + ", Image='" + Path.GetFileName(ImagePath)
                    + "', Sort='" + cbSort.SelectedItem.ToString()
                    + "' Where Name='" + clickedItem.SubItems[0].Text + "'";
          
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

            GetProductDataFromDB();
        }

        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();

            if(tbName.Text != null && tbPrice.Text != null && tbStock.Text != null && cbSort.Text != null
                && PathForSave != null)
            {
                try
                {
                    string sql = "INSERT INTO product VALUES('" + tbName.Text + "'," + tbPrice.Text + ","
                        + tbStock.Text + ",'" + cbSort.SelectedItem.ToString() + @"','" + PathForSave + "')";
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

                    GetProductDataFromDB();
                }
                catch(Exception eee)
                {
                    
                }
            }
        }

        private void btnImageSelect_Click(object sender, EventArgs e)
        {
            string image_file = string.Empty;
            fileDlg.InitialDirectory = @"C:\31337\image";
            fileDlg.Filter = "Images Files(*.jpg; *.jpeg; *.gif; *.bmp; *.png)|*.jpg;*.jpeg;*.gif;*.bmp;*.png";
            if(fileDlg.ShowDialog() == DialogResult.OK)
            {
                image_file = fileDlg.FileName;
            }
            else 
            {
                return;
            }

            ImagePath = image_file;
            PathForSave = Path.GetFileName(image_file);
            pbImg.Image = Bitmap.FromFile(ImagePath);
        }

        private bool IsAlreadyExist(String str)
        {
            foreach (ListViewItem item in listView1.Items)
            {
                if (str.Equals(item.SubItems[0].Text))
                {
                    clickedItem = item;
                    return true;
                }
            }

            return false;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "Delete From product Where Name='" + tbName.Text + "'";

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

            GetProductDataFromDB();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (btnReject.Text == "판매 불가 설정")
            {
                string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                MySqlConnection conn = new MySqlConnection(connstr);
                MySqlCommand cmd = conn.CreateCommand();

                string sql = "Update product Set Stock = 0 Where Name='" + clickedItem.SubItems[0].Text + "'";
                tbStock.Text = "0";

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
                GetProductDataFromDB();
            }
            else
            {
                string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
                MySqlConnection conn = new MySqlConnection(connstr);
                MySqlCommand cmd = conn.CreateCommand();

                string sql = "Update product Set Stock = 100 Where Name='" + clickedItem.SubItems[0].Text + "'";
                tbStock.Text = "100";

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
                GetProductDataFromDB();
            }
        }

        class MyListViewComparer : IComparer
        {
            private int col;
            private SortOrder order;
            public MyListViewComparer()
            {
                col = 0;
                order = SortOrder.Ascending;
            }
            public MyListViewComparer(int column, SortOrder order)
            {
                col = column;
                this.order = order;
            }
            public int Compare(object x, object y)
            {
                int returnVal = -1;
                returnVal = String.Compare(((ListViewItem)x).SubItems[col].Text, ((ListViewItem)y).SubItems[col].Text);
                if (order == SortOrder.Descending) 
                    returnVal *= -1;
                return returnVal;
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != sortColumn)
            {
                sortColumn = e.Column;
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                if(listView1.Sorting == SortOrder.Ascending)
                {
                    listView1.Sorting = SortOrder.Descending;
                }
                else
                {
                    listView1.Sorting = SortOrder.Ascending;
                }
            }

            listView1.Sort();
            this.listView1.ListViewItemSorter = new MyListViewComparer(e.Column, listView1.Sorting);
        }
    }
}
