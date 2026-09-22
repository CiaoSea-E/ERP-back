namespace ERP
{
    partial class FrmSuaPhuongTien
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lblBienSoXe = new System.Windows.Forms.Label();
            this.txtBienSoXe = new System.Windows.Forms.TextBox();
            this.lblLoaiXe = new System.Windows.Forms.Label();
            this.txtLoaiXe = new System.Windows.Forms.TextBox();
            this.lblTaiTrong = new System.Windows.Forms.Label();
            this.txtTaiTrong = new System.Windows.Forms.TextBox();
            this.lblTenTaiXe = new System.Windows.Forms.Label();
            this.txtTenTaiXe = new System.Windows.Forms.TextBox();
            this.lblTrangThaiXe = new System.Windows.Forms.Label();
            this.cmbTrangThaiXe = new System.Windows.Forms.ComboBox();
            this.chkKichHoat = new System.Windows.Forms.CheckBox();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblBienSoXe
            // 
            this.lblBienSoXe.AutoSize = true;
            this.lblBienSoXe.Location = new System.Drawing.Point(30, 30);
            this.lblBienSoXe.Name = "lblBienSoXe";
            this.lblBienSoXe.Size = new System.Drawing.Size(64, 15);
            this.lblBienSoXe.TabIndex = 0;
            this.lblBienSoXe.Text = "Biển Số Xe:";
            // 
            // txtBienSoXe
            // 
            this.txtBienSoXe.Location = new System.Drawing.Point(150, 27);
            this.txtBienSoXe.Name = "txtBienSoXe";
            this.txtBienSoXe.Size = new System.Drawing.Size(290, 23);
            this.txtBienSoXe.TabIndex = 1;
            // 
            // lblLoaiXe
            // 
            this.lblLoaiXe.AutoSize = true;
            this.lblLoaiXe.Location = new System.Drawing.Point(30, 75);
            this.lblLoaiXe.Name = "lblLoaiXe";
            this.lblLoaiXe.Size = new System.Drawing.Size(46, 15);
            this.lblLoaiXe.TabIndex = 2;
            this.lblLoaiXe.Text = "Loại Xe:";
            // 
            // txtLoaiXe
            // 
            this.txtLoaiXe.Location = new System.Drawing.Point(150, 72);
            this.txtLoaiXe.Name = "txtLoaiXe";
            this.txtLoaiXe.Size = new System.Drawing.Size(290, 23);
            this.txtLoaiXe.TabIndex = 2;
            // 
            // lblTaiTrong
            // 
            this.lblTaiTrong.AutoSize = true;
            this.lblTaiTrong.Location = new System.Drawing.Point(30, 120);
            this.lblTaiTrong.Name = "lblTaiTrong";
            this.lblTaiTrong.Size = new System.Drawing.Size(73, 15);
            this.lblTaiTrong.TabIndex = 4;
            this.lblTaiTrong.Text = "Tải Trọng (T):";
            // 
            // txtTaiTrong
            // 
            this.txtTaiTrong.Location = new System.Drawing.Point(150, 117);
            this.txtTaiTrong.Name = "txtTaiTrong";
            this.txtTaiTrong.Size = new System.Drawing.Size(290, 23);
            this.txtTaiTrong.TabIndex = 3;
            // 
            // lblTenTaiXe
            // 
            this.lblTenTaiXe.AutoSize = true;
            this.lblTenTaiXe.Location = new System.Drawing.Point(30, 165);
            this.lblTenTaiXe.Name = "lblTenTaiXe";
            this.lblTenTaiXe.Size = new System.Drawing.Size(63, 15);
            this.lblTenTaiXe.TabIndex = 6;
            this.lblTenTaiXe.Text = "Tên Tài Xế:";
            // 
            // txtTenTaiXe
            // 
            this.txtTenTaiXe.Location = new System.Drawing.Point(150, 162);
            this.txtTenTaiXe.Name = "txtTenTaiXe";
            this.txtTenTaiXe.Size = new System.Drawing.Size(290, 23);
            this.txtTenTaiXe.TabIndex = 4;
            // 
            // lblTrangThaiXe
            // 
            this.lblTrangThaiXe.AutoSize = true;
            this.lblTrangThaiXe.Location = new System.Drawing.Point(30, 210);
            this.lblTrangThaiXe.Name = "lblTrangThaiXe";
            this.lblTrangThaiXe.Size = new System.Drawing.Size(81, 15);
            this.lblTrangThaiXe.TabIndex = 8;
            this.lblTrangThaiXe.Text = "Trạng Thái Xe:";
            // 
            // cmbTrangThaiXe
            // 
            this.cmbTrangThaiXe.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTrangThaiXe.FormattingEnabled = true;
            this.cmbTrangThaiXe.Items.AddRange(new object[] {
            "Đang rảnh",
            "Đang vận chuyển",
            "Bảo trì"});
            this.cmbTrangThaiXe.Location = new System.Drawing.Point(150, 207);
            this.cmbTrangThaiXe.Name = "cmbTrangThaiXe";
            this.cmbTrangThaiXe.Size = new System.Drawing.Size(290, 23);
            this.cmbTrangThaiXe.TabIndex = 5;
            // 
            // chkKichHoat
            // 
            this.chkKichHoat.AutoSize = true;
            this.chkKichHoat.Location = new System.Drawing.Point(150, 250);
            this.chkKichHoat.Name = "chkKichHoat";
            this.chkKichHoat.Size = new System.Drawing.Size(78, 19);
            this.chkKichHoat.TabIndex = 6;
            this.chkKichHoat.Text = "Kích Hoạt";
            this.chkKichHoat.UseVisualStyleBackColor = true;
            // 
            // btnLuu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Location = new System.Drawing.Point(234, 290);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(100, 35);
            this.btnLuu.TabIndex = 7;
            this.btnLuu.Text = "Cập Nhật";
            this.btnLuu.UseVisualStyleBackColor = false;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Location = new System.Drawing.Point(340, 290);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(100, 35);
            this.btnHuy.TabIndex = 8;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // FrmSuaPhuongTien
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 350);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.chkKichHoat);
            this.Controls.Add(this.cmbTrangThaiXe);
            this.Controls.Add(this.lblTrangThaiXe);
            this.Controls.Add(this.txtTenTaiXe);
            this.Controls.Add(this.lblTenTaiXe);
            this.Controls.Add(this.txtTaiTrong);
            this.Controls.Add(this.lblTaiTrong);
            this.Controls.Add(this.txtLoaiXe);
            this.Controls.Add(this.lblLoaiXe);
            this.Controls.Add(this.txtBienSoXe);
            this.Controls.Add(this.lblBienSoXe);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmSuaPhuongTien";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Cập Nhật Thông Tin Phương Tiện";
            this.Load += new System.EventHandler(this.FrmSuaPhuongTien_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblBienSoXe;
        private System.Windows.Forms.TextBox txtBienSoXe;
        private System.Windows.Forms.Label lblLoaiXe;
        private System.Windows.Forms.TextBox txtLoaiXe;
        private System.Windows.Forms.Label lblTaiTrong;
        private System.Windows.Forms.TextBox txtTaiTrong;
        private System.Windows.Forms.Label lblTenTaiXe;
        private System.Windows.Forms.TextBox txtTenTaiXe;
        private System.Windows.Forms.Label lblTrangThaiXe;
        private System.Windows.Forms.ComboBox cmbTrangThaiXe;
        private System.Windows.Forms.CheckBox chkKichHoat;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnHuy;
    }
}