using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Dependencies.IntCode
{
    public class IntCode
    {
        public int Output { get; private set; } = -1;

        public IntCode() { }

        public void Execute(List<int> codes)
        {
            var temp = codes.ToList();
            var addr = 0;

            if (!temp.Any()) return;
            Reset();

            while (true)
            {
                var operation = ReadOpCode(temp[addr]);
                if (operation.Operator == IntCodeOperator.Exit) break;

                switch (operation.Operator)
                {
                    case IntCodeOperator.Add:
                    case IntCodeOperator.Multiply:
                        Handle(operation, temp, addr);
                        break;
                    case IntCodeOperator.Input:
                    case IntCodeOperator.Output:
                    default:
                        throw new Exception("Unrecognizable opcode.");
                }

                addr += GetOffset(operation.Operator);
            }

            Output = temp[0];
        }

        void Handle(IntCodeOperation operation, List<int> codes, int addr)
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
                    return 4;
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
        Exit = 99
    }

    public enum IntCodeParamMode
    {
        Position = 0,
        Immediate = 1
    }
}
