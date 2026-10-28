using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace LoginFormApp
{
    public partial class Form2 : Form
    {
        // Kết nối chung tới CSDL LoginDB.mdf
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\LoginDB.mdf;Integrated Security=True;Connect Timeout=30";

        public class Course
        {
            public string CourseId { get; set; }
            public string CourseName { get; set; }
        }

        public Form2()
        {
            InitializeComponent();
            SetupRegisterControls();
            LoadCourseData();
        }

        private void SetupRegisterControls()
        {
            if (mtxtPhone != null)
                mtxtPhone.Mask = "(000) 000-0000";

            if (dtpBirthDate != null)
            {
                dtpBirthDate.Format = DateTimePickerFormat.Custom;
                dtpBirthDate.CustomFormat = "dd/MM/yyyy";
            }

            if (rdoMale != null)
                rdoMale.Checked = true;
        }

        private void LoadCourseData()
        {
            if (cboCourse == null) return;

            List<Course> courses = new List<Course>
            {
                new Course { CourseId = "C01", CourseName = "Lập trình C# WinForms" },
                new Course { CourseId = "C02", CourseName = "Lập trình Web ASP.NET Core" },
                new Course { CourseId = "C03", CourseName = "Quản trị CSDL SQL Server" },
                new Course { CourseId = "C04", CourseName = "Lập trình Mobile (Flutter)" }
            };

            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "CourseName";
            cboCourse.ValueMember = "CourseId";
            cboCourse.SelectedIndex = 0;
        }

        // Nút Đăng ký -> Đẩy dữ liệu lên bảng Students
        private void btnRegister_Click(object sender, EventArgs e)
        {
            if (!mtxtPhone.MaskCompleted)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Số điện thoại!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                mtxtPhone.Focus();
                return;
            }

            string phone = mtxtPhone.Text;
            DateTime birthDate = dtpBirthDate.Value;
            string gender = rdoMale.Checked ? "Nam" : "Nữ";
            string courseId = cboCourse.SelectedValue?.ToString();
            string courseName = cboCourse.Text;
            bool isPaid = chkStatus.Checked;

            string query = @"INSERT INTO Students (Phone, BirthDate, Gender, CourseId, CourseName, IsPaid) 
                            VALUES (@Phone, @BirthDate, @Gender, @CourseId, @CourseName, @IsPaid)";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                        cmd.Parameters.AddWithValue("@Gender", gender);
                        cmd.Parameters.AddWithValue("@CourseId", courseId);
                        cmd.Parameters.AddWithValue("@CourseName", courseName);
                        cmd.Parameters.AddWithValue("@IsPaid", isPaid);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            string statusText = isPaid ? "Đã hoàn thành học phí" : "Chưa hoàn thành học phí";

                            string info = $"ĐÃ LƯU VÀO CSDL THÀNH CÔNG!\n\n" +
                                          $"• SĐT: {phone}\n" +
                                          $"• Ngày sinh: {birthDate:dd/MM/yyyy}\n" +
                                          $"• Giới tính: {gender}\n" +
                                          $"• Khóa học: {courseName} ({courseId})\n" +
                                          $"• Trạng thái: {statusText}";

                            MessageBox.Show(info, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối hoặc lưu CSDL: " + ex.Message, "Lỗi Hệ Thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}