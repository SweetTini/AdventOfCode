using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Dependencies.IntCode
{
    public class IntCode
    {
        Queue<int> _inputs;

        public int Output { get; private set; } = -1;

        public IntCode()
        {
            _inputs = new Queue<int>();
        }

        public void SetInputs(params int[] inputs)
        {
            _inputs.Clear();
            inputs.ToList().ForEach(x => _inputs.Enqueue(x));
        }

        public int Execute(List<int> instructions)
        {
            var temp = instructions.ToList();
            var addr = 0;
            bool canStep;

            if (!temp.Any()) return -1;
            Reset();

            while (true)
            {
                canStep = true;

                var operation = ReadOpCode(temp[addr]);
                if (operation.Operator == IntCodeOperator.Exit) break;

                switch (operation.Operator)
                {
                    case IntCodeOperator.Add:
                    case IntCodeOperator.Multiply:
                        Calculate(operation, temp, addr);
                        break;
                    case IntCodeOperator.Input:
                    case IntCodeOperator.Output:
                        HandleResponse(operation, temp, addr);
                        break;
                    case IntCodeOperator.JumpIfTrue:
                    case IntCodeOperator.JumpIfFalse:
                        canStep = GotoCheck(operation, temp, ref addr);
                        break;
                    case IntCodeOperator.LessThan:
                    case IntCodeOperator.Equals:
                        Compare(operation, temp, addr);
                        break;
                    default:
                        throw new Exception("Unrecognizable opcode.");
                }

                if (canStep) addr += GetOffset(operation.Operator);
            }

            return temp[0];
        }

        void Calculate(IntCodeOperation operation, List<int> codes, int addr)
        {
            var op1 = codes[addr + 1];
            var op2 = codes[addr + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op1 = codes[op1];
            if (operation.ParameterB == IntCodeParamMode.Position) op2 = codes[op2];

            var result = operation.Operator == IntCodeOperator.Multiply ? op1 * op2 : op1 + op2;

            if (operation.ParameterC == IntCodeParamMode.Position)
                codes[codes[addr + 3]] = result;
            else codes[addr + 3] = result;
        }

        void HandleResponse(IntCodeOperation operation, List<int> codes, int addr)
        {
            var memAddr = codes[addr + 1];
            if (operation.ParameterA == IntCodeParamMode.Position) memAddr = codes[memAddr];
            var memAddrHex = memAddr.ToString("X").PadLeft(8, '0');

            if (operation.Operator == IntCodeOperator.Input)
            {
                if (_inputs.Any())
                    if (operation.ParameterA == IntCodeParamMode.Position)
                        codes[codes[addr + 1]] = _inputs.Dequeue();
                    else codes[addr + 1] = _inputs.Dequeue();
                else throw new Exception("No inputs provided.");
            }
            else
            {
                Output = memAddr;
            }
        }

        bool GotoCheck(IntCodeOperation operation, List<int> codes, ref int addr)
        {
            var op = codes[addr + 1];
            var nextAddr = codes[addr + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op = codes[op];
            if (operation.ParameterB == IntCodeParamMode.Position) nextAddr = codes[nextAddr];

            var canJump = operation.Operator == IntCodeOperator.JumpIfFalse ? op == 0 : op != 0;
            if (canJump)
            {
                addr = nextAddr;
                return false;
            }

            return true;
        }

        void Compare(IntCodeOperation operation, List<int> codes, int addr)
        {
            var op1 = codes[addr + 1];
            var op2 = codes[addr + 2];

            if (operation.ParameterA == IntCodeParamMode.Position) op1 = codes[op1];
            if (operation.ParameterB == IntCodeParamMode.Position) op2 = codes[op2];

            var result = operation.Operator == IntCodeOperator.LessThan ? op1 < op2 : op1 == op2;
            var resultAsNum = Convert.ToInt32(result);

            if (operation.ParameterC == IntCodeParamMode.Position)
                codes[codes[addr + 3]] = resultAsNum;
            else codes[addr + 3] = resultAsNum;
        }

        void Reset() => Output = 0;

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
