using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Moky_Project
{
    public partial class Form1 : Form
    {
       static string Connectionstring = "Server=.;Database=FawryDB;User ID=sa; Password=samir123456";
        SqlConnection connection = new SqlConnection(Connectionstring);

        float Total_Fawry = 0;
        float Total_Cash = 0;

        private bool dragging = false;
        private Point offset;

        void DisapleAll()
        {
            PNumberTB.Enabled = false;
            FBalanceTB.Enabled = false;
            CBalanceTB.Enabled = false;
            IDToDeleteTB.Enabled = false;
            SubmitBTN.Enabled = false;
            DeleteBTN.Enabled = false;
        }
        void EnapleAll()
        {
            PNumberTB.Enabled = true;
            FBalanceTB.Enabled = true;
            CBalanceTB.Enabled = true;
            IDToDeleteTB.Enabled = true;
            SubmitBTN.Enabled = true;
            DeleteBTN.Enabled = true;
        }
        public Form1()
        {
            InitializeComponent();
            DisapleAll();
            _RefereshDataSource();
            SectionList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            float TotalFawry = GetFawryTotalBalanceFromDatabase();
            remainder.Text = TotalFawry.ToString();
        }

        private void SubmitBTN_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(PNumberTB.Text) || String.IsNullOrEmpty(FBalanceTB.Text) || String.IsNullOrEmpty(CBalanceTB.Text))
            {
                MessageBox.Show("Please Fill All Fields", "Error In Inputs");
                return;
            }

            string ClientPhoneNumber = PNumberTB.Text;
            string ClientFawryBalance = FBalanceTB.Text;
            string ClientCashBalance = CBalanceTB.Text;
            int InsertedID  = InsertClient(ClientPhoneNumber , ClientFawryBalance , ClientCashBalance);

            // DataBase To Get Fawry Balance And Cash Balance Then Minus Them From total Fawry
            float Geted_FawryBalance = 0F;
            float Geted_CahsBalance = 0F;
            GetFawryGetCash__Reminder(InsertedID,ref Geted_FawryBalance , ref Geted_CahsBalance);
            // Get Fawry Totol Balance From Table To Mius it
            float TotalFawry = GetFawryTotalBalanceFromDatabase();

            Total_Fawry = TotalFawry - Geted_FawryBalance;
            Total_Cash += Geted_CahsBalance;

            UpdateFawryTotalBalanceFromDatabase(Total_Fawry);

            PNumberTB.Clear();
            FBalanceTB.Clear();
            CBalanceTB.Clear();

            _RefereshDataSource();

            remainder.Text = Total_Fawry.ToString();
            label15.Text = Total_Fawry.ToString();
            AllCash.Text = Total_Cash.ToString();
        }
      


        private void DeleteBTN_Click(object sender, EventArgs e)
        {
            float fawryBalance = 0;
            float fawryBalance__Save = 0;

            float cashBalance = 0;

            if (String.IsNullOrEmpty(IDToDeleteTB.Text))
            {
                MessageBox.Show("Error In Input", "Please Enter The ID To Delete ", MessageBoxButtons.OK ,MessageBoxIcon.Error);
                return;
            }

            
            int idToDelete = Convert.ToInt32(IDToDeleteTB.Text);




            GetFawryGetCash__Reminder(idToDelete,ref fawryBalance, ref cashBalance);


            float TotalFawry = GetFawryTotalBalanceFromDatabase();

            fawryBalance__Save = TotalFawry + fawryBalance;

            UpdateFawryTotalBalanceFromDatabase(fawryBalance__Save);
            Total_Cash -= cashBalance;

            label15.Text = fawryBalance__Save.ToString();
            remainder.Text = fawryBalance__Save.ToString();
            AllCash.Text = Total_Cash.ToString();
            DeleteClient(idToDelete);
            _RefereshDataSource();
        }
        private void label8_Click(object sender, EventArgs e)
        {
            this.Close();
        }



        private void SectionList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SectionList.ClearSelection();

        }

        private void SectionList_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            SectionList.ClearSelection();

        }

        private void SectionList_SelectionChanged(object sender, EventArgs e)
        {
            SectionList.ClearSelection();
        }

 

        private void PNumberTB_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }

            e.Handled = true;
        }

        private void FBalanceTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }

            if (e.KeyChar == '.')
            {
                if (FBalanceTB.Text.Contains("."))
                {
                    e.Handled = true;
                }
                return;
            }
            e.Handled = true;
        }

        private void CBalanceTB_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                return;
            }
            if (char.IsDigit(e.KeyChar))
            {
                return;
            }

            if (e.KeyChar == '.')
            {
                if (CBalanceTB.Text.Contains("."))
                {
                    e.Handled = true;
                }
                return;
            }
            e.Handled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Error In Input", "Please Enter Total Balance", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            remainder.Text = textBox1.Text;          
            button1.Enabled = false;
            textBox1.Enabled = false;

            float TotalFawryFromTB = Convert.ToSingle(textBox1.Text);
            UpdateFawryTotalBalanceFromDatabase(TotalFawryFromTB);
            EnapleAll();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                offset = new Point(e.X, e.Y);
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point currentScreenPos = PointToScreen(e.Location);
                Location = new Point(currentScreenPos.X - offset.X, currentScreenPos.Y - offset.Y);
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }




        // Data Base Layer
        public int InsertClient(string ClientPhoneNumber , string ClientFawryBalance , string ClientCashBalance)
        {
            int Inserted_ID = 0;
            string Insert_Query = @"insert into ClientCharges values (@PhoneNumber, @FawryBalance , @CashBalance);
                                 Select Scope_IDentity();";
            SqlCommand Insert_command = new SqlCommand(Insert_Query, connection);
            Insert_command.Parameters.AddWithValue("@PhoneNumber", ClientPhoneNumber);
            Insert_command.Parameters.AddWithValue("@FawryBalance", ClientFawryBalance);
            Insert_command.Parameters.AddWithValue("@CashBalance", ClientCashBalance);

            try
            {
                connection.Open();
                object Result = Insert_command.ExecuteScalar();
                if (Result != null && int.TryParse(Result.ToString() , out Inserted_ID))
                {
                    MessageBox.Show("تم اضافة الرقم والمال بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(" لم يتم اضافة الرقم والمال بنجاح فشل", "فشل", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
            return Inserted_ID;
        }

        public void DeleteClient(int ID )
        {
            string Delete_Query = @"delete from ClientCharges where ID = @ID";
            SqlCommand Delete_Command = new SqlCommand(Delete_Query, connection);
            Delete_Command.Parameters.AddWithValue("@ID", ID);
            try
            {
                connection.Open();
                int RowsAffected = Delete_Command.ExecuteNonQuery();
                if (RowsAffected > 0)
                {
                    MessageBox.Show($"تم حذف العملية بنجاح {ID}", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show($"فشل حذف العملية {ID}", "فشل", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        public DataTable GetAllClients_Charges()
        {
            DataTable dt = new DataTable();
            string GetAll = @"select * from ClientCharges";
            SqlCommand GetALl_Command = new SqlCommand(GetAll, connection);

            try
            {
                connection.Open();
                SqlDataReader reader = GetALl_Command.ExecuteReader();
                if (reader.HasRows)
                {
                    dt.Load(reader);
                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
            return dt;
        }

        private void _RefereshDataSource()
        {
            SectionList.DataSource = GetAllClients_Charges();
        }

        public void GetFawryGetCash__Reminder(int InsertedID ,ref float Fawry_Remainder , ref float Cash_Remainder)
        {
            string Query = @"select * from ClientCharges where ID = @ID";
            SqlCommand sqlCommand = new SqlCommand(Query, connection);
            sqlCommand.Parameters.AddWithValue("@ID", InsertedID);

            try
            {
                connection.Open();
                SqlDataReader reader = sqlCommand.ExecuteReader();
                if (reader.Read())
                {
                    Fawry_Remainder = Convert.ToSingle(reader["FawryBalance"]);
                    Cash_Remainder = Convert.ToSingle(reader["CashBalance"]);
                }
                reader.Close();
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("هنا مفيش مشكلة", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
        }

        public float GetFawryTotalBalanceFromDatabase()
        {
            float FawryTotalBalanceFromDataBase = 0;
            string GetFawryTotalBalance_Query = @"select TotalFawryBalance from TotalFawryBalance;";
            SqlCommand GetFawryTotalBalance_Command = new SqlCommand(GetFawryTotalBalance_Query, connection);
            try
            {
                connection.Open();
                SqlDataReader reader = GetFawryTotalBalance_Command.ExecuteReader();
                if (reader.Read())
                {
                    FawryTotalBalanceFromDataBase = Convert.ToSingle(reader["TotalFawryBalance"]);
                }
                reader.Close();
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("المشكلة هنا رقم 2", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
            return FawryTotalBalanceFromDataBase;
        }

        public void UpdateFawryTotalBalanceFromDatabase(float GetedTotalFawryBalance)
        {
            string GetFawryTotalBalance_Query = @"UPDATE TotalFawryBalance
                                                    SET TotalFawryBalance = @GetedTotalFawryBalance;";
            SqlCommand GetFawryTotalBalance_Command = new SqlCommand(GetFawryTotalBalance_Query, connection);
            GetFawryTotalBalance_Command.Parameters.AddWithValue("@GetedTotalFawryBalance", GetedTotalFawryBalance);

            try
            {
                connection.Open();
                int RowsAffected = GetFawryTotalBalance_Command.ExecuteNonQuery();
                if (RowsAffected > 0)
                {
                    MessageBox.Show("تم تحديث رصيد فوري الكلي", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("لم يتم تحديث رصيد فوري الكلي", "فشل", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
            }
            catch (Exception Error)
            {
                MessageBox.Show(Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                connection.Close();
            }
            
        }


    }
}
