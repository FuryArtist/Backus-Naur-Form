using System;
using Сradle.lexer;

#region Известные проблемы
//
#endregion

namespace Сradle.parser
{
    public class Parser
    {
        private Lexer lexer;
        private Token token;
        public string output;
        int label;  // Счетчик Jmp

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
            NextToken(); // Прочитать первый токен
        }

        void NextToken()
        {
            token = lexer.GetToken();
        }
        void Match(int tag)
        {
            if (token.tag == tag)
                NextToken();
            else Error("syntax error");
        }
        void Error(string message)
        {
            throw new Exception("Error at line " + lexer.line + ": " + message);
        }
        #region Блочные конструкции
        public void Program()
        {
            Block();
        }
        void Block()
        {
            Match('{');
            while (token.tag != '}')
            {
                switch (token.tag)
                {
                    case Tag.IF: If(); break;
                    case Tag.ID: Assign(); break;
                    case '{': Block(); break;
                }
            }
            Match('}'); // Проверить конец блока
        }
        void Assign()
        {
            string name = token.GetValue();
            NextToken();
            Match('=');
            Expression();
            output += "LEA " + name + "(PC), A0\n";
            output += "MOVE D0, (A0)\n";
            Match(';'); // Проверить конец выражения
        }
        #endregion
        #region Арифметика
        void Expression()
        {
            Term();
            while (token.tag == '+' || token.tag == '-')
            {
                output += ("MOVE D0, -(SP)\n");
                switch (token.tag)
                {
                    case '+': Add(); break;
                    case '-': Subtract(); break;
                }
            }
        }
        void Term()
        {
            Unary();
            while (token.tag == '*' || token.tag == '/')
            {
                output += "MOVE D0, -(SP)\n";
                switch (token.tag)
                {
                    case '*': Multiply(); break;
                    case '/': Divide(); break;
                }
            }
        }
        void Unary()
        {
            //if (token.tag == '+')
            //    Match('+');
            if (token.tag == '-')
            {
                Match('-');
                if (token.tag == Tag.NUM)
                {
                    output += "MOVE #-" + token.GetValue() + ", D0\n";
                    NextToken();
                }
                else // Для Tag.ID
                {
                    Factor();
                    output += "NEG D0\n";
                }
            }
            //else if (token.tag == '!') // Возможна ошибка
            //{
            //    Match('!');
            //    BoolFactor(); // Factor()
            //    output += "EOR #-1, D0\n";
            //}
            else Factor();  // Если унарной операции не обнаружено 
        }
        void Factor()
        {
            switch (token.tag)
            {
                case '(':
                    Match('(');
                    Expression();
                    Match(')');
                    break;
                case Tag.NUM:
                    output += "MOVE #" + token.GetValue() + ", D0\n";
                    NextToken();
                    break;
                case Tag.ID:
                    output += "MOVE " + token.GetValue() + "(PC), D0\n";
                    NextToken();
                    break;
                case Tag.TRUE: // Позволяет складывать int с bool
                    output += "MOVE #-1, D0\n";
                    NextToken();
                    break;
                case Tag.FALSE:
                    output += "CLR D0\n";
                    NextToken();
                    break;
                default:
                    Error("syntax error");
                    break;
            }
        }
        #endregion
        #region Логика
        void BoolExpression()
        {
            BoolTerm();
            while (token.tag == '|')
            {
                output += "MOVE D0, -(SP)\n";
                Or();
            }
        }
        void BoolTerm()
        {
            NotFactor(); //BoolFactor()
            while (token.tag == '&')
            {
                output += "MOVE D0, -(SP)\n";
                And();
            }
        }
        void NotFactor()
        {
            if (token.tag == '!')
            {
                Match('!');
                BoolFactor();
                output += "EOR #-1, D0\n";
            }
            else BoolFactor();
        }
        void BoolFactor()
        {
            Relation();
            while (token.tag == Tag.EQ || token.tag == Tag.NE)
            {
                output += "MOVE D0, -(SP)\n";
                switch (token.tag)
                {
                    case Tag.EQ: Equals(); break;
                    case Tag.NE: NotEquals(); break;
                }
            }
            //switch (token.tag)
            //{
            //    case Tag.TRUE:
            //        output += "MOVE #-1, D0\n";
            //        NextToken();
            //        break;
            //    case Tag.FALSE:
            //        output += "CLR D0\n";
            //        NextToken();
            //        break;
            //    default: Relation(); break; // Возможна ошибка
            //}
        }      
        void Relation()
        {
            Expression();
            switch (token.tag)
            {
                //case Tag.EQ:
                //    output += "MOVE D0, -(SP)\n";
                //    Equals();
                //    break;
                //case Tag.NE:
                //    output += "MOVE D0, -(SP)\n";
                //    NotEquals();
                //    break;
                case '<':
                    output += "MOVE D0, -(SP)\n";
                    Less();
                    break;
                case '>':
                    output += "MOVE D0, -(SP)\n";
                    Greater();
                    break;
            }
        }
        #endregion
        #region Операции
        // Убрать в отдельный класс?
        void Equals()
        {
            Match(Tag.EQ);
            Relation(); //Expression();
            output += "CMP (SP)+, D0\n";
            output += "SEQ D0\n";
        }
        void NotEquals()
        {
            Match(Tag.NE);
            Relation(); //Expression();
            output += "CMP (SP)+, D0\n";
            output += "SNE D0\n";
        }
        void Less()
        {
            Match('<');
            Expression();
            output += "CMP (SP)+, D0\n";
            output += "SGE D0\n";
        }
        void Greater()
        {
            Match('>');
            Expression();
            output += "CMP (SP)+, D0\n";
            output += "SLE D0\n";
        }        
        void Or()
        {
            Match('|');
            BoolTerm();
            output += "OR (SP)+, D0\n";
        }
        void And()
        {
            Match('&');
            BoolFactor(); // NotFactor()
            output += "AND (SP)+, D0\n";
        }
        void Add()
        {
            Match('+');
            Term();
            output += "ADD (SP)+, D0\n";
        }
        void Subtract()
        {
            Match('-');
            Term();
            output += "SUB (SP)+, D0\n";
            output += "NEG D0\n";
        }
        void Multiply()
        {
            Match('*');
            Factor();
            output += "MULS (SP)+, D0\n";
        }
        void Divide()
        {
            Match('/');
            Factor();
            output += "MOVE (SP)+, D1\n";
            output += "EXS.L D0\n";
            output += "DIVS D1, D0\n";
        }
        #endregion
        #region Условные конструкции
        string NewLabel()
        {
            int temp = label;
            label++;
            return temp.ToString();
        }
        void PostLabel(string count)
        {
            output += "L" + count + ":\n";
        }
        
        void If()
        {
            Match(Tag.IF);
            // Блок условия
            Match('(');
            BoolExpression();
            Match(')');
            // Блок тела
            string L1 = NewLabel(), L2 = L1;
            output += "BEQ L" + L1 + "\n";
            Block();
            if (token.tag == Tag.ELSE)
            {
                Match(Tag.ELSE);
                L2 = NewLabel();
                output += "BRA L" + L2 + "\n";
                PostLabel(L1);
                Block();
            }
            PostLabel(L2);
        }
        #endregion
    }
}