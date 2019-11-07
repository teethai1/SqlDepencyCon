using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// How to use this program
// 1. Change ConnectionString
// 2. Change SQL Command for you need monitor Ex. I want to see  QCFirstLotMode, QCFinishLotMode,LotStartTime,LotEndTime
// 3. Click button 1 for active SqlDepencyCon
// 4. Edit data on database for checking event 
// 5. When you edited data on database it will call dependency1_OnChange

//  THARIN YENPAIROJ    07/11/2019

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        SqlConnection connection = null;
        SqlCommand command = null;
        DataSet dataToWatch = null;
        string tableName1 = "DBData";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlDependencyFunction(tbLotno.Text);
        }
         string  GetSQLTest(string LotNo)
        {
            if (LotNo == "")
            {
                return "";
            }
            return "SELECT QCFirstLotMode, QCFinishLotMode,LotStartTime,LotEndTime FROM dbo.DBData WHERE Lotno = '" + LotNo + "' and MCNo = '" + tbMcNo .Text + "'";
        }

        void GetData1()
        {
            dataToWatch.Clear();
            command.Notification = null;
            SqlDependency dependency1 = new SqlDependency(command);
            dependency1.OnChange += dependency1_OnChange;
            using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            {
                adapter.Fill(dataToWatch, tableName1);
                dataGridView1.DataSource = dataToWatch;
                dataGridView1.DataMember = tableName1;

                //เชคเงื่อนไขที่ จังหวะนี้
            }
            
        }

        private void dependency1_OnChange(object sender, SqlNotificationEventArgs e)
        {
            ISynchronizeInvoke i = (ISynchronizeInvoke)this;
            if (i.InvokeRequired)
            {
                OnChangeEventHandler tempDelegate = new OnChangeEventHandler(dependency1_OnChange);
                object[] args = new[] { sender, e };
                i.BeginInvoke(tempDelegate, args);
                return;
            }

            SqlDependency dependency1 = (SqlDependency)sender;
            dependency1.OnChange -= dependency1_OnChange;
            if (e.Type != SqlNotificationType.Change)
                return;
            GetData1();
        }

        private void SqlDependencyFunction(string lotNo)
        {
            SqlDependency.Stop(Properties .Settings.Default.SqlDepencyCon);
            SqlDependency.Start(Properties.Settings.Default.SqlDepencyCon);
            if (connection == null)
                connection = new SqlConnection(Properties.Settings.Default.SqlDepencyCon);
            command = new SqlCommand(GetSQLTest(lotNo), connection);
            if (dataToWatch == null)
                dataToWatch = new DataSet();
            GetData1();
        }
    }
}
