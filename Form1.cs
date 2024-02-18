using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsForms
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// チェックボックスの幅
        /// </summary>
        private static readonly int CheckBoxWidth = 15;

        /// <summary>
        /// チェックボックスの高さ
        /// </summary>
        private static readonly int CheckBoxHeight = 15;

        /// <summary>
        /// ヘッダチェックボックス列インデックス
        /// </summary>
        private static readonly int HeaderCheckBoxColumnIndex = 0;

        /// <summary>
        /// ヘッダチェックボックス
        /// </summary>
        private CheckBox HeaderCheckBox = new CheckBox();

        public Form1()
        {
            InitializeComponent();

            this.Load += Form1_Load;

            label1.Text = Encrypt();

            var cipherText = Convert.FromBase64String(label1.Text);
            label2.Text = Decrypt(cipherText);

        }


        /// <summary>
        /// フォームがロードされた時に呼び出されます。
        /// </summary>
        /// <param name="sender">フォーム</param>
        /// <param name="e">イベント</param>
        private void Form1_Load(object sender, EventArgs e)
        {
            // データグリッドビューを設定します。
            // 1列目をチェックボックス、2列目をテキストとします。
            dataGridView1.Columns.Add(new DataGridViewCheckBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());

            // 2列目のヘッダテキストを設定します。
            dataGridView1.Columns[1].HeaderText = "名前";

            // データを設定します。
            dataGridView1.Rows.Add(false, "Apple");
            dataGridView1.Rows.Add(false, "Banana");
            dataGridView1.Rows.Add(false, "Orange");

            // 行追加を禁止します。
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.AllowUserToResizeRows = false;


            // ヘッダチェックボックスを設定します。
            HeaderCheckBox.Name = "HeaderCheckbox";
            HeaderCheckBox.Size = new Size(CheckBoxWidth, CheckBoxHeight);
            HeaderCheckBox.CheckedChanged += new EventHandler(HeaderCheckbox_CheckedChanged);
            dataGridView1.Controls.Add(HeaderCheckBox);
            dataGridView1.Columns[0].HeaderText = "全選択";
            dataGridView1.Columns[0].HeaderCell.Style.Padding = new Padding(15,0,0,0);
            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //dataGridView1.Columns[0].DefaultCellStyle.Padding = new Padding(1, 0, 0, 0);

            // ヘッダチェックボックスの位置を設定します。
            SetHeaderCheckBoxLocation();

        }

        /// <summary>
        ///  ヘッダチェックボックスが変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">チェックボックス</param>
        /// <param name="e">イベント</param>
        private void HeaderCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            // データグリッドビューの列目のチェックボックスにヘッダチェックボックスの値を設定します。
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Cells[0].Value = HeaderCheckBox.Checked;
            }

            // 1列目のセルが選択されていると表示上チェックされないため、
            // データグリッドビューの選択セルを無しにします。
            dataGridView1.CurrentCell = null;
        }

        /// <summary>
        /// データグリッドビューの列幅が変更された時に呼び出されます。
        /// </summary>
        /// <param name="sender">データグリッドビュー</param>
        /// <param name="e">イベント</param>
        private void DataGridView_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            // 1列目以外の場合
            if (e.Column.Index != HeaderCheckBoxColumnIndex)
            {
                // 何もしません。
                return;
            }

            // ヘッダチェックボックスの位置を設定します。
            SetHeaderCheckBoxLocation();
        }

        /// <summary>
        /// ヘッダチェックボックスの位置を設定します。
        /// </summary>
        private void SetHeaderCheckBoxLocation()
        {
            // ヘッダ行の1列目のセルの表示領域を取得します。
            var rect = dataGridView1.GetCellDisplayRectangle(HeaderCheckBoxColumnIndex, -1, true);

            // 取得した表示領域でヘッダチェックボックスの大きさを加味して、中心位置に算出します。
            //rect.X = rect.Location.X + (rect.Size.Width - CheckBoxWidth) / 2;
            rect.X = 5;
            rect.Y = rect.Location.Y + (rect.Size.Height - CheckBoxHeight) / 2 + 1;
            rect.Width = rect.Size.Width;
            rect.Height = rect.Size.Height;

            // ヘッダチェックボックスの位置を設定します。
            HeaderCheckBox.Location = rect.Location;
        }

        private string Encrypt()
        {
            string plainText = "password";
            string strGuid = "E3AB38C857F74745B8C73B821DAA1EAA";
            byte[] byteGuid = System.Text.Encoding.UTF8.GetBytes(strGuid);
            string aesKey = Convert.ToBase64String(byteGuid);
            var key = Convert.FromBase64String(aesKey);

            using (var aes = Aes.Create())
            {
                var encryptor = aes.CreateEncryptor(key, aes.IV);
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    foreach(var data in aes.IV){
                        ms.WriteByte(data);
                    }
                    
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }
                    var bytes = ms.ToArray();
                    return Convert.ToBase64String(bytes);
                }
            }
        }

        private string Decrypt(byte[] cipherText)
        {
            string strGuid = "E3AB38C857F74745B8C73B821DAA1EAA";
            byte[] byteGuid = System.Text.Encoding.UTF8.GetBytes(strGuid);
            string aesKey = Convert.ToBase64String(byteGuid);
            var key = Convert.FromBase64String(aesKey);

            var iv = new byte[16];
            Array.Copy(cipherText, 0, iv, 0, iv.Length);
            using (var aes = Aes.Create())
            {
                var decryptor = aes.CreateDecryptor(key, iv);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                    using (var bw = new BinaryWriter(cs))
                    {
                        bw.Write(cipherText, iv.Length, cipherText.Length - iv.Length);
                    }
                    var bytes = ms.ToArray();
                    return System.Text.Encoding.Default.GetString(bytes);
                }
            }
        }

    }
}
        
