using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Dependencies.IntCode
{
    public class IntCode
    {
        Queue<int> _inputs;
        List<int> _codes;
        int _address;

        public int Output { get; private set; }

        public bool IsPaused { get; private set; }

        public IntCode()
        {
            _inputs = new Queue<int>();
        }

        public void SetInputs(params int[] inputs)
        {
            _inputs.Clear();
            inputs.ToList().ForEach(x => _inputs.Enqueue(x));
        }

        public int Execute(List<int> codes)
        {
            _codes = codes.ToList();
            if (!_codes.Any())
                return int.MinValue;
            Reset();

            return Execute();
        }

        int Execute()
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
                    default:
                        throw new Exception("Unrecognizable opcode.");
                }

                if (!IsPaused && canStep) _address += GetOffset(operation.Operator);
            }

            return IsPaused ? int.MinValue : _codes[0];
        }

        public int Resume()
        {
            if (!IsPaused) return int.MinValue;
            IsPaused = false;
            return Execute();
        }

        void Calculate(IntCodeOperation operation)
        {
            var op1 = _codes[_address + 1];
            var op2 = _codes[_address + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op1 = _codes[op1];
            if (operation.ParameterB == IntCodeParamMode.Position) op2 = _codes[op2];

            var result = operation.Operator == IntCodeOperator.Multiply ? op1 * op2 : op1 + op2;

            if (operation.ParameterC == IntCodeParamMode.Position)
                _codes[_codes[_address + 3]] = result;
            else _codes[_address + 3] = result;
        }

        void HandleResponse(IntCodeOperation operation)
        {
            var memAddr = _codes[_address + 1];
            if (operation.ParameterA == IntCodeParamMode.Position) memAddr = _codes[memAddr];
            var memAddrHex = memAddr.ToString("X").PadLeft(8, '0');

            if (operation.Operator == IntCodeOperator.Input)
            {
                if (_inputs.Any())
                {
                    if (operation.ParameterA == IntCodeParamMode.Position)
                        _codes[_codes[_address + 1]] = _inputs.Dequeue();
                    else _codes[_address + 1] = _inputs.Dequeue();
                }
                else
                {
                    IsPaused = true;
                    return;
                }
            }
            else
            {
                Output = memAddr;
            }
        }

        bool GotoCheck(IntCodeOperation operation)
        {
            var op = _codes[_address + 1];
            var nextAddr = _codes[_address + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op = _codes[op];
            if (operation.ParameterB == IntCodeParamMode.Position) nextAddr = _codes[nextAddr];

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
            var op1 = _codes[_address + 1];
            var op2 = _codes[_address + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op1 = _codes[op1];
            if (operation.ParameterB == IntCodeParamMode.Position) op2 = _codes[op2];

            var result = operation.Operator == IntCodeOperator.LessThan ? op1 < op2 : op1 == op2;
            var resultAsNum = Convert.ToInt32(result);

            if (operation.ParameterC == IntCodeParamMode.Position)
                _codes[_codes[_address + 3]] = resultAsNum;
            else _codes[_address + 3] = resultAsNum;
        }

        void Reset()
        {
            _address = 0;
            Output = 0;
            IsPaused = false;
        }

        IntCodeOperation ReadOpCode(int opCode)
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
        Exit = 99
    }

    public enum IntCodeParamMode
    {
        Position = 0,
        Immediate = 1
    }
}
