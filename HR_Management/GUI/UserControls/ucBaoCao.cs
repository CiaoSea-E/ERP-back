using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using HR_Management.BLL;
using HR_Management.DTO;

namespace HR_Management.GUI.UserControls
{
    [System.ComponentModel.DesignerCategory("Code")]
    public class ucBaoCao : UserControl
    {
        private readonly ThongKeBaoCaoBLL _bllBaoCao = new ThongKeBaoCaoBLL();

        private DataGridView dgvDeptStats = null!;
        private DataGridView dgvEduStats = null!;
        private DataGridView dgvHistoryReports = null!;

        public ucBaoCao()
        {
            InitializeComponent();
            LoadAllStats();
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = ThemeColor.BodyBg;

            // ===== TOP: Actions Bar =====
            FlowLayoutPanel pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                BackColor = Color.White,
                Padding = new Padding(16, 12, 16, 12),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            pnlActions.Paint += (s, e) =>
            {
                using (Pen p = new Pen(ThemeColor.CardBorder, 1))
                {
                    e.Graphics.DrawLine(p, 0, pnlActions.Height - 1, pnlActions.Width, pnlActions.Height - 1);
                }
            };

            Button btnCreateReport = UIHelper.CreateButton("📝 Lập Báo Cáo Mới", ThemeColor.Primary, Color.White, 160, 38);
            btnCreateReport.Margin = new Padding(0, 0, 10, 6);
            btnCreateReport.Click += BtnCreateReport_Click;

            Button btnExportCsv = UIHelper.CreateButton("📥 Xuất Báo Cáo CSV", ThemeColor.Success, Color.White, 160, 38);
            btnExportCsv.Margin = new Padding(0, 0, 10, 6);
            btnExportCsv.Click += BtnExportCsv_Click;

            Button btnRefresh = UIHelper.CreateButton("🔄 Làm mới dữ liệu", Color.FromArgb(100, 116, 139), Color.White, 150, 38);
            btnRefresh.Margin = new Padding(0, 0, 0, 6);
            btnRefresh.Click += (s, e) => LoadAllStats();

            pnlActions.Controls.AddRange(new Control[] { btnCreateReport, btnExportCsv, btnRefresh });

            // Container for Tabs with margin/padding
            Panel pnlTabContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 12, 16, 16),
                BackColor = ThemeColor.BodyBg
            };

            TabControl tabStats = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(16, 8),
                Font = ThemeColor.BodyFont
            };

            Panel CreateGridCard(DataGridView dgv)
            {
                Panel card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(1) };
                card.Paint += (s, e) =>
                {
                    using (Pen p = new Pen(ThemeColor.CardBorder, 1))
                    {
                        e.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                    }
                };
                card.Controls.Add(dgv);
                return card;
            }

            // TAB 1: Phong ban & Quy luong
            TabPage tabDept = new TabPage { Text = "📊 Cơ Cấu Nhân Sự Theo PB", BackColor = ThemeColor.BodyBg, Padding = new Padding(12) };
            dgvDeptStats = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.StyleDataGridView(dgvDeptStats);
            dgvDeptStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabDept.Controls.Add(CreateGridCard(dgvDeptStats));

            // TAB 2: Trinh do hoc van
            TabPage tabEdu = new TabPage { Text = "🎓 Thống Kê Học Vấn", BackColor = ThemeColor.BodyBg, Padding = new Padding(12) };
            dgvEduStats = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.StyleDataGridView(dgvEduStats);
            dgvEduStats.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            tabEdu.Controls.Add(CreateGridCard(dgvEduStats));

            // TAB 3: Lich su bao cao
            TabPage tabHistory = new TabPage { Text = "📋 Nhật Ký Báo Cáo", BackColor = ThemeColor.BodyBg, Padding = new Padding(12) };
            dgvHistoryReports = new DataGridView { Dock = DockStyle.Fill };
            UIHelper.StyleDataGridView(dgvHistoryReports);
            dgvHistoryReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Mã BC", DataPropertyName = "MaBaoCao", FillWeight = 12 });
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tên Báo Cáo", DataPropertyName = "TenBaoCao", FillWeight = 22 });
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Loại", DataPropertyName = "LoaiBaoCao", FillWeight = 16 });
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Ngày Lập", DataPropertyName = "NgayLap", FillWeight = 12, DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" } });
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Người Lập", DataPropertyName = "NguoiLap", FillWeight = 16 });
            dgvHistoryReports.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Nội Dung", DataPropertyName = "NoiDung", FillWeight = 22 });
            tabHistory.Controls.Add(CreateGridCard(dgvHistoryReports));

            tabStats.TabPages.Add(tabDept);
            tabStats.TabPages.Add(tabEdu);
            tabStats.TabPages.Add(tabHistory);

            pnlTabContainer.Controls.Add(tabStats);

            this.Controls.Add(pnlTabContainer);
            this.Controls.Add(pnlActions);
        }

        public void LoadAllStats()
        {
            try
            {
                dgvDeptStats.DataSource = _bllBaoCao.GetDepartmentStats();
                dgvEduStats.DataSource = _bllBaoCao.GetEducationStats();
                dgvHistoryReports.DataSource = _bllBaoCao.GetAllReports();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lấy dữ liệu báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCreateReport_Click(object? sender, EventArgs e)
        {
            using (Form f = new Form())
            {
                f.Text = "LẬP BÁO CÁO THỐNG KÊ MỚI";
                f.ClientSize = new Size(1400, 800);
                f.MinimumSize = new Size(1400, 800);
                f.StartPosition = FormStartPosition.CenterParent;
                f.FormBorderStyle = FormBorderStyle.Sizable;
                f.MaximizeBox = true;
                f.MinimizeBox = false;
                f.BackColor = Color.White;
                f.Font = ThemeColor.BodyFont;

                Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 64, Padding = new Padding(24, 16, 24, 16) };
                Label lHeader = new Label { Text = "TẠO BÁO CÁO NHÂN SỰ", Font = ThemeColor.SubHeaderFont, ForeColor = ThemeColor.Primary, AutoSize = true };
                pnlHeader.Controls.Add(lHeader);

                TableLayoutPanel tlp = new TableLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    ColumnCount = 2,
                    RowCount = 3,
                    Padding = new Padding(36, 28, 36, 20),
                    AutoScroll = true
                };
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
                tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
                tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 160F));

                Label l1 = new Label { Text = "Tên báo cáo:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(4, 14, 16, 14) };
                TextBox txtTenBC = new TextBox { Dock = DockStyle.Fill, Text = "Báo cáo cơ cấu nhân sự định kỳ", Margin = new Padding(0, 10, 16, 10) };

                Label l2 = new Label { Text = "Loại báo cáo:", AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(4, 14, 16, 14) };
                ComboBox cboLoai = new ComboBox { Dock = DockStyle.Fill, DropDownStyle = ComboBoxStyle.DropDownList, Margin = new Padding(0, 10, 16, 10) };
                cboLoai.Items.AddRange(new object[] { "Biến động nhân sự", "Cơ cấu phòng ban", "Thống kê quỹ lương", "Hợp đồng lao động", "Khác" });
                cboLoai.SelectedIndex = 1;

                Label l3 = new Label { Text = "Ghi chú nội dung:", AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Top, Padding = new Padding(0, 6, 0, 0), Margin = new Padding(4, 12, 16, 12) };
                TextBox txtNoiDung = new TextBox
                {
                    Dock = DockStyle.Fill,
                    Multiline = true,
                    Height = 140,
                    MinimumSize = new Size(0, 130),
                    ScrollBars = ScrollBars.Vertical,
                    Text = "Báo cáo tổng hợp số liệu thực tế từ cơ sở dữ liệu phân hệ nhân sự.",
                    Margin = new Padding(0, 10, 16, 10)
                };

                tlp.Controls.Add(l1, 0, 0); tlp.Controls.Add(txtTenBC, 1, 0);
                tlp.Controls.Add(l2, 0, 1); tlp.Controls.Add(cboLoai, 1, 1);
                tlp.Controls.Add(l3, 0, 2); tlp.Controls.Add(txtNoiDung, 1, 2);

                Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 90, Padding = new Padding(24, 16, 24, 16) };
                FlowLayoutPanel flpButtons = new FlowLayoutPanel
                {
                    Dock = DockStyle.Right,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    WrapContents = false,
                    FlowDirection = FlowDirection.RightToLeft
                };

                Button btnCancel = UIHelper.CreateButton("Hủy", Color.FromArgb(148, 163, 184), Color.White, 100, 44);
                btnCancel.Click += (s2, e2) => f.Close();

                Button btnSave = UIHelper.CreateButton("💾 Lưu Báo Cáo", ThemeColor.Success, Color.White, 160, 44);
                btnSave.Margin = new Padding(0, 0, 16, 0);
                btnSave.Click += (s2, e2) =>
                {
                    var report = new ThongKeBaoCaoDTO
                    {
                        MaBaoCao = _bllBaoCao.GetNextId(),
                        TenBaoCao = txtTenBC.Text.Trim(),
                        LoaiBaoCao = cboLoai.SelectedItem?.ToString() ?? "Tổng hợp",
                        NgayLap = DateTime.Today,
                        ID_NV = HeThongBLL.CurrentUser?.ID_NV,
                        NoiDung = txtNoiDung.Text.Trim()
                    };

                    if (_bllBaoCao.CreateReport(report, out string err))
                    {
                        MessageBox.Show("Lưu báo cáo vào hệ thống thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        f.Close();
                        LoadAllStats();
                    }
                    else
                    {
                        MessageBox.Show(err, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                flpButtons.Controls.Add(btnCancel);
                flpButtons.Controls.Add(btnSave);
                pnlBottom.Controls.Add(flpButtons);

                f.Controls.Add(tlp);
                f.Controls.Add(pnlBottom);
                f.Controls.Add(pnlHeader);
                f.ShowDialog();
            }
        }

        private void BtnExportCsv_Click(object? sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV File (*.csv)|*.csv";
                sfd.FileName = $"BaoCao_NhanSu_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        DataTable dt = _bllBaoCao.GetDepartmentStats();
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            sb.Append(dt.Columns[i].ColumnName + (i == dt.Columns.Count - 1 ? "" : ","));
                        }
                        sb.AppendLine();

                        foreach (DataRow row in dt.Rows)
                        {
                            for (int i = 0; i < dt.Columns.Count; i++)
                            {
                                string val = row[i]?.ToString()?.Replace(",", " ") ?? "";
                                sb.Append(val + (i == dt.Columns.Count - 1 ? "" : ","));
                            }
                            sb.AppendLine();
                        }

                        File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("Xuất file báo cáo thành công tại: " + sfd.FileName, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xuất file: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
