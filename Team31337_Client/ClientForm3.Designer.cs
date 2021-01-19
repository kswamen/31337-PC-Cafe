namespace Team31337_Client
{
    partial class ClientForm3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClientForm3));
            this.btnAll = new System.Windows.Forms.Button();
            this.btnRamen = new System.Windows.Forms.Button();
            this.btnSnack = new System.Windows.Forms.Button();
            this.btnBeverage = new System.Windows.Forms.Button();
            this.btnETC = new System.Windows.Forms.Button();
            this.btnOrder = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.lvMenuList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lvCalc = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tbSum = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnAll
            // 
            this.btnAll.Location = new System.Drawing.Point(12, 12);
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(200, 100);
            this.btnAll.TabIndex = 0;
            this.btnAll.Text = "전 체";
            this.btnAll.UseVisualStyleBackColor = true;
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnRamen
            // 
            this.btnRamen.Location = new System.Drawing.Point(12, 118);
            this.btnRamen.Name = "btnRamen";
            this.btnRamen.Size = new System.Drawing.Size(200, 100);
            this.btnRamen.TabIndex = 1;
            this.btnRamen.Text = "라 면";
            this.btnRamen.UseVisualStyleBackColor = true;
            this.btnRamen.Click += new System.EventHandler(this.btnRamen_Click);
            // 
            // btnSnack
            // 
            this.btnSnack.Location = new System.Drawing.Point(12, 224);
            this.btnSnack.Name = "btnSnack";
            this.btnSnack.Size = new System.Drawing.Size(200, 100);
            this.btnSnack.TabIndex = 2;
            this.btnSnack.Text = "과 자";
            this.btnSnack.UseVisualStyleBackColor = true;
            this.btnSnack.Click += new System.EventHandler(this.btnSnack_Click);
            // 
            // btnBeverage
            // 
            this.btnBeverage.Location = new System.Drawing.Point(12, 330);
            this.btnBeverage.Name = "btnBeverage";
            this.btnBeverage.Size = new System.Drawing.Size(200, 100);
            this.btnBeverage.TabIndex = 3;
            this.btnBeverage.Text = "음 료 수";
            this.btnBeverage.UseVisualStyleBackColor = true;
            this.btnBeverage.Click += new System.EventHandler(this.btnBeverage_Click);
            // 
            // btnETC
            // 
            this.btnETC.Location = new System.Drawing.Point(12, 436);
            this.btnETC.Name = "btnETC";
            this.btnETC.Size = new System.Drawing.Size(200, 100);
            this.btnETC.TabIndex = 4;
            this.btnETC.Text = "기타 간식";
            this.btnETC.UseVisualStyleBackColor = true;
            this.btnETC.Click += new System.EventHandler(this.btnETC_Click);
            // 
            // btnOrder
            // 
            this.btnOrder.Location = new System.Drawing.Point(503, 436);
            this.btnOrder.Name = "btnOrder";
            this.btnOrder.Size = new System.Drawing.Size(300, 100);
            this.btnOrder.TabIndex = 5;
            this.btnOrder.Text = "상품주문";
            this.btnOrder.UseVisualStyleBackColor = true;
            this.btnOrder.Click += new System.EventHandler(this.btnOrder_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(809, 436);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(300, 100);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "닫기";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lvMenuList
            // 
            this.lvMenuList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvMenuList.FullRowSelect = true;
            this.lvMenuList.GridLines = true;
            this.lvMenuList.Location = new System.Drawing.Point(218, 12);
            this.lvMenuList.Name = "lvMenuList";
            this.lvMenuList.Size = new System.Drawing.Size(279, 524);
            this.lvMenuList.TabIndex = 7;
            this.lvMenuList.UseCompatibleStateImageBehavior = false;
            this.lvMenuList.View = System.Windows.Forms.View.Details;
            this.lvMenuList.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lvMenuList_MouseClick);
            this.lvMenuList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvMenuList_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "상품명";
            this.columnHeader1.Width = 130;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "가격";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "재고";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 70;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(503, 80);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(300, 300);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 8;
            this.pictureBox1.TabStop = false;
            // 
            // lvCalc
            // 
            this.lvCalc.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.lvCalc.FullRowSelect = true;
            this.lvCalc.GridLines = true;
            this.lvCalc.Location = new System.Drawing.Point(809, 12);
            this.lvCalc.Name = "lvCalc";
            this.lvCalc.Size = new System.Drawing.Size(297, 368);
            this.lvCalc.TabIndex = 9;
            this.lvCalc.UseCompatibleStateImageBehavior = false;
            this.lvCalc.View = System.Windows.Forms.View.Details;
            this.lvCalc.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lvCalc_MouseDoubleClick);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "상품명";
            this.columnHeader4.Width = 100;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "가격";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "갯수";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "총 금액";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbSum
            // 
            this.tbSum.Location = new System.Drawing.Point(1006, 397);
            this.tbSum.Name = "tbSum";
            this.tbSum.Size = new System.Drawing.Size(100, 21);
            this.tbSum.TabIndex = 10;
            this.tbSum.Text = "0";
            this.tbSum.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.label1.Location = new System.Drawing.Point(919, 402);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "상품 총 금액 :";
            // 
            // ClientForm3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(1118, 544);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbSum);
            this.Controls.Add(this.lvCalc);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lvMenuList);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnOrder);
            this.Controls.Add(this.btnETC);
            this.Controls.Add(this.btnBeverage);
            this.Controls.Add(this.btnSnack);
            this.Controls.Add(this.btnRamen);
            this.Controls.Add(this.btnAll);
            this.Name = "ClientForm3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ClientForm3";
            this.Load += new System.EventHandler(this.ClientForm3_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAll;
        private System.Windows.Forms.Button btnRamen;
        private System.Windows.Forms.Button btnSnack;
        private System.Windows.Forms.Button btnBeverage;
        private System.Windows.Forms.Button btnETC;
        private System.Windows.Forms.Button btnOrder;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.ListView lvMenuList;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ListView lvCalc;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.TextBox tbSum;
        private System.Windows.Forms.Label label1;
    }
}