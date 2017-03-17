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

using System.Data.SqlClient;



namespace jgmsInterface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonSaveClass_Click(object sender, EventArgs e)
        {
            try
            {
                //This is my connection string i have assigned the database file address path  
                string MyConnection2 = "Server=localhost;Database=jgms;Uid=root;Pwd=jack;";
                //This is my insert query in which i am taking input from the user through windows forms  
                string Query = "insert into jgms.class(className,ClassID,PreReq,description) values('" + this.classNameTxtbox.Text + "','" + this.classIDTxtBox.Text + "','" + this.preRequisitesTxtBox.Text + "','" + this.descriptionTxtBox.Text + "');";
                //This is  MySqlConnection here i have created the object and pass my connection string.  
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                //This is command class which will handle the query and connection object.  
                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                MySqlDataReader MyReader2;
                MyConn2.Open();
                MyReader2 = MyCommand2.ExecuteReader();     // Here our query will be executed and data saved into the database.  
                MessageBox.Show("Save Data");
                while (MyReader2.Read())

                {
                }
                MyConn2.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void classReportButton_Click(object sender, EventArgs e)
        {

            try
            {
                string MyConnection2 = "Server=localhost;Database=jgms;Uid=root;Pwd=root;";// change to jgms DB, was testing on dummy DB
                //Display query  
                string Query = "SELECT classID,className,description,prereq FROM class";// change fields and table, currently displays info from example DB to ensure i was getting connection to MySql
                
                MySqlConnection MyConn2 = new MySqlConnection(MyConnection2);
                MySqlCommand MyCommand2 = new MySqlCommand(Query, MyConn2);
                 MyConn2.Open();  
                //For offline connection we weill use  MySqlDataAdapter class.  
                MySqlDataAdapter MyAdapter = new MySqlDataAdapter();
                MyAdapter.SelectCommand = MyCommand2;
                DataTable dTable = new DataTable();
                MyAdapter.Fill(dTable);
                classReportDataGridView.DataSource = dTable; // here i have assign dTable object to the classReportDataGridView object to display data.               
                MyConn2.Close();  
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
