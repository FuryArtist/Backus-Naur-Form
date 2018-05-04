using System;
using System.Collections;
using System.IO;

namespace Сradle.lexer
{
    public class Lexer
    {
        StreamReader code;
        public int line = 1;
        char symbol = ' ';
        Hashtable tokens = new Hashtable();

        public Lexer(StreamReader code)
        {
            this.code = code;
            ReserveToken(new Word(Tag.TRUE, "true"));
            ReserveToken(new Word(Tag.FALSE, "false"));
            ReserveToken(new Word(Tag.IF, "if"));
            ReserveToken(new Word(Tag.ELSE, "else"));
            ReserveToken(new Word(Tag.WHILE, "while"));
            ReserveToken(new Word(Tag.DO, "do"));
            ReserveToken(new Word(Tag.BREAK, "break"));
        }
        
        void ReserveToken(Word token) // Резервирует ключевыe слова
        {
            tokens.Add(token.lexeme, token);
        }
        void GetChar()
        {
            symbol = (char)code.Read();
        }
        bool GetChar(char sign) // Выполняет опережающее чтение
        {
            GetChar();
            if (symbol != sign)
                return false;
            else
            {
                symbol = ' ';
                return true;
            }
        }

        public Token GetToken()
        {
            for (; ; GetChar()) // Пропускает пробелы, считает строки
            {
                if (symbol == ' ' || symbol == '\t' || symbol == '\r')
                    continue;
                else if (symbol == '\n')
                    line += 1;
                else break;
            }
            switch (symbol) // Для составных операторов
            {
                case '=':
                    if (GetChar('='))
                        return Word.eq;
                    else
                        return new Token('=');
                case '!':
                    if (GetChar('='))
                        return Word.ne;
                    else
                        return new Token('!');
                case '<':
                    if (GetChar('='))
                        return Word.le;
                    else
                        return new Token('<');
                case '>':
                    if (GetChar('='))
                        return Word.ge;
                    else
                        return new Token('>');
            }
            if (Char.IsDigit(symbol)) // Для чисел
            {
                string value = "";
                do
                {
                    value += Char.GetNumericValue(symbol);
                    GetChar();
                }
                while (Char.IsDigit(symbol));
                return new Num(int.Parse(value));
            }
            if (Char.IsLetter(symbol)) // Для слов
            {
                string value = "";
                do
                {
                    value += symbol;
                    GetChar();
                }
                while (Char.IsLetterOrDigit(symbol));

                Word word = (Word)tokens[value];
                if (word != null)
                    return word;
                else
                {
                    word = new Word(Tag.ID, value); // Создать новую переменную
                    tokens.Add(value, word);
                    return word;
                }
            }
            // Для одиночных символов
            Token token = new Token(symbol);
            symbol = ' ';
            return token;
        }
    }
}