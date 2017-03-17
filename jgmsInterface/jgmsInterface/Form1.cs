using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace jgmsInterface
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection connection = new MySqlConnection("server=localhost;database=jgms;uid=root;pwd=root;");

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();
                string classOption = "select * from class;";
                MySqlDataAdapter Ada = new MySqlDataAdapter(classOption, connection);
                DataTable dt = new DataTable();
                DataSet classInfo = new System.Data.DataSet();
                Ada.Fill(classInfo, "class");
               
                dataGrid_digsReport.DataSource = classInfo.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }

        private void buttonSaveClass_Click(object sender, EventArgs e)
        {
            string newClassQuery = "insert into class(classId, className, description, prereq) values("
                + txtbox_NewClass_ClassID.Text + ", '" + txtbox_NewClass_ClassName.Text + "', '" + txtbox_NewClass_Description.Text 
                + "', '" + txtbox_NewClass_PreReq.Text + "');";

            try
            {

                connection.Open();
                MySqlCommand newClass = new MySqlCommand(newClassQuery, connection);
                MySqlDataReader rdr = newClass.ExecuteReader();
                rdr.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
