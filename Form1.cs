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
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());

            // ヘッダテキスト
            dataGridView1.Columns[1].HeaderText = "列2";
            dataGridView1.Columns[2].HeaderText = "列3";
            dataGridView1.Columns[3].HeaderText = "列4";


            dataGridView1.Columns[0].FillWeight = 10;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            dataGridView1.Columns[1].FillWeight = 30;


            // データを設定します。
            dataGridView1.Rows.Add(false, "Apple");
            dataGridView1.Rows.Add(false, "Banana");
            dataGridView1.Rows.Add(false, "Orange");

            // 行追加を禁止します。
            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.AllowUserToResizeRows = false;

            // 複数行、セル結合
            //dataGridView1.CellPainting += dataGridView1_CellPainting;
            dataGridView1.CellPainting += dataGridView1_CellPainting2;
            //dataGridView1.CellPainting += *dataGridView1_CellPainting3;

            // ヘッダチェックボックスを設定します。
            HeaderCheckBox.Name = "HeaderCheckbox";
            /*            HeaderCheckBox.Size = new Size(CheckBoxWidth, CheckBoxHeight);
                        HeaderCheckBox.CheckedChanged += new EventHandler(HeaderCheckbox_CheckedChanged);
                        dataGridView1.Controls.Add(HeaderCheckBox);
                        dataGridView1.Columns[0].HeaderText = "全選択";
                        dataGridView1.Columns[0].HeaderCell.Style.Padding = new Padding(15,0,0,0);
            */            //dataGridView1.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                          //dataGridView1.Columns[0].DefaultCellStyle.Padding = new Padding(1, 0, 0, 0);

            // ヘッダチェックボックスの位置を設定します。
            //SetHeaderCheckBoxLocation();

            this.dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.MultiSelect = false;
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
                    foreach (var data in aes.IV)
                    {
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



        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //ヘッダー行以外はスキップ
            if(e.RowIndex != -1)
            {
                return;
            }

            //2-3列目のヘッダーを2行にする
            //if(e.ColumnIndex == 1 || e.ColumnIndex == 2)
            //{
            //    // 既存の描画を無効
            //    e.Paint(e.CellBounds, DataGridViewPaintParts.None);
            //    e.Handled = true;
            //}

            //描画領域の取得
            var rect = dataGridView1.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
            rect.Width -= 1;
            rect.Height -= 1;

            //背景塗りつぶし
            e.Graphics.FillRectangle(new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.BackColor), rect);
            //外枠描画
            e.Graphics.DrawRectangle(new Pen(dataGridView1.GridColor), rect);
            var separatedHeight = rect.Height / 2;
            //分割線を描画
            for(int i = 0; i<2; i++)
            {
                if (i == 0)
                {
                    continue;

                }
                else
                {
                    e.Graphics.DrawLine(
                        new Pen(SystemColors.ControlDark),
                        rect.Left,
                        rect.Top + separatedHeight * i,
                        rect.Right,
                        rect.Top + separatedHeight * i);
                }
            }


        }

        private void dataGridView1_CellPainting2(object sender, DataGridViewCellPaintingEventArgs e)
        {
            //ヘッダー行以外はスキップ
            if (e.RowIndex != -1)
            {
                return;
            }

            //セルの矩形を取得
            Rectangle joinRect = new Rectangle();

            //2-3列目の1行目のヘッダーセルを結合
            if (e.ColumnIndex == 1)
            {
                //セルの矩形を取得
                joinRect = e.CellBounds;

                //3列目の幅を取得し2列目の幅に足す
                joinRect.Width += dataGridView1.Columns[2].Width;
                joinRect.Height = joinRect.Height / 2;

                //矩形の位置を補正
                joinRect.Y -= 1;


                //e.Graphics.FillRectangle(new SolidBrush(dataGridView1.ColumnHeadersDefaultCellStyle.BackColor), joinRect);

                e.Graphics.DrawRectangle(new Pen(dataGridView1.GridColor), joinRect);

            }

            if(e.ColumnIndex == 1 || e.ColumnIndex == 2)
            {
                for(int i= 0; i<2; i++)
                {
                    Rectangle nomalRect = e.CellBounds;

                    nomalRect.Height = joinRect.Height;
                    nomalRect.Width = dataGridView1.Rows[1].Cells[i + 1].ContentBounds.Width;
                    //nomalRect.Width += nomalRect.Width;
                    nomalRect.X +=nomalRect.Width;
                    nomalRect.Y += joinRect.Height - 1;

                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.ControlDark), nomalRect);

                    e.Graphics.DrawRectangle(new Pen(dataGridView1.GridColor), nomalRect);
                }
                
            }

            // 結合セル以外は既定の描画を行う
            if (!(e.ColumnIndex == 1 || e.ColumnIndex == 2))
            {
                e.Paint(e.ClipBounds, e.PaintParts);
            }

            // イベントハンドラ内で処理を行ったことを通知
            e.Handled = true;
        }


        private void dataGridView1_CellPainting3(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex != -1) return;

            if (e.ColumnIndex == 2)
            {
                e.AdvancedBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.None;
            }

            if (e.ColumnIndex >= 2)
            {
                e.AdvancedBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.None;
                e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.None;

                if (e.ColumnIndex < dataGridView1.Columns.Count - 1 &&
                    Convert.ToString(dataGridView1.Rows[0].Cells[e.ColumnIndex + 1].Value) != "")
                {
                    e.AdvancedBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
                }

            }
        }

    }

}