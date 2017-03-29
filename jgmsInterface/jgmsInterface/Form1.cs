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


        private void tabControl3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl3.SelectedTab == tabControl3.TabPages["classReportTabPage"])//your specific tabname
            {
                try
                {
                    connection.Open();
                    string classOption = "select * from class;";
                    MySqlDataAdapter Ada = new MySqlDataAdapter(classOption, connection);
                    DataTable dt = new DataTable();
                    DataSet classInfo = new System.Data.DataSet();
                    Ada.Fill(classInfo);

                    dataGrid_classReport.DataSource = classInfo.Tables[0];
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

            if (tabControl3.SelectedTab == tabControl3.TabPages["digsReportTabPage"])//your specific tabname
            {
                try
                {
                    connection.Open();
                    string personOption = "select lastName, firstName, phone, email from person;";
                    MySqlDataAdapter Ada = new MySqlDataAdapter(personOption, connection);
                    DataTable dt = new DataTable();
                    DataSet personInfo = new System.Data.DataSet();
                    Ada.Fill(personInfo, "person");

                    dataGrid_digsReport.DataSource = personInfo.Tables[0];
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

            if (tabControl3.SelectedTab == tabControl3.TabPages["scheduleReportTabPage"])//your specific tabname
            {
                try
                {
                    connection.Open();
                    string scheduleOption = "select s.Sdate, c.className, p.lastName, p.firstName, co.cost, " +
                        "cc.hours, cc.costPerHour, (cost + costPerHour) " +
                        "from schedule as s, class as c, person as p, consumables as co, " +
                        "class_cost as cc, instructor as i " +
                        "where s.instructorID = i.instructorID AND " +
                        "i.personID = p.personID AND " +
                        "s.consumableType = co.consumableType AND " +
                        "s.class_classID = c.classID AND " +
                        "s.scheduleID = cc.scheduleID;";
                    MySqlDataAdapter Ada = new MySqlDataAdapter(scheduleOption, connection);
                    DataTable dt = new DataTable();
                    DataSet scheduleInfo = new System.Data.DataSet();
                    Ada.Fill(scheduleInfo);

                    dataGrid_scheduleReport.DataSource = scheduleInfo.Tables[0];
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

            if (tabControl3.SelectedTab == tabControl3.TabPages["tabname"])//your specific tabname
            {
                // your stuff
            }

            if (tabControl3.SelectedTab == tabControl3.TabPages["instructorReportTabPage"])//your specific tabname
            {
                try
                {
                    connection.Open();
                    string instructorOption = "select p.lastName, p.firstName, p.email, p.phone, i.active from " +
                                              "person as p, instructor as i where p.personID = i.personID; ";
                    MySqlDataAdapter Ada = new MySqlDataAdapter(instructorOption, connection);
                    DataTable dt = new DataTable();
                    DataSet instructorInfo = new System.Data.DataSet();
                    Ada.Fill(instructorInfo, "instructor");

                    dataGrid_instructorReport.DataSource = instructorInfo.Tables[0];
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
}
