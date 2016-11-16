using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace labExcelBuildFile
{
    public partial class Form1 : Form
    {
        private string _filePath = "";
        private DataTable dt = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "Excel 2003 (*.xls)|*.xls";
            fd.ShowReadOnly = true;
            DialogResult r = fd.ShowDialog();
            
            if(r != DialogResult.OK) return;

            _filePath = fd.FileName;
            labFilePath.Text = "打开文件：" + _filePath;
            btnBuildSave.Enabled = true;

            string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _filePath + ";Extended Properties=Excel 8.0;";
            using (OleDbConnection conn = new OleDbConnection(strConn))
            {
                conn.Open();

                using (OleDbDataAdapter ad = new OleDbDataAdapter("select * from [Sheet1$]", conn))
                {
                    dt = new DataTable();
                    ad.Fill(dt);
                }
            }

            dataGridView.DataSource = dt;
        }


        private void btnBuildSave_Click(object sender, EventArgs e)
        {
            if(0 == txtSQL.Text.Trim().Length)
            {
                MessageBox.Show("请先输入语句！");
                return;
            }

            // string strSQL = "insert into [wj_peixun].[dbo].[User_List]([Cn_Name], [En_Name], [Sex], [Birthday], [Photo], [Nationality_Code], [CardID], [Company], [ProvinceID], [CityID], [ClassCode], [UserID], [UserCode], [Current_Dan], [Dan_Num], [Current_Dan_Poom_Type], [Current_Dan_PassDate], [Request_Dan], [Request_Dan_Poom_Type], [Phone], [Address], [PeiXun_Id], [Intro], [nianxian], [IsUser], [Cta_UserCheck_Flags], [IsSuccess], [CreateTime]) values('__Cn_Name__', '__En_Name__', __Sex__, '__Birthday__', '__Photo__', '__Nationality_Code__', '__CardID__', '__Company__', __ProvinceID__, __CityID__, '__ClassCode__', '__UserID__', '__UserCode__', __Current_Dan__, '__Dan_Num__', __Current_Dan_Poom_Type__, '__Current_Dan_PassDate__', __Request_Dan__, __Request_Dan_Poom_Type__, '__Phone__', '__Address__', __PeiXun_Id__, '__Intro__', __nianxian__, __IsUser__, __Cta_UserCheck_Flags__, __IsSuccess__, '__CreateTime__');";
            string strSQL = txtSQL.Text.Trim();
            string midstr = "";
            string midSQL = "";

            SaveFileDialog sf = new SaveFileDialog();
            sf.Filter = "Text File (*.txt)|*.txt";
            DialogResult r = sf.ShowDialog();

            if(r != DialogResult.OK)
            {
                return;
            }

            string _saveFile = sf.FileName;


            

            int totCoumnCount = dt.Columns.Count;

            for (int i = 0; i < totCoumnCount; i++)
            {
                if (midstr.Length > 0) midstr += "|";
                midstr += dt.Columns[i].ColumnName;
            }

            string[] labTab = midstr.Split('|');

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                midSQL = strSQL;


                for (int j = 0; j < totCoumnCount; j++)
                {

                    midSQL = midSQL.Replace("__" + labTab[j] + "__", dt.Rows[i][labTab[j]].ToString());
                }

                sb.Append(midSQL + "\r\n");
            }

            if (File.Exists(_saveFile))
            {
                File.Delete(_saveFile);
            }

            FileStream fs = new FileStream(_saveFile, FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(sb.ToString());
            sw.Close();
            fs.Close();

            MessageBox.Show("操作完成！");
        }
    }
}
