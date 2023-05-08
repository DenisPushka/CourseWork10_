namespace CourseWork10
{
    /// <summary>
    /// Управляющий автомат.
    /// </summary>
    public class ManageMachine
    {
        #region Поля

        /// <summary>
        /// Сигналы из КСД.
        /// </summary>
        private bool[] _d;

        /// <summary>
        /// Текущее состояние автомата.
        /// </summary>
        private bool[] _a;

        /// <summary>
        /// Термы.
        /// </summary>
        private readonly bool[] _t;

        /// <summary>
        /// Предыдущая метка автомата.
        /// </summary>
        private byte _lastState;

        /// <summary>
        /// Операционный автомат.
        /// </summary>
        private OperationMachine _operationMachine;

        private readonly MainForm _mainForm;

        /// <summary>
        /// Выход.
        /// </summary>
        private bool _run = true;

        #endregion

        /// <summary>
        /// Данные внесены в автомат.
        /// </summary>
        private bool InstallData { get; set; }


        /// <summary>
        /// Инициализация значений по умолчанию.
        /// </summary>
        public ManageMachine(MainForm form)
        {
            _mainForm = form;
            _a = new bool[7];
            _a[0] = true;
            _d = new bool[3];
            _t = new bool[23];
        }

        /// <summary>
        /// Внесение данные. Если данные еще не внесесы, то добавляются.
        /// </summary>
        /// <param name="a">Множитель А.</param>
        /// <param name="b">Множитель В.</param>
        public void InputData(ushort a, ushort b)
        {
            if (!InstallData)
            {
                _operationMachine = new OperationMachine(a, b);
                InstallData = true;
            }
        }

        /// <summary>
        /// Автоматический режим.
        /// </summary>
        public void AutomaticMode()
        {
            while (_run)
            {
                Step();
            }
        }

        /// <summary>
        /// Такт.
        /// </summary>
        public void Step()
        {
            if (!_run)
                return;
            
            var x = _operationMachine.X;
            _mainForm.UpdateInfoPly(x);
            _mainForm.UpdateInfoState(_d);

            StateMemory(Decoder());
            KC_T(x);
            var y = KC_Y();
            KC_D();
            _operationMachine.Step(y);

            _run = _operationMachine.Run;
            // Вывод информации на остальные схемы.
            _mainForm.UpdateInfoRegister(_operationMachine.B, _operationMachine.Count, _operationMachine.C);
            _mainForm.UpdateStateMemory(_a);
            _mainForm.UpdateInfoKc(_t, y, _d, _operationMachine.X);
        }

        /// <summary>
        /// Комбинационная схема T(Терма).
        /// </summary>
        private void KC_T(bool[] x)
        {
            _t[0] = _a[0] && !x[0];
            _t[1] = _a[6];
            _t[2] = _a[0] && x[0] && !x[1] && !x[2];
            _t[3] = _a[1] && !x[3] && !x[4] && !x[5];
            _t[4] = _a[4] && !x[7] && !x[3] && !x[4] && !x[5];
            _t[5] = _a[1] && !x[3] && x[4];
            _t[6] = _a[1] && !x[3] && !x[4] && x[5];
            _t[7] = _a[2];
            _t[8] = _a[4] && !x[7] && !x[3] && x[4];
            _t[9] = _a[4] && !x[7] && !x[3] && !x[4] && x[5];
            _t[10] = _a[1] && x[3] && x[6];
            _t[11] = _a[1] && x[3] && !x[6];
            _t[12] = _a[3] && x[6];
            _t[13] = _a[3] && !x[6];
            _t[14] = _a[4] && !x[7] && x[3] && x[6];
            _t[15] = _a[4] && !x[7] && x[3] && !x[6];
            _t[16] = _a[4] && x[7] && x[8];
            _t[17] = _a[0] && x[0] && x[1];
            _t[18] = _a[0] && x[0] && !x[1] && x[2];
            _t[19] = _a[4] && x[7] && !x[8] && x[9];
            _t[20] = _a[4] && x[7] && !x[8] && !x[9];
            _t[21] = _a[5] && x[9];
            _t[22] = _a[5] && !x[9];
        }

        /// <summary>
        /// Комбинационная схема D.
        /// </summary>
        private void KC_D()
        {
            _d[0] = _t[2] || _t[5] || _t[6] || _t[7] || _t[8] || _t[9] || _t[16];
            _d[1] = _t[3] || _t[4] || _t[5] || _t[6] || _t[7] || _t[8] || _t[9]
                    || _t[17] || _t[18] || _t[19] || _t[20] || _t[21] || _t[22];
            _d[2] = _t[10] || _t[11] || _t[12] || _t[13] || _t[14] || _t[15]
                    || _t[16] || _t[17] || _t[18] || _t[19] || _t[20] || _t[21] || _t[22];
        }

        /// <summary>
        /// Комбинационная схема Y.
        /// </summary>
        private bool[] KC_Y()
        {
            var signal = new bool[12];

            signal[0] = _t[2] || _t[17] || _t[18];
            signal[1] = signal[2] = signal[3] = _t[2];
            signal[4] = _t[3] || _t[4] || _t[5] || _t[8];
            signal[5] = _t[6] || _t[7] || _t[9];
            signal[6] = signal[7] = _t[11] || _t[13] || _t[15];
            signal[8] = _t[10] || _t[11] || _t[12] || _t[13] || _t[14] || _t[15];
            signal[9] = _t[16];
            signal[10] = _t[19] || _t[21];
            signal[11] = _t[1];

            return signal;
        }

        /// <summary>
        /// Дешифратор.
        /// </summary>
        private byte Decoder()
        {
            byte number = 0;

            if (_d[0])
                number = 1;
            if (_d[1])
                number += 2;
            if (_d[2])
                number += 4;

            return number;
        }

        /// <summary>
        /// Память состояний.
        /// </summary>
        /// <param name="newState">Индекс нового состояния.</param>
        private void StateMemory(byte newState)
        {
            _a[_lastState] = false;
            // Запоминаем состояние, чтобы можно было его установить в false на след такте.
            _lastState = newState;
            _a[newState] = true;
        }

        /// <summary>
        /// Сброс всех данных.
        /// </summary>
        public void Reset()
        {
            _operationMachine = new OperationMachine(0, 0);
            InstallData = false;
            _lastState = 0;
            _a = new bool[6];
            _a[0] = true;
            _d = new bool[3];
            _run = true;
        }
    }
}