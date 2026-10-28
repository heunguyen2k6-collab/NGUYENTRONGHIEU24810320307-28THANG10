using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace LoginFormApp
{
    public partial class Form1 : Form
    {
        // Chuỗi kết nối tự động trỏ vào file LoginDB.mdf trong project
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LoginDB.mdf;Integrated Security=True;Connect Timeout=30";

        public Form1()
        {
            InitializeComponent();

            // Phím tắt Enter để Đăng nhập, Esc để Thoát
            this.AcceptButton = btnLogin;
            this.CancelButton = btnExit;

            // Ẩn mật khẩu
            txtPassword.UseSystemPasswordChar = true;
        }

        // 1. Mã hóa SHA-256
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        // 2. Validate dữ liệu
        private bool ValidateInput()
        {
            bool isValid = true;
            errorProvider1.Clear();

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider1.SetError(txtUsername, "Vui lòng nhập Tên đăng nhập!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider1.SetError(txtPassword, "Vui lòng nhập Mật khẩu!");
                isValid = false;
            }

            return isValid;
        }

        // 3. CheckBox Ẩn/Hiện mật khẩu
        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // 4. Xử lý Đăng nhập
        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;

            string username = txtUsername.Text.Trim();
            string hashedPassword = HashPassword(txtPassword.Text.Trim());

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT FullName FROM Users WHERE Username = @Username AND PasswordHash = @PasswordHash";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", username);
                        cmd.Parameters.AddWithValue("@PasswordHash", hashedPassword);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            string fullName = result.ToString();
                            MessageBox.Show($"Đăng nhập thành công! Chào mừng {fullName}.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // --- CHUYỂN SANG FORM2 ---
                            this.Hide(); // Ẩn Form đăng nhập

                            Form2 f2 = new Form2();
                            f2.ShowDialog(); // Hiển thị Form Đăng ký Học viên

                            this.Close(); // Đóng Form1 khi tắt Form2
                        }
                        else
                        {
                            MessageBox.Show("Tên đăng nhập hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 5. Nút Thoát
        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}