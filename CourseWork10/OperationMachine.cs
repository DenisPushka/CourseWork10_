using System;

namespace CourseWork10
{
    /// <summary>
    /// Операционный автомат.
    /// </summary>
    public class OperationMachine
    {
        #region Поля

        /// <summary>
        /// Логические состояния.
        /// </summary>
        public bool[] X { get; }

        /// <summary>
        /// Счетчик
        /// </summary>
        public byte Count { get; private set; }

        /// <summary>
        /// Множитель А.
        /// </summary>
        private readonly ushort _a;

        /// <summary>
        /// Множитель В.
        /// </summary>
        public ushort B { get; private set; }

        /// <summary>
        /// Регистр С.
        /// </summary>
        public uint C { get; private set; }

        /// <summary>
        /// Нужна для проверка знака.
        /// </summary>
        private byte _d;

        /// <summary>
        /// Микрооперации.
        /// </summary>
        private readonly Action[] _operations;

        /// <summary>
        /// Выход.
        /// </summary>
        public bool Run { get; private set; } = true;

        #endregion

        /// <summary>
        /// Инициализация полей.
        /// </summary>
        /// <param name="a">Множитель А.</param>
        /// <param name="b">Множитель В.</param>
        public OperationMachine(ushort a, ushort b)
        {
            _a = a;
            B = b;
            X = new bool[10];
            X[0] = true;

            _operations = new Action[]
            {
                () => { C = 0; }, // y0.
                () => { _d = (byte)(B >> 15); },
                () => { B = (ushort)((ushort)(B << 1) >> 1); },
                () => { Count = 8; }, // y3.

                () => { C += (uint)(_a << 17) >> 3; }, // y4.
                () => { C += (uint)(_a << 17) >> 2; },
                () => { B = (ushort)((B << 2) >> 4); },
                () =>
                {
                    var buff = C >> 31;
                    buff = ((buff << 1) | buff) << 30;
                    C = (C >> 2) + buff;
                }, // y7.

                () => { Count--; }, // y8.
                () => { C += 0x10000; },
                () => { C |= 0x80000000; },
                () => { Run = false; }
            };
        }

        /// <summary>
        /// Такт.
        /// </summary>
        /// <param name="signals">Вектор сигналов из КСУ.</param>
        public void Step(bool[] signals)
        {
            for (var index = 0; index < signals.Length; index++)
            {
                if (signals[index])
                {
                    _operations[index]();
                }
            }

            LogicalDevice();
        }

        /// <summary>
        /// Вычисление логического результата каждого логического блока. 
        /// </summary>
        private void LogicalDevice()
        {
            X[1] = (_a & 0x7fff) == 0;
            X[2] = (B & 0x7fff) == 0;
            X[3] = (B & 0x0003) == 0;
            X[4] = (B & 0x0003) == 1;
            X[5] = (B & 0x0003) == 2;
            X[6] = Count == 1;
            X[7] = Count == 0;
            X[8] = C << 16 >> 31 != 0;
            X[9] = ((_a >> 15) ^ _d) == 1;
        }
    }
}