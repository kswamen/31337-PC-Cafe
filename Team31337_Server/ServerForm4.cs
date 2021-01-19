using System;
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
    public partial class ServerForm4 : Form
    {
        ListViewItem clickedItem = new ListViewItem();


        public ServerForm4()
        {
            InitializeComponent();
        }

        private void ServerForm4_Load(object sender, EventArgs e)
        {
            GetMemberDataFromDB();
        }

        public void GetMemberDataFromDB()
        {
            listView1.Items.Clear();

            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95";
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
                String[] arr = new String[4];
                arr[0] = reader["ID"].ToString();
                arr[1] = reader["Password"].ToString();
                arr[2] = reader["Name"].ToString();
                arr[3] = reader["PhoneNum"].ToString();

                ListViewItem temp = new ListViewItem(arr);

                listView1.Items.Add(temp);
            }

            conn.Dispose();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "Delete From member Where ID='" + clickedItem.SubItems[0].Text + "'";

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

            GetMemberDataFromDB();
        }

        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            clickedItem = listView1.GetItemAt(e.X, e.Y);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string connstr = "Server=127.0.0.1;Port=3306;Database=31337;Uid=root;Pwd=amen95;CharSet=utf8;";
            MySqlConnection conn = new MySqlConnection(connstr);
            MySqlCommand cmd = conn.CreateCommand();

            string sql = "Update member Set Password = '123456789a' Where ID = '" + clickedItem.SubItems[0].Text + "'";

            cmd.CommandText = sql;

            MessageBox.Show(sql);

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

            GetMemberDataFromDB();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
