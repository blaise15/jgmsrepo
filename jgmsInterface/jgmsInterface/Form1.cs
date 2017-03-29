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
using System.Collections;
using System.Net;
using System.Net.Mail;

namespace jgmsInterface
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection connection = new MySqlConnection("server=localhost;database=jgms;uid=root;pwd=root;");
        int originalVoucher;
        ArrayList signUpScheduleID = new ArrayList();
        ArrayList signUpScheduleIDAlt = new ArrayList();

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
        //
        //Transactions Group
        //
        #region
        //This region contains the methods that prevent the user from inputting
        //letters into textboxes that only need numeric values.
        #region
        private void txtbxNewClassCheckNum_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxNewMbrshpCheckNum_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxNewMbrshpPersonID_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxNewMbrshpMemberID_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxNewClassVoucher_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
        private void txtBxOthPrson1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxOthPrson2_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxOthPrson3_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void txtBxOthPrson4_KeyPress(object sender, KeyPressEventArgs e)
        {

        }
        #endregion
        //This will insert the data in when people sign up for classes. It will
        //insert the data into the class roster table.
        private void NewClassSvTrnsctn_Click(object sender, EventArgs e)
        {

        }
        //This will insert the data for the new membership transaction when the button is pressed.
        private void NewMembershipSvTrnsctn_Click(object sender, EventArgs e)
        {

        }
        //Method to update the expiration date of the membership.
        private void UpdateMembership(string personID)
        {
            connection.Open();
            string queryUpdateExpDate = "UPDATE membership JOIN person on person.MembershipID=membership.membershipID SET expirationDate = curdate() + INTERVAL 1 YEAR WHERE personID =  " + personID + ";";
            MySqlCommand qryUpdateExpDate = new MySqlCommand(queryUpdateExpDate, connection);
            MySqlDataReader myReader = qryUpdateExpDate.ExecuteReader();
            myReader.Close();
            connection.Close();

        }
        //This will automatically fill the textboxes with the first name and the last
        //name of the individual whos personID corresponds with the inserted one.
        private void txtbxNewClassPersonID_Leave(object sender, EventArgs e)
        {

        }
        //This will fill the listboxes up with the members' personID and first and last name.
        private void Form1_Load(object sender, EventArgs e)
        {

            string queryPersonIDName = "Select personID, firstName, lastName FROM person ORDER by lastName;";
            originalVoucher = int.Parse(txtBxNewClassVoucher.Text);

            FillCheckBox();
            try
            {
                connection.Open();
                MySqlCommand queryPrsnIDName = new MySqlCommand(queryPersonIDName, connection);
                MySqlDataReader myReader;
                //Filling the datagridviews that contain the PersonIDs and Names.
                myReader = queryPrsnIDName.ExecuteReader();
                int i = 0;
                while (myReader.Read())
                {
                    dataGridViewClassTransaction.Rows.Add();
                    dataGridViewClassTransaction.Rows[i].Cells[0].Value = myReader.GetString(0);
                    dataGridViewClassTransaction.Rows[i].Cells[1].Value = myReader.GetString(2) + ", " + myReader.GetString(1);
                    dataGridViewMembership.Rows.Add();
                    dataGridViewMembership.Rows[i].Cells[0].Value = myReader.GetString(0);
                    dataGridViewMembership.Rows[i].Cells[1].Value = myReader.GetString(2) + ", " + myReader.GetString(1);
                    i++;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //This will fill in the auto fill boxes (membershipID, firstName, lastName,
        //membershipType on the New Membership Transaction Tab.
        private void txtBxNewMbrshpPersonID_Leave(object sender, EventArgs e)
        {

        }
        //This method will fill the checkbox with the most up to date information.
        public string[] FillCheckBox()
        {
            string[] classNameArray = new string[30];
            DateTime dt = DateTime.Now;
            string queryClassName = "select scheduleID, sDate, className, enrollment, capacity from schedule JOIN class ON" +
                                    " schedule.class_classID = class.classID where sDate " +
                                    "BETWEEN CURDATE() AND (CURDATE() + INTERVAL 35 DAY) ORDER BY Sdate;";
            string className;

            chkLstBxClassNames.Items.Clear();
            try
            {
                int i = 0;
                //This will query the class table and retrieve the class names that correspond to
                //to the classes that are occuring for that month.
                connection.Open();
                MySqlCommand queryClass = new MySqlCommand(queryClassName, connection);
                MySqlDataReader myReader;
                myReader = queryClass.ExecuteReader();
                while (myReader.Read())
                {

                    className = (myReader.GetString(0) + " " + myReader.GetString(1) + " " + myReader.GetString(2) + " " + myReader.GetString(3) + "/" + myReader.GetString(4)); ;
                    chkLstBxClassNames.Items.Add(className);
                    classNameArray[i] = myReader.GetString(2);
                }
                myReader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return classNameArray;
        }
        //Determines the cost of the class.
        public double DetermineClassCost(ArrayList list, int scheduleID)
        {
            double classCost = 0;
            double hours = 0;
            double costPerHour = 0;
            double consumableCost = 0;

            //Query to find the hours and cost per hour.
            string queryHoursCost = "SELECT hours, costPerHour FROM class_cost WHERE scheduleID = " + list[scheduleID] + ";";
            string queryConsumableCost = "SELECT cost FROM consumables JOIN schedule ON schedule.consumableType = consumables.ConsumableType WHERE scheduleID = "
                                        + list[scheduleID] + ";";
            try
            {
                connection.Open();
                MySqlCommand qryHoursCost = new MySqlCommand(queryHoursCost, connection);
                MySqlCommand qryConsumableCost = new MySqlCommand(queryConsumableCost, connection);
                MySqlDataReader myCostReader;
                //Executing the query to hours and cost per hour.
                myCostReader = qryHoursCost.ExecuteReader();
                while (myCostReader.Read())
                {
                    hours = myCostReader.GetDouble(0);
                    costPerHour = myCostReader.GetDouble(1);
                }
                myCostReader.Close();
                //Quderying the cost of the consumables for the class.
                myCostReader = qryConsumableCost.ExecuteReader();
                while (myCostReader.Read())
                {
                    consumableCost = myCostReader.GetDouble(0);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            //Adding the cost of the class.
            classCost = (hours * costPerHour) + consumableCost;
            //Adding the cost of the consumables.
            return classCost;
        }
        //This will update the cost each time a class is checked for enrollment.
        private void chkLstBxClassNames_ItemCheck(object sender, ItemCheckEventArgs e)
        {

        }
        //This will deduct the cost of the voucher from the overall cost of the class registration. This is not the best way to do it.
        private void txtBxNewClassVoucher_Leave(object sender, EventArgs e)
        {

        }
        //This will make it so the textboxes and labels become visible so that other members on the account can have their personIDs inserted.
        private void cmbBoxOthMbrsOnAcct_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //Sets the cost textbox for membership transaction to the appropriate cost dependent on membership type.
        private void txtBxNewMbrshpMbrshpType_TextChanged(object sender, EventArgs e)
        {

        }
        //Sets the cost text box for membership transaction to the appropriate cost dependent on the amount of members in the family.
        private void cmbBoxOthMbrsOnAcct_SelectedValueChanged(object sender, EventArgs e)
        {

        }
        //This will enable the check number box so the check number can be inserted.
        private void cmbBxNewMbrshpPayment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //This will enable the check number box so the check number can be inserted.
        private void cmbBxNewClassPayment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        //This will create the string for the class receipt to be sent as an email.
        private string CreateClassReceiptEmail()
        {
            string emailString = "";

            emailString += txtbxNewClassFName.Text + " " + txtbxNewClassLName.Text + ",\n\tThank you for enrolling in classes this month.\n" +
                            "You have been enrolled in the following courses:\n\n";

            try
            {
                if (signUpScheduleID.Count > 0)
                {
                    for (int i = 0; i < signUpScheduleID.Count; i++)
                    {
                        connection.Open();
                        string queryClassNameDate = "SELECT className,sdate  FROM class JOIN schedule ON schedule.class_classID = class.classID where scheduleID = " + signUpScheduleID[i] + ";";
                        MySqlCommand qryClassName = new MySqlCommand(queryClassNameDate, connection);
                        MySqlDataReader myReader = qryClassName.ExecuteReader();
                        while (myReader.Read())
                        {
                            emailString += myReader.GetString(0) + " " + myReader.GetString(1) + "\n";
                        }
                        connection.Close();
                    }
                }
                if (txtBxNewClassVoucher.Text != "0")
                {
                    emailString += "\nVoucher Charge: $" + txtBxNewClassVoucher.Text;
                }
                emailString += "\nTotal charge: $" + txtbxNewClassCost.Text;
                if (signUpScheduleIDAlt.Count > 0)
                {
                    emailString += "\n\nYou have been enrolled as an alternate in the following courses:\n\n";
                    for (int i = 0; i < signUpScheduleIDAlt.Count; i++)
                    {
                        connection.Open();
                        string queryClassNameDate = "SELECT className,sdate  FROM class JOIN schedule ON schedule.class_classID = class.classID where scheduleID = " + signUpScheduleIDAlt[i] + ";";
                        MySqlCommand qryClassName = new MySqlCommand(queryClassNameDate, connection);
                        MySqlDataReader myReader = qryClassName.ExecuteReader();
                        while (myReader.Read())
                        {
                            emailString += myReader.GetString(0) + " " + myReader.GetString(1) + "\n";
                        }
                        connection.Close();
                        emailString += "An instructor will contact you if an open slot arises for " +
                                      "the courses you are an alternate for.";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            emailString += "\n\nThank you for enrolling in the courses for this month at JGMS." +
                            "\nWe are excited to see you at the upcoming classes." +
                            "\nPlease don't forget class sign ups are on the first" +
                            "\nThursday of every month." +
                            "\n\n\nThank you!\nJacksonville Gem and Mineral Society";
            signUpScheduleID.Clear();
            signUpScheduleIDAlt.Clear();
            return emailString;
        }
        //This will create the string for membership receipt to be sent as an email.
        private string CreateMembershipReceiptEmail()
        {            
            string emailReceipt = "";

            emailReceipt += txtBxNewMbrshpFName.Text + " " + txtBxNewMbrshpLName.Text + ",\n\nThank you for updating your membership"
                + "with JGMS. The following are the new membership\nexpiration dates for the memberships you updated today:\n\n";
            emailReceipt += FindNameAndExpirationDate(txtBxNewMbrshpPersonID.Text);
            if (txtBxOthPrson2.Visible == true)
            {
                emailReceipt += "\n" + FindNameAndExpirationDate(txtBxOthPrson2.Text);
            }
            if (txtBxOthPrson3.Visible == true)
            {
                emailReceipt += "\n" + FindNameAndExpirationDate(txtBxOthPrson3.Text);
            }
            if (txtBxOthPrson4.Visible == true)
            {
                emailReceipt += "\n" + FindNameAndExpirationDate(txtBxOthPrson4.Text);
            }
            emailReceipt += "\n\nTotal= $" + txtBxNewMbrshpCost.Text;
            emailReceipt += "\n\nThank you for renewing your memberhsip.";
            emailReceipt += "\n\n\nThank you!\nJacksonville Gem and Mineral Society";
            return emailReceipt;
        }
        //This method will send the email to the member who signed up for classes and 
        public void SendEmail(string receipt, string personID)
        {
            string queryEmail = "Select email FROM person WHERE personID = " + personID + ";";
            try
            {
                string email = "";
                //Getting the email address of the member.

                connection.Open();
                MySqlCommand qryEmail = new MySqlCommand(queryEmail, connection);
                MySqlDataReader myReader = qryEmail.ExecuteReader();
                while (myReader.Read())
                {
                    email = myReader.GetString(0);
                }
                connection.Close();
                SmtpClient client = new SmtpClient("smtp.gmail.com");
                client.Port = 587;
                client.EnableSsl = true;
                client.Timeout = 100000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("JaxGemAndMineralSociety@gmail.com",
                                    "CS3662017");
                MailMessage msg = new MailMessage();
                msg.To.Add(email);
                msg.From = new MailAddress("JaxGemAndMineralSociety@gmail.com", "JGMS");
                msg.Subject = "Class Receipt";
                msg.Body = receipt;
                client.Send(msg);
                MessageBox.Show("Receipt sent.");
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        //This method will check to make sure that the requiremments have been met for the user to enroll in a class.
        public bool CheckMembershipValid(string personID)
        {
            bool flag = true;
            DateTime today = DateTime.Now;
            DateTime expDate = new DateTime();
            string queryExpDate = "SELECT expirationDate FROM membership JOIN person ON person.membershipID = membership.membershipID WHERE person.personID = " + personID + ";";
            try
            {
                //Querying the expiration date of the member's membership.
                connection.Open();
                MySqlCommand qryExpDate = new MySqlCommand(queryExpDate, connection);
                MySqlDataReader myReader = qryExpDate.ExecuteReader();
                while (myReader.Read())
                {
                    expDate = myReader.GetDateTime(0);
                }
                connection.Close();
                //Testing to see if the expiration date of the member is expired.
                //If the date is less than today then it will show that it is expired and return false.
                int result = DateTime.Compare(expDate, today);
                if (result <= 0)
                {
                    flag = false;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }
            return flag;
        }
        //Method that will return the members name and membership expiration date.
        public string FindNameAndExpirationDate(string personID)
        {

            string nameAndExpDate;
            string name = " ";
            string qryExp = " ";
            string queryNameAndExp = "SELECT firstName, lastName, expirationDate from person JOIN membership ON person.MembershipID = membership.MembershipID WHERE personID = "
                                    + personID + ";";
            try
            {
                connection.Open();
                MySqlCommand qryNameAndExp = new MySqlCommand(queryNameAndExp, connection);
                MySqlDataReader myReader = qryNameAndExp.ExecuteReader();
                while (myReader.Read())
                {
                    name = myReader.GetString(0) + " " + myReader.GetString(1);
                    qryExp = myReader.GetString(2);
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            string[] temp = qryExp.Split('/');
            string tempYear = temp[2];
            string[] temp2 = tempYear.Split(' ');
            nameAndExpDate = name + ":  " + temp[0] + "/" + temp2[0];
            return nameAndExpDate;

        }
        //This region contains the methods that will display the members' name when the 
        //personID is inserted into the other personID.
        #region
        //Displaying the name of the personID displayed in other person1.
        private void txtBxOthPrson1_TextChanged(object sender, EventArgs e)
        {

        }
        //Displaying the name of the personID displayed in other person2.
        private void txtBxOthPrson2_TextChanged(object sender, EventArgs e)
        {

        }
        //Displaying the name of the personID displayed in other person3
        private void txtBxOthPrson3_TextChanged(object sender, EventArgs e)
        {

        }
        //Displaying the name of the personID displayed in other person4.
        private void txtBxOthPrson4_TextChanged(object sender, EventArgs e)
        {

        }
        #endregion
        //Method that will determine the members name. Used when filling in the labels for other users on the membership tab.
        private string DetermineUserName(string personID)
        {
            string name = "";
            string queryName = "SELECT firstName, lastName FROM person WHERE personID = " + personID + ";";
            try
            {
                connection.Open();
                MySqlCommand qryName = new MySqlCommand(queryName, connection);
                MySqlDataReader myReader = qryName.ExecuteReader();
                while (myReader.Read())
                {
                    name = myReader.GetString(0) + " " + myReader.GetString(1);
                }
                myReader.Close();
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            return name;
        }
#endregion
    }
}
