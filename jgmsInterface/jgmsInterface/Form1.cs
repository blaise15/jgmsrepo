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
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxNewMbrshpCheckNum_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxNewMbrshpPersonID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxNewMbrshpMemberID_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxNewClassVoucher_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
        private void txtBxOthPrson1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxOthPrson2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxOthPrson3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void txtBxOthPrson4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }
        #endregion
        //This will insert the data in when people sign up for classes. It will
        //insert the data into the class roster table.
        private void NewClassSvTrnsctn_Click(object sender, EventArgs e)
        {
            //The boolean expValid will be used to determine whether to send an email out later or not.
            //If expVaid is true then it will send out an email, if false it will not.
            bool expValid = true;
            try
            {
                connection.Open();
                //Assigning a value to expValid whether or not the enrollment is still valid.
                expValid = CheckMembershipValid(txtbxNewClassPersonID.Text);
                if (expValid == true)
                {
                    try
                    {
                        //Boolean variable that will let us know whether the box is checked.
                        bool checkVal;
                        bool alternate = false;
                        //Looping throught the checked indices for the classes that are being selected to sign up for.
                        for (int i = 0; i < chkLstBxClassNames.Items.Count; i++)
                        {
                            //Checking to see if the checkbox is checked.
                            checkVal = chkLstBxClassNames.GetItemChecked(i);
                            if (checkVal == true)
                            {
                                string temp = (string)chkLstBxClassNames.Items[i];
                                string[] scheduleID = temp.Split(' ');
                                //Queries that will determine enrollment and capacity to see if the user is going to be an alternate.
                                string queryEnrollment = "SELECT enrollment FROM schedule WHERE scheduleID = " + scheduleID[0] + ";";
                                string queryCapacity = "SELECT capacity FROM schedule WHERE scheduleID = " + scheduleID[0] + ";";
                                int enrollment = 0;
                                int capacity = 0;
                                //Querying the amount already enrolled.
                                MySqlCommand qryEnrollment = new MySqlCommand(queryEnrollment, connection);
                                MySqlDataReader myReader2 = qryEnrollment.ExecuteReader();
                                if (myReader2.Read())
                                {
                                    enrollment = myReader2.GetInt32(0);
                                }
                                myReader2.Close();
                                //Querying the capacity.
                                MySqlCommand qryCapacity = new MySqlCommand(queryCapacity, connection);
                                myReader2 = qryCapacity.ExecuteReader();
                                if (myReader2.Read())
                                {
                                    capacity = myReader2.GetInt32(0);
                                }
                                myReader2.Close();
                                //Testing to see whether enrollment is greater than capacity. 
                                if (enrollment >= capacity)
                                {
                                    alternate = true;
                                    signUpScheduleIDAlt.Add(scheduleID[0]);
                                }
                                //Adding the class names and dates to the arrays for classes they are signed up for.
                                else
                                {
                                    signUpScheduleID.Add(scheduleID[0]);

                                }
                                string queryFillRoster = "INSERT INTO class_roster (scheduleID, personID, alternates) VALUES ( " + scheduleID[0] + "," + txtbxNewClassPersonID.Text +
                                                            "," + alternate + ");";
                                string queryUpdateEnrollment = "UPDATE schedule SET enrollment = enrollment + 1 WHERE scheduleID = " + scheduleID[0] + ";";
                                //Inserting the users information into the class_roster table.
                                MySqlCommand qryFillRoster = new MySqlCommand(queryFillRoster, connection);
                                MySqlDataReader myReader4 = qryFillRoster.ExecuteReader();
                                myReader4.Close();
                                //Updating the enrollment column in the schedule table.
                                MySqlCommand qryUpdateEnrollment = new MySqlCommand(queryUpdateEnrollment, connection);
                                MySqlDataReader myReader5 = qryUpdateEnrollment.ExecuteReader();
                                myReader5.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    string receiptString = CreateClassReceiptEmail();
                    MessageBox.Show(receiptString);
                    SendEmail(receiptString, txtbxNewClassPersonID.Text);
                    //Inserting the data into the payment table.
                    #region
                    try
                    {
                        string paymentMethod;
                        if (cmbBxNewClassPayment.SelectedIndex == 0)
                        {
                            paymentMethod = "Cash";
                        }
                        else if (cmbBxNewClassPayment.SelectedIndex == 1)
                        {
                            paymentMethod = " Square";
                        }
                        else
                        {
                            paymentMethod = "Check";
                        }
                        string queryFillPaymentNoCheck = "INSERT INTO payment(personID, amount, datePaid, paymentType, feeType) VALUES (" + txtbxNewClassPersonID.Text +
                                                          ", " + txtbxNewClassCost.Text + " , NOW(),\"" + paymentMethod + "\", \"Class\");";
                        string queryFillPaymentCheck = "INSERT INTO payment(personID, amount, datePaid, paymentType, checkNumber, feeType) VALUES (" + txtbxNewClassPersonID.Text +
                                                    ", " + txtbxNewClassCost.Text + " , NOW(),\"" + paymentMethod + "\", " + txtbxNewClassCheckNum.Text + ",\"Class\");";
                        if (cmbBxNewClassPayment.SelectedIndex != 2)
                        {
                            MySqlCommand qryFillPaymentNoCheck = new MySqlCommand(queryFillPaymentNoCheck, connection);
                            MySqlDataReader myReader6 = qryFillPaymentNoCheck.ExecuteReader();
                            myReader6.Close();
                        }
                        else
                        {
                            MySqlCommand qryFillPaymentCheck = new MySqlCommand(queryFillPaymentCheck, connection);
                            MySqlDataReader myReader7 = qryFillPaymentCheck.ExecuteReader();
                            myReader7.Close();
                        }
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            #endregion
            //Reupdating the class enrolled values in the checkbox
            FillCheckBox();
            //Clearing the text in the textboxes.
            txtbxNewClassCost.Text = "0";
            txtBxNewClassVoucher.Text = "0";
            txtbxNewClassCheckNum.Clear();
            txtbxNewClassFName.Clear();
            txtbxNewClassLName.Clear();
            txtbxNewClassPersonID.Clear();
            txtbxNewClassCheckNum.Enabled = false;
            cmbBxNewClassPayment.SelectedIndex = -1;
            //Unchecking the boxes in the checkedlist box.
            for (int i = 0; i < chkLstBxClassNames.Items.Count; i++)
            {
                chkLstBxClassNames.SetItemChecked(i, false);
            }
        }
        //This will insert the data for the new membership transaction when the button is pressed.
        private void NewMembershipSvTrnsctn_Click(object sender, EventArgs e)
        {
            string membershipReceipt;
            //Ensuring that a valid member has been selected.
            if (txtBxNewMbrshpMemberID.Text == "")
            {
                MessageBox.Show("Please insert a valid person ID.");
            }
            //Ensuring that a payment method has been selected. 
            else if (cmbBxNewMbrshpPayment.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a payment type.");
            }
            else
            {
                try
                {
                    connection.Open();
                    //Determining the payment method selected.
                    string paymentMethod;
                    if (cmbBxNewMbrshpPayment.SelectedIndex == 0)
                    {
                        paymentMethod = "Cash";
                    }
                    else if (cmbBxNewMbrshpPayment.SelectedIndex == 1)
                    {
                        paymentMethod = " Square";
                    }
                    else
                    {
                        paymentMethod = "Check";
                    }
                    //Strings that are used for the queries to insert the payment into the database.
                    string queryFillPaymentNoCheck = "INSERT INTO payment(personID, amount, datePaid, paymentType, feeType) VALUES (" + txtBxNewMbrshpPersonID.Text +
                                  ", " + txtBxNewMbrshpCost.Text + " , NOW(),\"" + paymentMethod + "\", \"Membership\");";
                    string queryFillPaymentCheck = "INSERT INTO payment(personID, amount, datePaid, paymentType, checkNumber, feeType) VALUES (" + txtBxNewMbrshpPersonID.Text +
                                                ", " + txtBxNewMbrshpCost.Text + " , NOW(),\"" + paymentMethod + "\", " + txtBxNewMbrshpCheckNum.Text + ",\"Membership\");";
                    //Processing a payment for cash and square.
                    if (cmbBxNewMbrshpPayment.SelectedIndex != 2)
                    {
                        MySqlCommand qryFillPaymentNoCheck = new MySqlCommand(queryFillPaymentNoCheck, connection);
                        MySqlDataReader myReader6 = qryFillPaymentNoCheck.ExecuteReader();
                        myReader6.Close();
                    }
                    //Processing a payment for check.
                    else
                    {
                        MySqlCommand qryFillPaymentCheck = new MySqlCommand(queryFillPaymentCheck, connection);
                        MySqlDataReader myReader7 = qryFillPaymentCheck.ExecuteReader();
                        myReader7.Close();
                    }
                    connection.Close();
                    //Updating the membership expiration date for the members.
                    //The if statements are for when you are updating family members.
                    UpdateMembership(txtBxNewMbrshpPersonID.Text);
                    if (txtBxOthPrson1.Visible == true)
                    {
                        UpdateMembership(txtBxOthPrson1.Text);
                    }
                    if (txtBxOthPrson2.Visible == true)
                    {
                        UpdateMembership(txtBxOthPrson2.Text);
                    }
                    if (txtBxOthPrson3.Visible == true)
                    {
                        UpdateMembership(txtBxOthPrson3.Text);
                    }
                    if (txtBxOthPrson4.Visible == true)
                    {
                        UpdateMembership(txtBxOthPrson4.Text);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                membershipReceipt = CreateMembershipReceiptEmail();
                MessageBox.Show(membershipReceipt);
                SendEmail(membershipReceipt, txtBxNewMbrshpPersonID.Text);

            }
            //Clears out the values inserted into the textbox. 
            txtBxNewMbrshpCheckNum.Clear();
            txtBxNewMbrshpCost.Clear();
            txtBxNewMbrshpFName.Clear();
            txtBxNewMbrshpLName.Clear();
            txtBxNewMbrshpMbrshpType.Clear();
            txtBxNewMbrshpMemberID.Clear();
            txtBxNewMbrshpPersonID.Clear();
            txtBxOthPrson1.Clear();
            txtBxOthPrson2.Clear();
            txtBxOthPrson3.Clear();
            txtBxOthPrson4.Clear();
            txtBxNewMbrshpCheckNum.Enabled = false;
            cmbBxNewMbrshpPayment.SelectedIndex = -1;
            txtBxOthPrson1.Visible = false;
            lblOthMbr1.Visible = false;
            txtBxOthPrson2.Visible = false;
            lblOthMbr2.Visible = false;
            txtBxOthPrson3.Visible = false;
            lblOthMbr3.Visible = false;
            txtBxOthPrson4.Visible = false;
            lblOthMbr4.Visible = false;
            cmbBoxOthMbrsOnAcct.Enabled = false;
            cmbBoxOthMbrsOnAcct.SelectedIndex = -1;

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
            //Query that will get the first name corresponding to the personID.
            string queryFName = "Select firstName from person where personID = " + txtbxNewClassPersonID.Text + ";";
            //Query that will get the last name corresponding to the personID.
            string queryLName = "Select lastName from person where personID = " + txtbxNewClassPersonID.Text + ";";

            if (txtbxNewClassPersonID.Text != "")
            {
                try
                {
                    connection.Open();
                    //Querying the first name of the individual.               
                    MySqlCommand queryFirstName = new MySqlCommand(queryFName, connection);
                    txtbxNewClassFName.Text = ((string)queryFirstName.ExecuteScalar()); ;
                    //Querying the last name of the individual
                    MySqlCommand queryLastName = new MySqlCommand(queryLName, connection);
                    txtbxNewClassLName.Text = ((string)queryLastName.ExecuteScalar());
                    connection.Close();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
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

            string queryFName = "Select firstName from person where personID = " + txtBxNewMbrshpPersonID.Text + ";";
            string queryLName = "Select lastName from person where personID = " + txtBxNewMbrshpPersonID.Text + ";";
            string queryMemberId = "Select membershipID from person where personID = " + txtBxNewMbrshpPersonID.Text + ";";
            int temp;

            if (txtBxNewMbrshpPersonID.Text != "")
            {
                try
                {
                    connection.Open();
                    //Querying the first name of the individual.
                    MySqlCommand queryFirstName = new MySqlCommand(queryFName, connection);
                    txtBxNewMbrshpFName.Text = (string)queryFirstName.ExecuteScalar();
                    //Querying the last name of the individual.
                    MySqlCommand queryLastName = new MySqlCommand(queryLName, connection);
                    txtBxNewMbrshpLName.Text = (string)queryLastName.ExecuteScalar();
                    //Querying the membershipID of the individual.
                    MySqlCommand queryMbrID = new MySqlCommand(queryMemberId, connection);
                    temp = (int)queryMbrID.ExecuteScalar();
                    txtBxNewMbrshpMemberID.Text = temp.ToString();
                    //Querying the membershipType of the individual
                    string queryMbrshpType = "Select membershipType from membership where membershipID = " + txtBxNewMbrshpMemberID.Text + ";";
                    MySqlCommand queryMbrType = new MySqlCommand(queryMbrshpType, connection);
                    txtBxNewMbrshpMbrshpType.Text = (string)queryMbrType.ExecuteScalar();
                    connection.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                if (txtBxNewMbrshpMbrshpType.Text == "Family")
                {
                    cmbBoxOthMbrsOnAcct.Enabled = true;
                }
            }

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
            double classCost = 0;
            double totalCost = int.Parse(txtbxNewClassCost.Text);
            int enrollment = 0;
            int capacity = 0;
            int i = e.Index;
            //ArrayList that will hold the scheduleIDs
            ArrayList scheduleIDs = new ArrayList();

            //This region fills the arraylist with the scheduleIDs so we can reference them to determine the cost.
            #region            
            string querySchedID = "select scheduleID, sDate from schedule where sDate BETWEEN CURDATE() AND (CURDATE() + INTERVAL 35 DAY) order by sDate;";
            try
            {
                connection.Open();
                MySqlCommand qrySchedID = new MySqlCommand(querySchedID, connection);
                MySqlDataReader myReader;
                //Filling the array list with the scheduleIDs.
                myReader = qrySchedID.ExecuteReader();
                while (myReader.Read())
                {
                    scheduleIDs.Add(myReader.GetString(0));
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            #endregion
            //Queries that will determine enrollment and capacity to see if the user is going to be an alternate.
            string queryEnrollment = "SELECT enrollment FROM schedule WHERE scheduleID = " + scheduleIDs[i] + ";";
            string queryCapacity = "SELECT capacity FROM schedule WHERE scheduleID = " + scheduleIDs[i] + ";";

            //checkVal = chkLstBxClassNames.GetItemCheckState(i); 
            //if(checkVal==true)
            if (e.NewValue == CheckState.Checked)
            {
                connection.Open();
                //Query the enrollment value.
                MySqlCommand qryEnrollment = new MySqlCommand(queryEnrollment, connection);
                enrollment = (int)qryEnrollment.ExecuteScalar();
                //Query the capacity value.
                MySqlCommand qryCapacity = new MySqlCommand(queryCapacity, connection);
                capacity = (int)qryCapacity.ExecuteScalar();
                if (enrollment < capacity)
                {
                    //Determine the total cost of the class if enrollment is less than capacity.
                    classCost = DetermineClassCost(scheduleIDs, i);
                }
                connection.Close();

            }
            if (e.NewValue == CheckState.Unchecked)
            {
                connection.Open();
                //Query the enrollment value.
                MySqlCommand qryEnrollment = new MySqlCommand(queryEnrollment, connection);
                enrollment = (int)qryEnrollment.ExecuteScalar();
                //Query the capacity value.
                MySqlCommand qryCapacity = new MySqlCommand(queryCapacity, connection);
                capacity = (int)qryCapacity.ExecuteScalar();
                if (enrollment < capacity)
                {
                    //Determine the total cost of the class if enrollment is less than capacity.
                    classCost = DetermineClassCost(scheduleIDs, i);
                }
                //Query the enrollment value.
                MySqlCommand qryEnrollment2 = new MySqlCommand(queryEnrollment, connection);
                enrollment = (int)qryEnrollment2.ExecuteScalar();
                //Query the capacity value.
                MySqlCommand qryCapacity2 = new MySqlCommand(queryCapacity, connection);
                capacity = (int)qryCapacity2.ExecuteScalar();
                if (enrollment < capacity)
                {
                    //Determine the total cost of the class if enrollment is less than capacity.
                    classCost = -DetermineClassCost(scheduleIDs, i);
                }
                connection.Close();
            }
            //Calculating the total cost.
            totalCost += (classCost);
            txtbxNewClassCost.Text = totalCost.ToString();
        }
        //This will deduct the cost of the voucher from the overall cost of the class registration. This is not the best way to do it.
        private void txtBxNewClassVoucher_Leave(object sender, EventArgs e)
        {


            int voucherAmount = int.Parse(txtBxNewClassVoucher.Text);
            int costAmount = int.Parse(txtbxNewClassCost.Text);
            int newCostAmount = costAmount - voucherAmount + originalVoucher;
            originalVoucher = voucherAmount;
            txtbxNewClassCost.Text = newCostAmount.ToString();
        }
        //This will make it so the textboxes and labels become visible so that other members on the account can have their personIDs inserted.
        private void cmbBoxOthMbrsOnAcct_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Will display one other textbox to insert a personID.
            if (cmbBoxOthMbrsOnAcct.SelectedIndex == 0)
            {
                txtBxOthPrson1.Visible = true;
                lblOthMbr1.Visible = true;
                txtBxOthPrson2.Visible = false;
                lblOthMbr2.Visible = false;
                txtBxOthPrson3.Visible = false;
                lblOthMbr3.Visible = false;
                txtBxOthPrson4.Visible = false;
                lblOthMbr4.Visible = false;
            }
            //Will display two other textboxes to insert a personID.
            else if (cmbBoxOthMbrsOnAcct.SelectedIndex == 1)
            {
                txtBxOthPrson1.Visible = true;
                lblOthMbr1.Visible = true;
                txtBxOthPrson2.Visible = true;
                lblOthMbr2.Visible = true;
                txtBxOthPrson3.Visible = false;
                lblOthMbr3.Visible = false;
                txtBxOthPrson4.Visible = false;
                lblOthMbr4.Visible = false;
            }
            //Will display three other textboxes to insert a personID.
            else if (cmbBoxOthMbrsOnAcct.SelectedIndex == 2)
            {
                txtBxOthPrson1.Visible = true;
                lblOthMbr1.Visible = true;
                txtBxOthPrson2.Visible = true;
                lblOthMbr2.Visible = true;
                txtBxOthPrson3.Visible = true;
                lblOthMbr3.Visible = true;
                txtBxOthPrson4.Visible = false;
                lblOthMbr4.Visible = false;
            }
            //Will display four other textboxes to insert a personID.
            else if (cmbBoxOthMbrsOnAcct.SelectedIndex == 3)
            {
                txtBxOthPrson1.Visible = true;
                lblOthMbr1.Visible = true;
                txtBxOthPrson2.Visible = true;
                lblOthMbr2.Visible = true;
                txtBxOthPrson3.Visible = true;
                lblOthMbr3.Visible = true;
                txtBxOthPrson4.Visible = true;
                lblOthMbr4.Visible = true;
            }
        }
        //Sets the cost textbox for membership transaction to the appropriate cost dependent on membership type.
        private void txtBxNewMbrshpMbrshpType_TextChanged(object sender, EventArgs e)
        {
            if (txtBxNewMbrshpMbrshpType.Text == "Individual")
            {
                txtBxNewMbrshpCost.Text = "20";
            }
            else if (txtBxNewMbrshpMbrshpType.Text == "Family")
            {
                txtBxNewMbrshpCost.Text = "30";
            }
        }
        //Sets the cost text box for membership transaction to the appropriate cost dependent on the amount of members in the family.
        private void cmbBoxOthMbrsOnAcct_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cmbBoxOthMbrsOnAcct.SelectedIndex == 0)
            {
                txtBxNewMbrshpCost.Text = "40";
            }
            if (cmbBoxOthMbrsOnAcct.SelectedIndex == 1)
            {
                txtBxNewMbrshpCost.Text = "45";
            }
            if (cmbBoxOthMbrsOnAcct.SelectedIndex == 2)
            {
                txtBxNewMbrshpCost.Text = "50";
            }
            if (cmbBoxOthMbrsOnAcct.SelectedIndex == 3)
            {
                txtBxNewMbrshpCost.Text = "55";
            }
        }
        //This will enable the check number box so the check number can be inserted.
        private void cmbBxNewMbrshpPayment_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbBxNewMbrshpPayment.SelectedIndex == 2)
            {
                txtBxNewMbrshpCheckNum.Enabled = true;
            }
        }
        //This will enable the check number box so the check number can be inserted.
        private void cmbBxNewClassPayment_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cmbBxNewClassPayment.SelectedIndex == 1)
            {
                txtbxNewClassCheckNum.Enabled = false;
            }
            else if (cmbBxNewClassPayment.SelectedIndex == 2)
            {
                txtbxNewClassCheckNum.Enabled = true;
            }
            else if (cmbBxNewClassPayment.SelectedIndex == 0)
            {
                txtbxNewClassCheckNum.Enabled = false;
            }
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
            lblOtherPerson1.Visible = true;
            if (txtBxOthPrson1.Text != "")
            {
                lblOtherPerson1.Text = DetermineUserName(txtBxOthPrson1.Text);
            }
            else if (txtBxOthPrson1.Text == "")
            {
                lblOtherPerson1.Visible = false;
            }
        }
        //Displaying the name of the personID displayed in other person2.
        private void txtBxOthPrson2_TextChanged(object sender, EventArgs e)
        {
            lblOtherPerson2.Visible = true;
            if (txtBxOthPrson2.Text != "")
            {
                lblOtherPerson2.Text = DetermineUserName(txtBxOthPrson2.Text);
            }
            else if (txtBxOthPrson2.Text == "")
            {
                lblOtherPerson2.Visible = false;
            }
        }
        //Displaying the name of the personID displayed in other person3
        private void txtBxOthPrson3_TextChanged(object sender, EventArgs e)
        {
            lblOtherPerson3.Visible = true;
            if (txtBxOthPrson3.Text != "")
            {
                lblOtherPerson3.Text = DetermineUserName(txtBxOthPrson3.Text);
            }
            else if (txtBxOthPrson3.Text == "")
            {
                lblOtherPerson3.Visible = false;
            }
        }
        //Displaying the name of the personID displayed in other person4.
        private void txtBxOthPrson4_TextChanged(object sender, EventArgs e)
        {
            lblOtherPerson4.Visible = true;
            if (txtBxOthPrson4.Text != "")
            {
                lblOtherPerson4.Text = DetermineUserName(txtBxOthPrson4.Text);
            }
            else if (txtBxOthPrson4.Text == "")
            {
                lblOtherPerson4.Visible = false;
            }
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
