using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CourseWork10
{
    /// <summary>
    /// Главная форма.
    /// </summary>
    public partial class MainForm : Form
    {
        #region Свойства

        /// <summary>
        /// Множитель А.
        /// </summary>
        private ushort A { get; set; }

        /// <summary>
        /// Множитель В.
        /// </summary>
        private ushort B { get; set; }

        /// <summary>
        /// Управляющий автомат.
        /// </summary>
        private ManageMachine ManageMachine { get; }

        /// <summary>
        /// Микропрограмма.
        /// </summary>
        private MicroProgram MicroProgram { get; }

        #endregion

        /// <summary>
        /// Инициализация компонентов. 
        /// </summary>
        public MainForm()
        {
            InitializeComponent();

            dataGridView_A.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_B_input.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_register_C.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_register_B.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_count.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView_A.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_B_input.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_register_B.Font = new Font("Microsoft Sans Serif", 8);
            dataGridView_count.Font = new Font("Microsoft Sans Serif", 8);
            const int widthColumn = 25;
            var width = 0;

            for (var i = 4 - 1; i >= 0; i--)
            {
                var index = dataGridView_count.Columns.Add("column_" + i, i.ToString());
                dataGridView_count.Columns[index].Width = widthColumn;
                width += widthColumn;
            }

            dataGridView_count.Height = 45;
            dataGridView_count.Width = width + 3;
            width = 0;

            for (var i = 16 - 1; i >= 0; i--)
            {
                var index = dataGridView_A.Columns.Add("column_" + i, i.ToString());
                dataGridView_B_input.Columns.Add("column_" + i, i.ToString());
                dataGridView_register_B.Columns.Add("column_" + i, i.ToString());
                dataGridView_A.Columns[index].Width = widthColumn;
                dataGridView_B_input.Columns[index].Width = widthColumn;
                dataGridView_register_B.Columns[index].Width = widthColumn;
                width += widthColumn;
            }

            dataGridView_A.Height = 45;
            dataGridView_A.Width = width + 2;
            dataGridView_B_input.Height = 45;
            dataGridView_B_input.Width = width + 3;
            dataGridView_register_B.Height = 45;
            dataGridView_register_B.Width = width + 3;
            width = 0;

            for (var i = 32 - 1; i >= 0; i--)
            {
                var index = dataGridView_register_C.Columns.Add("column_" + i, i.ToString());
                dataGridView_register_C.Columns[index].Width = widthColumn;
                width += widthColumn;
            }

            dataGridView_register_C.Height = 45;
            dataGridView_register_C.Width = width + 3;

            dataGridView_A.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_B_input.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_register_B.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_register_C.Rows.Add(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 
                0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            dataGridView_count.Rows.Add(0, 0, 0, 0);
            dataGridView_A.BorderStyle = BorderStyle.FixedSingle;
            dataGridView_B_input.BorderStyle = BorderStyle.FixedSingle;
            dataGridView_register_C.BorderStyle = BorderStyle.FixedSingle;
            dataGridView_register_B.BorderStyle = BorderStyle.FixedSingle;
            dataGridView_count.BorderStyle = BorderStyle.FixedSingle;
            dataGridView_A.RowHeadersVisible = false;
            dataGridView_B_input.RowHeadersVisible = false;
            dataGridView_register_C.RowHeadersVisible = false;
            dataGridView_count.RowHeadersVisible = false;
            dataGridView_register_B.RowHeadersVisible = false;
            dataGridView_A.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView_B_input.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView_register_C.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView_count.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dataGridView_register_B.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            ManageMachine = new ManageMachine(this);
            MicroProgram = new MicroProgram(this);
            radioButton_A0.Checked = true;
        }

        /// <summary>
        /// Обработчик нажатия на регистр А.
        /// </summary>
        private void dataGridView_A_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var value = (int)dataGridView_A.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            value = value == 0 ? 1 : 0;
            dataGridView_A.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            var cells = dataGridView_A.Rows[e.RowIndex].Cells;
            var strB = new StringBuilder();
            for (var i = 0; i < cells.Count; i++)
                strB.Append(cells[i].Value);

            var denial = false;
            ushort a = 0;
            if ((int)(cells[0].Value) == 1)
            {
                a = (ushort)Convert.ToInt16(strB.ToString(), 2);
                strB.Replace("1", "0", 0, 1);
                denial = true;
            }

            A = (ushort)Convert.ToInt16(strB.ToString(), 2);
            A = denial ? a : A;
        }

        /// <summary>
        /// Обработчик нажатия на регистр В.
        /// </summary>
        private void dataGridView_B_input_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;

            var value = (int)dataGridView_B_input.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            value = value == 0 ? 1 : 0;
            dataGridView_B_input.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = value;
            var cells = dataGridView_B_input.Rows[e.RowIndex].Cells;
            var strB = new StringBuilder();
            for (var i = 0; i < cells.Count; i++)
                strB.Append(cells[i].Value);

            var denial = false;
            ushort b = 0;
            if (strB[0] - '0' == 1)
            {
                b = (ushort)Convert.ToInt16(strB.ToString(), 2);
                strB.Replace("1", "0", 0, 1);
                denial = true;
            }

            B = (ushort)Convert.ToInt16(strB.ToString(), 2);
            B = denial ? b : B;
        }

        /// <summary>
        /// Обновление визуальной части регистров.
        /// </summary>
        /// <param name="b">Обновляемый регистр В.</param>
        /// <param name="count">Счетчик.</param>
        /// <param name="c">Обновляемый регистр С.</param>
        public void UpdateInfoRegister(ushort b, byte count, uint c)
        {
            // update info register B.
            UpdateInfoRegister(dataGridView_register_B, b, 16);

            // update count.
            UpdateInfoRegister(dataGridView_count, count, 4);

            // update info about number C.
            var result = Convert.ToString(c, 2).PadLeft(32, '0');
            var bufferRes = string.Copy(result);

            for (var i = 32 - 1; i >= 0; i--)
                dataGridView_register_C.Rows[0].Cells[i].Value = bufferRes[i];
        }

        /// <summary>
        /// Обновление визуальной части регистров.
        /// </summary>
        /// <param name="table">Таблица с данными.</param>
        /// <param name="value">Число в ushort.</param>
        /// <param name="count">Количество разрядов.</param>
        private static void UpdateInfoRegister(DataGridView table, ushort value, short count)
        {
            var result = Convert.ToString(value, 2).PadLeft(count, '0');
            for (var i = count - 1; i >= 0; i--)
                table.Rows[0].Cells[i].Value = result[i];
        }

        /// <summary>
        /// Обновление состояния автомата.
        /// </summary>
        /// <param name="state">Массив с метками состояний.</param>
        public void UpdateStateMemory(bool[] state)
        {
            radioButton_A0.Checked = state[0];
            radioButton_A1.Checked = state[1];
            radioButton_A2.Checked = state[2];
            radioButton_A3.Checked = state[3];
            radioButton_A4.Checked = state[4];
            radioButton_A5.Checked = state[5];
            radioButton_A6.Checked = state[6];
            for (var i = 0; i < state.Length; i++)
                checkedListBox_A.SetItemChecked(i, state[i]);
        }

        /// <summary>
        /// Обновление состояния автомата.
        /// </summary>
        /// <param name="a">Номер метки.</param>
        public void UpdateStateMemory(ushort a)
        {
            radioButton_A0.Checked = a == 0;
            radioButton_A1.Checked = a == 1;
            radioButton_A2.Checked = a == 2;
            radioButton_A3.Checked = a == 3;
            radioButton_A4.Checked = a == 4;
            radioButton_A5.Checked = a == 5;
            radioButton_A6.Checked = a == 6;
        }

        /// <summary>
        /// Обновление значений в комабинационных схемах.
        /// </summary>
        /// <param name="t">Терма.</param>
        /// <param name="y">Выходные сигналы из КСУ.</param>
        /// <param name="d">Выходные сигналы из КСD.</param>
        /// <param name="x">Выходные логические состояния из ОА.</param>
        public void UpdateInfoKc(bool[] t, bool[] y, bool[] d, bool[] x)
        {
            for (var i = 0; i < t.Length; i++)
                checkedListBox_T.SetItemChecked(i, t[i]);
            for (var i = 0; i < y.Length; i++)
                checkedListBox_Y.SetItemChecked(i, y[i]);
            for (var i = 0; i < d.Length; i++)
                checkedListBox_D.SetItemChecked(i, d[i]);
            for (var i = 0; i < x.Length - 1; i++)
                checkedListBox_x.SetItemChecked(i, x[i + 1]);
        }
        
        /// <summary>
        /// Обновление текущего состояния.
        /// </summary>
        /// <param name="dt">Текущее состояние.</param>
        public void UpdateInfoState(bool[] dt)
        {
            for (var i = 0; i < dt.Length; i++)
                checkedListBox_stateMemory.SetItemChecked(i, dt[i]);
        }

        /// <summary>
        /// Обновление значений в ПЛУ.
        /// </summary>
        /// <param name="x">Выходные логические состояния из ОА.</param>
        public void UpdateInfoPly(bool[] x)
        {
            for (int i = 0, q = 0; i < x.Length; i++)
            {
                if (i == 0
                    || i == 1
                    || i == 2
                    || i == 4
                    || i == 5
                    || i == 8
                    || i == 9)
                {
                    continue;
                }

                checkedListBox_PLY.SetItemChecked(q, x[i]);
                q++;
            }
        }

        #region Buttons

        /// <summary>
        /// Сброс данных на регистрах.
        /// </summary>
        private void Button_reset_Click(object sender, EventArgs e)
        {
            ManageMachine.Reset();
            MicroProgram.Reset();
            UpdateInfoRegister(0, 0, 0);
            UpdateStateMemory(0);
        }

        /// <summary>
        /// Обработчик нажатия на кнопку "Такт".
        /// </summary>
        private void Button_Step_Click(object sender, EventArgs e)
        {
            if (checkBox_x0_1.Checked)
            {
                if (RadioButton_YA.Checked)
                {
                    if (RadioButton_Auto.Checked)
                    {
                        ManageMachine.InputData(A, B);
                        ManageMachine.AutomaticMode();
                    }
                    else
                    {
                        ManageMachine.InputData(A, B);
                        ManageMachine.Step();
                    }
                }
                else
                {
                    if (RadioButton_Auto.Checked)
                    {
                        MicroProgram.InputData(A, B);
                        MicroProgram.AutomaticMode();
                    }
                    else
                    {
                        MicroProgram.InputData(A, B);
                        MicroProgram.Step();
                    }
                }
            }
        }

        #endregion
    }
}