using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Quanlydiemhocsinh
{
    
    public partial class Login : Form
    {
            
        public Login()
        {
            InitializeComponent();
            kohienthi.Visible = false;
        }

        private void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có thật sự muốn thoát??", "Thông báo!!", MessageBoxButtons.OKCancel) != System.Windows.Forms.DialogResult.OK)
               e.Cancel = true;
        }

        private void dangnhap_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            kohienthi.Visible = false;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                kohienthi.Text = "Vui lòng nhập tên đăng nhập và mật khẩu.";
                kohienthi.Visible = true;
                return;
            }

            SqlConnection connection = DatabaseHelper.GetConnection();

            if (connection != null)
            {
                try
                {
                    string query = "SELECT MonDay FROM GiaoVien WHERE GiaoVienID = @Username AND SoDienThoai = @Password";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);

                    object result = command.ExecuteScalar();

                    if (result != null)
                    {
                        string monDay = result.ToString();
                        Console.WriteLine("Đăng nhập thành công! Môn dạy: " + monDay);
                        this.Hide();

                        // Truyền môn dạy vào form Diem
                        Diem mainForm = new Diem(monDay);
                        mainForm.Show();
                    }
                    else
                    {
                        kohienthi.Text = "Tên đăng nhập hoặc mật khẩu không đúng.";
                        kohienthi.Visible = true;
                        txtPassword.Clear();
                    }
                }
                catch (SqlException ex)
                {
                    kohienthi.Text = "Lỗi SQL: " + ex.Message;
                    kohienthi.Visible = true;
                    Console.WriteLine("Lỗi SQL: " + ex.Message);
                }
                catch (Exception ex)
                {
                    kohienthi.Text = "Lỗi khác: " + ex.Message;
                    kohienthi.Visible = true;
                    Console.WriteLine("Lỗi khác: " + ex.Message);
                }
                finally
                {
                    DatabaseHelper.CloseConnection(connection);
                }
            }
            else
            {
                kohienthi.Text = "Không thể kết nối đến cơ sở dữ liệu.";
                kohienthi.Visible = true;
                Console.WriteLine("Không thể tạo đối tượng kết nối.");
            }
        }
    }
}
