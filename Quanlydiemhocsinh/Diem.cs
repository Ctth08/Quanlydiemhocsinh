using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quanlydiemhocsinh
{
    public partial class Diem : Form
    {
        private string currentHocKy = "HK1";
        private string currentLopID;
        private string giaoVienMonDay;

        public Diem(string monDay)
        {
            InitializeComponent();
            giaoVienMonDay = monDay;
            button1.Visible = !string.IsNullOrEmpty(giaoVienMonDay);
            menuHocKy1.Click += MenuHocKy_Click;
            menuHocKy2.Click += MenuHocKy_Click;
            comboBoxLop.SelectedIndexChanged += ComboBoxLop_SelectedIndexChanged;
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            menuHocKy1.Checked = true;
            menuHocKy2.Checked = false;
        }

        private void Diem_Load(object sender, EventArgs e)
        {
            List<Lop> danhSachLop = DatabaseHelper.GetDanhSachLop();
            comboBoxLop.DataSource = danhSachLop;
            comboBoxLop.DisplayMember = "TenLop";
            comboBoxLop.ValueMember = "LopID";
            if (!string.IsNullOrEmpty(giaoVienMonDay))
            {
                this.Text = "Điểm môn " + giaoVienMonDay;
            }
        }

        private void MenuHocKy_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clickedItem = (ToolStripMenuItem)sender;
            menuHocKy1.Checked = false;
            menuHocKy2.Checked = false;
            clickedItem.Checked = true;
            currentHocKy = (clickedItem == menuHocKy1) ? "HK1" : "HK2";
            if (comboBoxLop.SelectedItem != null)
            {
                LoadDiemHocSinh();
            }
        }

        private void ComboBoxLop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxLop.SelectedItem != null)
            {
                currentLopID = ((Lop)comboBoxLop.SelectedItem).LopID;
                LoadDiemHocSinh();
            }
        }

        private void LoadDiemHocSinh()
        {
            if (string.IsNullOrEmpty(currentLopID)) return;

            // Nếu là giáo viên, chỉ load môn họ dạy
            string monHoc = string.IsNullOrEmpty(giaoVienMonDay) ? null : giaoVienMonDay;

            List<DiemHocSinh> danhSachDiem = DatabaseHelper.GetDiemHocSinhTheoLopVaMonHoc(
                currentLopID, monHoc, currentHocKy);

            dataGridView1.DataSource = danhSachDiem;

            // Đặt tên cột
            if (dataGridView1.Columns.Contains("TenHocSinh"))
                dataGridView1.Columns["TenHocSinh"].HeaderText = "Học Sinh";
            if (dataGridView1.Columns.Contains("TenMonHoc"))
                dataGridView1.Columns["TenMonHoc"].HeaderText = "Môn Học";
            if (dataGridView1.Columns.Contains("TX1"))
                dataGridView1.Columns["TX1"].HeaderText = "TX1";
            if (dataGridView1.Columns.Contains("TX2"))
                dataGridView1.Columns["TX2"].HeaderText = "TX2";
            if (dataGridView1.Columns.Contains("TX3"))
                dataGridView1.Columns["TX3"].HeaderText = "TX3";
            if (dataGridView1.Columns.Contains("TX4"))
                dataGridView1.Columns["TX4"].HeaderText = "TX4";
            if (dataGridView1.Columns.Contains("GiuaKy"))
                dataGridView1.Columns["GiuaKy"].HeaderText = "Giữa Kỳ";
            if (dataGridView1.Columns.Contains("CuoiKy"))
                dataGridView1.Columns["CuoiKy"].HeaderText = "Cuối Kỳ";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            // Xử lý nút chỉnh sửa
            dataGridView1.ReadOnly = false;
            button1.Text = "Lưu";
            button1.Click -= Button1_Click;
            button1.Click += LuuThayDoi_Click;
        }

        private void LuuThayDoi_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (DataGridViewRow row in dataGridView1.Rows)
                {
                    if (!row.IsNewRow && row.Cells["DiemID"].Value != null)
                    {
                        int diemID = Convert.ToInt32(row.Cells["DiemID"].Value);
                        double? tx1 = row.Cells["TX1"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["TX1"].Value);
                        double? tx2 = row.Cells["TX2"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["TX2"].Value);
                        double? tx3 = row.Cells["TX3"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["TX3"].Value);
                        double? tx4 = row.Cells["TX4"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["TX4"].Value);
                        double? giuaKy = row.Cells["GiuaKy"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["GiuaKy"].Value);
                        double? cuoiKy = row.Cells["CuoiKy"].Value == null ? null : (double?)Convert.ToDouble(row.Cells["CuoiKy"].Value);

                        DatabaseHelper.CapNhatDiem(diemID, tx1, tx2, tx3, tx4, giuaKy, cuoiKy);
                    }
                }
                MessageBox.Show("Cập nhật điểm thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Trở về trạng thái ban đầu
            dataGridView1.ReadOnly = true;
            button1.Text = "Chỉnh Sửa";
            button1.Click -= LuuThayDoi_Click;
            button1.Click += Button1_Click;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Diem_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn thoát??", "Thông báo!!", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
                e.Cancel = true;
        }
    }
}