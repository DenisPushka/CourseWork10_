using System;

namespace CourseWork10
{
    /// <summary>
    /// МикроПрограмма.
    /// </summary>
    public class MicroProgram
    {
        #region Поля

        /// <summary>
        /// Счетчик
        /// </summary>
        private byte _count;

        /// <summary>
        /// Множитель А.
        /// </summary>
        private ushort _a;

        /// <summary>
        /// Множитель В.
        /// </summary>
        private ushort _b;

        /// <summary>
        /// Регистр С.
        /// </summary>
        private uint _c;

        /// <summary>
        /// Нужно для определения знака.
        /// </summary>
        private byte _d;

        /// <summary>
        /// Микрооперации.
        /// </summary>
        private readonly Action[] _operations;

        /// <summary>
        /// Выход.
        /// </summary>
        private bool _run = true;

        /// <summary>
        /// Данные внесены в автомат.
        /// </summary>
        private bool _installData;

        /// <summary>
        /// Текущее состояние.
        /// </summary>
        private byte _currentPosition;

        /// <summary>
        /// Главная форма.
        /// </summary>
        private readonly MainForm _mainForm;

        #endregion

        public MicroProgram(MainForm form)
        {
            _mainForm = form;

            _operations = new Action[]
            {
                () => { _c = 0; }, // y0.
                () => { _d = (byte)(_b >> 15); },
                () => { _b = (ushort)((ushort)(_b << 1) >> 1); },
                () => { _count = 8; }, // y3.

                () => { _c += (uint)(_a << 17) >> 3; }, // y4.
                () => { _c += (uint)(_a << 17) >> 2; },
                () => { _b = (ushort)((_b << 2) >> 4); },
                () =>
                {
                    var buff = _c >> 31;
                    buff = ((buff << 1) | buff) << 30;
                    _c = (_c >> 2) + buff;
                }, // y7.

                () => { _count--; }, // y8.
                () => { _c += 0x10000; },
                () => { _c |= 0x80000000; },
                () => { _run = false; }
            };
        }

        /// <summary>
        /// Внесение данные. Если данные еще не внесесы, то добавляются.
        /// </summary>
        /// <param name="a">Множитель А.</param>
        /// <param name="b">Множитель В.</param>
        public void InputData(ushort a, ushort b)
        {
            if (!_installData)
            {
                _a = a;
                _b = b;
                _installData = true;
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
            switch (_currentPosition)
            {
                case 0:
                    if (_run)
                    {
                        if ((_a & 0x7fff) == 0 || (_b & 0x7fff) == 0)
                        {
                            _operations[0]();
                            _currentPosition = 0;
                        }
                        else
                        {
                            _operations[0]();
                            _operations[1]();
                            _operations[2]();
                            _operations[3]();
                            _currentPosition = 1;
                        }
                    }

                    break;
                case 1:
                    if ((_b & 0x0003) != 0)
                    {
                        if ((_b & 0x0003) == 1)
                        {
                            _operations[4]();
                        }
                        else if ((_b & 0x0003) == 2)
                        {
                            _operations[5]();
                        }
                        else
                        {
                            _operations[4]();
                            _currentPosition = 2;
                            break;
                        }


                        _currentPosition = 3;
                        break;
                    }

                    if (_count == 1)
                        _operations[8]();
                    else
                    {
                        _operations[6]();
                        _operations[7]();
                        _operations[8]();
                    }

                    _currentPosition = 4;
                    break;
                case 2:
                    _operations[5]();
                    _currentPosition = 3;
                    break;
                case 3:
                    if (_count == 1)
                    {
                        _operations[8]();
                    }
                    else
                    {
                        _operations[6]();
                        _operations[7]();
                        _operations[8]();
                    }

                    _currentPosition = 4;
                    break;
                case 4:
                    if (_count != 0)
                    {
                        if ((_b & 0x0003) != 0)
                        {
                            if ((_b & 0x0003) == 1)
                            {
                                _operations[4]();
                            }
                            else if ((_b & 0x0003) == 2)
                            {
                                _operations[5]();
                            }
                            else
                            {
                                _operations[4]();
                                _currentPosition = 2;
                                break;
                            }


                            _currentPosition = 3;
                            break;
                        }

                        if (_count == 1)
                            _operations[8]();
                        else
                        {
                            _operations[6]();
                            _operations[7]();
                            _operations[8]();
                        }

                        _currentPosition = 4;
                        break;
                    }

                    if (_c << 16 >> 31 != 0)
                    {
                        _operations[9]();
                        _currentPosition = 5;
                        break;
                    }

                    if (((_a >> 15) ^ _d) == 1)
                    {
                        _operations[10]();
                    }

                    _currentPosition = 6;
                    break;
                case 5:
                    if (((_a >> 15) ^ _d) == 1)
                    {
                        _operations[10]();
                    }

                    _currentPosition = 6;
                    break;
                case 6:
                    _operations[11]();
                    _currentPosition = 0;
                    break;
            }

            // Отображение данных.
            _mainForm.UpdateInfoRegister(_b, _count, _c);
            _mainForm.UpdateStateMemory(_currentPosition);
        }

        // Метод, отвечающий за сброс всех данных.
        public void Reset()
        {
            _a = 0;
            _b = 0;
            _installData = false;
            _currentPosition = 0;
            _run = true;
        }
    }
}