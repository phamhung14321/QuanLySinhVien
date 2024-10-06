using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Migrations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLySinhVien.Model;

namespace QuanLySinhVien
{
    public partial class Form1 : Form
    {
        private StudentContentDB StudentDB = new StudentContentDB();


        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            List<Student> listStudent = StudentDB.Student.ToList();
            List<Faculty> listFaculty = StudentDB.Faculty.ToList();
            FillDataCBB(listFaculty);
            FillDataDGV(listStudent);
        }

        private void FillDataDGV(List<Student> listStudent)
        {
            dgvSinhVien.Rows.Clear();
            foreach (var student in listStudent)
            {
                int RowNew = dgvSinhVien.Rows.Add();
                dgvSinhVien.Rows[RowNew].Cells[0].Value = student.StudentID;
                dgvSinhVien.Rows[RowNew].Cells[1].Value = student.FullName;
                dgvSinhVien.Rows[RowNew].Cells[2].Value = student.AverageScore;
                dgvSinhVien.Rows[RowNew].Cells[3].Value = student.Faculty.FacultyName;
            }
        }

        private void FillDataCBB(List<Faculty> listFaculty)
        {
            cmbKhoa.DataSource = listFaculty;
            cmbKhoa.DisplayMember = "FacultyName";
            cmbKhoa.ValueMember = "FacultyID";
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (CheckDataInput())
            {
                if (!CheckIdSinhVien(txtMSSV.Text)) 
                {
                    Student newStudent = new Student();
                    newStudent.StudentID = txtMSSV.Text;
                    newStudent.FullName = txtHoTen.Text;
                    newStudent.AverageScore = Convert.ToDouble(txtDiem.Text);
                    newStudent.FacultyID = Convert.ToInt32(cmbKhoa.SelectedValue.ToString());

                    StudentDB.Student.AddOrUpdate(newStudent);
                    StudentDB.SaveChanges();

                    loaddgvSinhVien();
                    loadForm();
                    MessageBox.Show($"Thêm sinh viên {newStudent.FullName} vào danh sách thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"Sinh viên có mã số {txtMSSV.Text} đã tồn tại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }


        private bool CheckDataInput()
        {
            if (txtMSSV.Text == "" || txtHoTen.Text == "" || txtDiem.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập đúng thông tin!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else if (txtMSSV.TextLength < 5)
            {
                MessageBox.Show("Mã số sinh viên nhập chưa đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                float kq = 0;
                bool KetQua = float.TryParse(txtDiem.Text, out kq);
                if (!KetQua)
                {
                    MessageBox.Show("Điểm sinh viên chưa đúng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            return true;
        }

        private void loadForm()
        {
            txtMSSV.Clear();
            txtHoTen.Clear();
            txtDiem.Clear();
        }

        private void loaddgvSinhVien()
        {
            List<Student> newListStudent = StudentDB.Student.ToList();
            FillDataDGV(newListStudent);
        }

        private bool CheckIdSinhVien(string idNewStudent)
        {
            int length = dgvSinhVien.Rows.Count;
            for (int i = 0; i < length; i++)
            {
                if (dgvSinhVien.Rows[i].Cells[0].Value != null)
                {
                    if (dgvSinhVien.Rows[i].Cells[0].Value.ToString() == idNewStudent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                if (CheckDataInput())
                {
                    string selectedStudentID = dgvSinhVien.SelectedRows[0].Cells[0].Value.ToString();
                    Student updateStudent = StudentDB.Student.FirstOrDefault(s => s.StudentID == selectedStudentID);
                    if (updateStudent != null)
                    {
                        DialogResult confirmResult = MessageBox.Show($"Bạn có chắc muốn sửa thông tin của sinh viên {updateStudent.FullName}?","Xác nhận sửa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        if (confirmResult == DialogResult.Yes)
                        {
                            updateStudent.FullName = txtHoTen.Text;
                            updateStudent.AverageScore = Convert.ToDouble(txtDiem.Text);
                            updateStudent.FacultyID = Convert.ToInt32(cmbKhoa.SelectedValue.ToString());
                            StudentDB.Student.AddOrUpdate(updateStudent);
                            StudentDB.SaveChanges();
                            loaddgvSinhVien();
                            loadForm();
                            MessageBox.Show($"Chỉnh sửa dữ liệu sinh viên {updateStudent.FullName} thành công!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy sinh viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa chọn sinh viên để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }


        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvSinhVien.SelectedRows.Count > 0)
            {
                string selectedStudentID = dgvSinhVien.SelectedRows[0].Cells[0].Value.ToString();
                Student deleteStudent = StudentDB.Student.FirstOrDefault(s => s.StudentID == selectedStudentID);

                if (deleteStudent != null)
                {
                    DialogResult confirmResult = MessageBox.Show($"Bạn có chắc muốn xóa sinh viên {deleteStudent.FullName}?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (confirmResult == DialogResult.Yes)
                    {
                        StudentDB.Student.Remove(deleteStudent);
                        StudentDB.SaveChanges();
                        loaddgvSinhVien();
                        MessageBox.Show($"Xóa sinh viên {deleteStudent.FullName} thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sinh viên cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa chọn sinh viên để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

