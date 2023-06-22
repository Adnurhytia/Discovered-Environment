using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.WebRequestMethods;
using System.Windows.Input;
using System.Collections;

namespace Discovered_Environment
{
    public partial class Halaman_Data_Status_Mahasiswa : Form
    {
        private string stringConnection = "data Source=DESKTOP-4IT269M\\ADINDANURHAYATI;" + "database=Universitas;User ID=sa;Password=3007dinda";
        private SqlConnection koneksi;

        private void refreshform()
        {
            cbxnama.Enabled = false;
            cbxStatusMahasiswa.Enabled = false;
            cbxTahunMasuk.Enabled = false;
            cbxnama.SelectedIndex = -1;
            cbxStatusMahasiswa.SelectedIndex = -1;
            cbxTahunMasuk.SelectedIndex = -1;
            txtNIM.Visible = false;
            btnSave.Enabled = false;
            btnClear.Enabled = false;
            btnAdd.Enabled = true;
        }
        public Halaman_Data_Status_Mahasiswa()
        {
            InitializeComponent();
            koneksi = new SqlConnection(stringConnection);
            refreshform();
        }

        private void Halaman_Data_Status_Mahasiswa_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void dataGridView()
        {
            koneksi.Open();
            string str = "select * from dbo.status_mahasiswa";
            SqlDataAdapter da = new SqlDataAdapter(str, koneksi);
            DataSet ds = new DataSet();
            da.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            koneksi.Close();
        }
        private void cbNama()
        {
            koneksi.Open();
            string str = "SELECT nama_mahasiswa FROM Mahasiswa";
            SqlCommand cmd = new SqlCommand(str, koneksi);
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                cbxnama.Items.Add(dr.GetString(0));
            }
            dr.Close();
            koneksi.Close();
        }
        private void cbTahunMasuk()
        {
            int y = DateTime.Now.Year - 2010;
            string[] type = new string[y];
            int i = 0;
            for (i = 0; i < type.Length; i++)
            {
                if (i == 0)
                {
                    cbxTahunMasuk.Items.Add("2010");
                }
                else
                {
                    int l = 2010 + i;
                    cbxTahunMasuk.Items.Add(l.ToString());
                }
            }
        }

        private void cbxnama_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedName = cbxnama.SelectedItem.ToString();


            koneksi.Open();
            string str = "SELECT nim FROM mahasiswa WHERE nama_mahasiswa = @nama_mahasiswa";
            SqlCommand cm = new SqlCommand(str, koneksi);
            cm.Parameters.AddWithValue("@nama_mahasiswa", selectedName);
            SqlDataReader dr = cm.ExecuteReader();

            if (dr.Read())
            {
                string nim = dr.GetString(0);
                txtNIM.Text = nim;
            }
            else
            {
                txtNIM.Text = "";
            }

            dr.Close();
            koneksi.Close();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            dataGridView();
            btnOpen.Enabled = false;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            cbxTahunMasuk.Enabled = true;
            cbxnama.Enabled = true;
            cbxStatusMahasiswa.Enabled = true;
            txtNIM.Visible = true;
            cbTahunMasuk();
            cbNama();
            btnClear.Enabled = true;
            btnSave.Enabled = true;
            btnAdd.Enabled = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string nim = txtNIM.Text;
            string statusMahasiswa = cbxStatusMahasiswa.Text;
            string tahunMasuk = cbxTahunMasuk.Text;
            int count = 0;
            string tempKodeStatus = "";
            string kodeStatus = "";
            koneksi.Open();

            string str = "select count (*) from dbo.status_mahasiswa";
            SqlCommand cm = new SqlCommand(str, koneksi);
            count = (int)cm.ExecuteScalar();
            if (count == 0)
            {
                kodeStatus = "1";
            }
            else
            {
                string querryString = "SELECT TOP 1 id_status FROM dbo.status_mahasiswa ORDER BY id_status DESC";
                cm = new SqlCommand(querryString, koneksi);
                SqlCommand cmStatusMahasiswa = new SqlCommand(querryString, koneksi);
                SqlDataReader dr = cm.ExecuteReader();
                if (dr.Read())
                {
                    tempKodeStatus = dr.GetString(0);
                }
                dr.Close();
                int tempKode = int.Parse(tempKodeStatus);
                tempKode++;
                kodeStatus = tempKode.ToString();
            }
            string queryString = "INSERT INTO dbo.status_mahasiswa (id_status, nim, status_mahasiswa, tahun_masuk) VALUES (@id_status, @nim, @status_mahasiswa, @tahun_masuk)";
            SqlCommand cmd = new SqlCommand(queryString, koneksi);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("id_status", kodeStatus));
            cmd.Parameters.Add(new SqlParameter("NIM", nim));
            cmd.Parameters.Add(new SqlParameter("status_mahasiswa", statusMahasiswa));
            cmd.Parameters.Add(new SqlParameter("tahun_masuk", tahunMasuk));
            cmd.ExecuteNonQuery();
            koneksi.Close();
            MessageBox.Show("Data berhasil disimpan", "Sukses!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            refreshform();
            dataGridView();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            refreshform();
        }
        private void Halaman_Data_Status_Mahasiswa_FormClosed(object sender, FormClosedEventArgs e)
        {
            Form1 fm = new Form1();
            fm.Show();
            this.Hide();
        }
    }
}
