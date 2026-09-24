namespace ERP
{
    partial class FrmSuaDiemVanChuyen
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
            this.lblMaDVC = new System.Windows.Forms.Label();
            this.txtMaDVC = new System.Windows.Forms.TextBox();
            this.lblTenDVC = new System.Windows.Forms.Label();
            this.txtTenDVC = new System.Windows.Forms.TextBox();
            this.lblDiaChiDVC = new System.Windows.Forms.Label();
            this.txtDiaChiDVC = new System.Windows.Forms.TextBox();
            this.lblSDT_DVC = new System.Windows.Forms.Label();
            this.txtSDT_DVC = new System.Windows.Forms.TextBox();
            this.lblTenNguoiDaiDien = new System.Windows.Forms.Label();
            this.txtTenNguoiDaiDien = new System.Windows.Forms.TextBox();
            this.btnLuu = new System.Windows.Forms.Button();
            this.btnHuy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblMaDVC
            // 
            this.lblMaDVC.AutoSize = true;
            this.lblMaDVC.Location = new System.Drawing.Point(30, 30);
            this.lblMaDVC.Name = "lblMaDVC";
            this.lblMaDVC.Size = new System.Drawing.Size(52, 15);
            this.lblMaDVC.TabIndex = 0;
            this.lblMaDVC.Text = "Mã DVC:";
            // 
            // txtMaDVC
            // 
            this.txtMaDVC.Location = new System.Drawing.Point(150, 27);
            this.txtMaDVC.Name = "txtMaDVC";
            this.txtMaDVC.Size = new System.Drawing.Size(290, 23);
            this.txtMaDVC.TabIndex = 1;
            // 
            // lblTenDVC
            // 
            this.lblTenDVC.AutoSize = true;
            this.lblTenDVC.Location = new System.Drawing.Point(30, 75);
            this.lblTenDVC.Name = "lblTenDVC";
            this.lblTenDVC.Size = new System.Drawing.Size(56, 15);
            this.lblTenDVC.TabIndex = 2;
            this.lblTenDVC.Text = "Tên DVC:";
            // 
            // txtTenDVC
            // 
            this.txtTenDVC.Location = new System.Drawing.Point(150, 72);
            this.txtTenDVC.Name = "txtTenDVC";
            this.txtTenDVC.Size = new System.Drawing.Size(290, 23);
            this.txtTenDVC.TabIndex = 2;
            // 
            // lblDiaChiDVC
            // 
            this.lblDiaChiDVC.AutoSize = true;
            this.lblDiaChiDVC.Location = new System.Drawing.Point(30, 120);
            this.lblDiaChiDVC.Name = "lblDiaChiDVC";
            this.lblDiaChiDVC.Size = new System.Drawing.Size(73, 15);
            this.lblDiaChiDVC.TabIndex = 4;
            this.lblDiaChiDVC.Text = "Địa Chỉ DVC:";
            // 
            // txtDiaChiDVC
            // 
            this.txtDiaChiDVC.Location = new System.Drawing.Point(150, 117);
            this.txtDiaChiDVC.Name = "txtDiaChiDVC";
            this.txtDiaChiDVC.Size = new System.Drawing.Size(290, 23);
            this.txtDiaChiDVC.TabIndex = 3;
            // 
            // lblSDT_DVC
            // 
            this.lblSDT_DVC.AutoSize = true;
            this.lblSDT_DVC.Location = new System.Drawing.Point(30, 165);
            this.lblSDT_DVC.Name = "lblSDT_DVC";
            this.lblSDT_DVC.Size = new System.Drawing.Size(60, 15);
            this.lblSDT_DVC.TabIndex = 6;
            this.lblSDT_DVC.Text = "SĐT DVC:";
            // 
            // txtSDT_DVC
            // 
            this.txtSDT_DVC.Location = new System.Drawing.Point(150, 162);
            this.txtSDT_DVC.Name = "txtSDT_DVC";
            this.txtSDT_DVC.Size = new System.Drawing.Size(290, 23);
            this.txtSDT_DVC.TabIndex = 4;
            // 
            // lblTenNguoiDaiDien
            // 
            this.lblTenNguoiDaiDien.AutoSize = true;
            this.lblTenNguoiDaiDien.Location = new System.Drawing.Point(30, 210);
            this.lblTenNguoiDaiDien.Name = "lblTenNguoiDaiDien";
            this.lblTenNguoiDaiDien.Size = new System.Drawing.Size(110, 15);
            this.lblTenNguoiDaiDien.TabIndex = 8;
            this.lblTenNguoiDaiDien.Text = "Người Đại Diện:";
            // 
            // txtTenNguoiDaiDien
            // 
            this.txtTenNguoiDaiDien.Location = new System.Drawing.Point(150, 207);
            this.txtTenNguoiDaiDien.Name = "txtTenNguoiDaiDien";
            this.txtTenNguoiDaiDien.Size = new System.Drawing.Size(290, 23);
            this.txtTenNguoiDaiDien.TabIndex = 5;
            // 
            // btnLuu
            // 
            this.btnLuu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(13)))), ((int)(((byte)(110)))), ((int)(((byte)(253)))));
            this.btnLuu.ForeColor = System.Drawing.Color.White;
            this.btnLuu.Location = new System.Drawing.Point(234, 260);
            this.btnLuu.Name = "btnLuu";
            this.btnLuu.Size = new System.Drawing.Size(100, 35);
            this.btnLuu.TabIndex = 6;
            this.btnLuu.Text = "Cập Nhật";
            this.btnLuu.UseVisualStyleBackColor = false;
            this.btnLuu.Click += new System.EventHandler(this.btnLuu_Click);
            // 
            // btnHuy
            // 
            this.btnHuy.Location = new System.Drawing.Point(340, 260);
            this.btnHuy.Name = "btnHuy";
            this.btnHuy.Size = new System.Drawing.Size(100, 35);
            this.btnHuy.TabIndex = 7;
            this.btnHuy.Text = "Hủy";
            this.btnHuy.UseVisualStyleBackColor = true;
            this.btnHuy.Click += new System.EventHandler(this.btnHuy_Click);
            // 
            // FrmSuaDiemVanChuyen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 320);
            this.Controls.Add(this.btnHuy);
            this.Controls.Add(this.btnLuu);
            this.Controls.Add(this.txtTenNguoiDaiDien);
            this.Controls.Add(this.lblTenNguoiDaiDien);
            this.Controls.Add(this.txtSDT_DVC);
            this.Controls.Add(this.lblSDT_DVC);
            this.Controls.Add(this.txtDiaChiDVC);
            this.Controls.Add(this.lblDiaChiDVC);
            this.Controls.Add(this.txtTenDVC);
            this.Controls.Add(this.lblTenDVC);
            this.Controls.Add(this.txtMaDVC);
            this.Controls.Add(this.lblMaDVC);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FrmSuaDiemVanChuyen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sửa Thông Tin Điểm Vận Chuyển";
            this.Load += new System.EventHandler(this.FrmSuaDiemVanChuyen_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lblMaDVC;
        private System.Windows.Forms.TextBox txtMaDVC;
        private System.Windows.Forms.Label lblTenDVC;
        private System.Windows.Forms.TextBox txtTenDVC;
        private System.Windows.Forms.Label lblDiaChiDVC;
        private System.Windows.Forms.TextBox txtDiaChiDVC;
        private System.Windows.Forms.Label lblSDT_DVC;
        private System.Windows.Forms.TextBox txtSDT_DVC;
        private System.Windows.Forms.Label lblTenNguoiDaiDien;
        private System.Windows.Forms.TextBox txtTenNguoiDaiDien;
        private System.Windows.Forms.Button btnLuu;
        private System.Windows.Forms.Button btnHuy;
    }
}