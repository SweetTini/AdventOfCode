using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AdventOfCode.Dependencies.IntCode
{
    public class IntCode
    {
        Queue<long> _inputs;
        List<long> _outputs;
        long[] _codes;
        long _address, _base;

        public long MemorySize { get; private set; }

        public ReadOnlyCollection<long> Outputs { get; private set; }

        public bool IsPaused { get; private set; }

        public IntCode(long memorySize = 2048)
        {
            _inputs = new Queue<long>();
            _outputs = new List<long>();

            MemorySize = memorySize;
            Outputs = new ReadOnlyCollection<long>(_outputs);
        }

        public void SetInputs(params long[] inputs)
        {
            _inputs.Clear();
            inputs.ToList().ForEach(x => _inputs.Enqueue(x));
        }

        public long Execute(IEnumerable<int> codes) => Execute(codes.Select(x => (long)x));

        public long Execute(IEnumerable<long> codes)
        {
            if (!codes.Any())
                return int.MinValue;

            _codes = new long[MemorySize];
            Array.Copy(codes.ToArray(), _codes, codes.Count());
            Reset();

            return Execute();
        }

        long Execute()
        {
            bool canStep;

            while (true)
            {
                canStep = true;

                var operation = ReadOpCode(_codes[_address]);
                if (IsPaused || operation.Operator == IntCodeOperator.Exit)
                {
                    if (operation.Operator == IntCodeOperator.Exit)
                    {
                        _inputs.Clear();
                        IsPaused = false;
                    }

                    break;
                }

                switch (operation.Operator)
                {
                    case IntCodeOperator.Add:
                    case IntCodeOperator.Multiply:
                        Calculate(operation);
                        break;
                    case IntCodeOperator.Input:
                    case IntCodeOperator.Output:
                        HandleResponse(operation);
                        break;
                    case IntCodeOperator.JumpIfTrue:
                    case IntCodeOperator.JumpIfFalse:
                        canStep = GotoCheck(operation);
                        break;
                    case IntCodeOperator.LessThan:
                    case IntCodeOperator.Equals:
                        Compare(operation);
                        break;
                    case IntCodeOperator.ShiftBase:
                        ShiftBase(operation);
                        break;
                    default:
                        throw new Exception("Unrecognizable opcode.");
                }

                if (!IsPaused && canStep) _address += GetOffset(operation.Operator);
            }

            return IsPaused ? 0 : _codes[0];
        }

        public long Resume()
        {
            if (!IsPaused) return int.MinValue;
            IsPaused = false;
            return Execute();
        }

        long GetMemory(IntCodeParamMode paramMode, long address)
        {
            long value;

            switch (paramMode)
            {
                default:
                case IntCodeParamMode.Position:
                    value = _codes[address];
                    value = _codes[value];
                    break;
                case IntCodeParamMode.Immediate:
                    value = _codes[address];
                    break;
                case IntCodeParamMode.Relative:
                    value = _codes[address];
                    value = _codes[_base + value];
                    break;
            };

            return value;
        }

        void SetMemory(IntCodeParamMode paramMode, long address, long value)
        {
            switch (paramMode)
            {
                default:
                case IntCodeParamMode.Position:
                    address = _codes[address];
                    _codes[address] = value;
                    break;
                case IntCodeParamMode.Immediate:
                    _codes[address] = value;
                    break;
                case IntCodeParamMode.Relative:
                    address = _codes[address];
                    _codes[_base + address] = value;
                    break;
            };
        }

        void Calculate(IntCodeOperation operation)
        {
            var op1 = GetMemory(operation.ParameterA, _address + 1);
            var op2 = GetMemory(operation.ParameterB, _address + 2);
            var result = operation.Operator == IntCodeOperator.Multiply ? op1 * op2 : op1 + op2;
            SetMemory(operation.ParameterC, _address + 3, result);
        }

        void HandleResponse(IntCodeOperation operation)
        {
            if (operation.Operator == IntCodeOperator.Input)
            {
                if (_inputs.Any())
                {
                    SetMemory(operation.ParameterA, _address + 1, _inputs.Dequeue());
                }
                else
                {
                    IsPaused = true;
                    return;
                }
            }
            else
            {
                var output = GetMemory(operation.ParameterA, _address + 1);
                _outputs.Add(output);
            }
        }

        bool GotoCheck(IntCodeOperation operation)
        {
            var op = GetMemory(operation.ParameterA, _address + 1);
            var nextAddr = GetMemory(operation.ParameterB, _address + 2);
            var canJump = operation.Operator == IntCodeOperator.JumpIfFalse ? op == 0 : op != 0;

            if (canJump)
            {
                _address = nextAddr;
                return false;
            }

            return true;
        }

        void Compare(IntCodeOperation operation)
        {
            var op1 = GetMemory(operation.ParameterA, _address + 1);
            var op2 = GetMemory(operation.ParameterB, _address + 2);
            var result = Convert.ToInt32(operation.Operator == IntCodeOperator.LessThan ? op1 < op2 : op1 == op2);
            SetMemory(operation.ParameterC, _address + 3, result);
        }

        void ShiftBase(IntCodeOperation operation)
        {
            var op = GetMemory(operation.ParameterA, _address + 1);
            _base += op;
        }

        void Reset()
        {
            _address = 0;
            _base = 0;
            _outputs.Clear();
            IsPaused = false;
        }

        IntCodeOperation ReadOpCode(long opCode)
        {
            var parser = new int[4];
            var opCodeStr = opCode.ToString().PadLeft(5, '0');
            var isFirst = true;

            for (int i = opCodeStr.Length - 2; i > -1; i--)
            {
                var data = isFirst 
                    ? opCodeStr.Substring(i, 2)
                    : opCodeStr[i].ToString();

                parser[parser.Length - 1 - i] = int.Parse(data);
                isFirst = false;
            }

            return new IntCodeOperation(
                (IntCodeOperator)parser[0], 
                (IntCodeParamMode)parser[1],                  
                (IntCodeParamMode)parser[2], 
                (IntCodeParamMode)parser[3]);
        }

        int GetOffset(IntCodeOperator oper)
        {
            switch (oper)
            {
                case IntCodeOperator.Add:
                case IntCodeOperator.Multiply:
                case IntCodeOperator.LessThan:
                case IntCodeOperator.Equals:
                    return 4;
                case IntCodeOperator.JumpIfTrue:
                case IntCodeOperator.JumpIfFalse:
                    return 3;
                case IntCodeOperator.Input:
                case IntCodeOperator.Output:
                case IntCodeOperator.ShiftBase:
                    return 2;
                default:
                    return 1;
            }
        }
    }

    public struct IntCodeOperation
    {
        public IntCodeOperator Operator { get; private set; }

        public IntCodeParamMode ParameterA { get; private set; }

        public IntCodeParamMode ParameterB { get; private set; }

        public IntCodeParamMode ParameterC { get; private set; }

        internal IntCodeOperation(
            IntCodeOperator oper, 
            IntCodeParamMode paramA = IntCodeParamMode.Position, 
            IntCodeParamMode paramB = IntCodeParamMode.Position, 
            IntCodeParamMode paramC = IntCodeParamMode.Position)
            : this()
        {
            Operator = oper;
            ParameterA = paramA;
            ParameterB = paramB;
            ParameterC = paramC;
        }
    }

    public enum IntCodeOperator
    {
        Add = 1,
        Multiply = 2,
        Input = 3,
        Output = 4,
        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,
        ShiftBase = 9,
        Exit = 99
    }

    public enum IntCodeParamMode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }
}
