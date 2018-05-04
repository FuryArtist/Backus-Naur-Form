namespace Сradle.lexer
{
    public class Word : Token
    {
        public string lexeme;
        public Word(int tag, string value) : base(tag)
        {
            lexeme = value;
        }
        public override string GetValue()
        {
            return "" + lexeme;
        }
        public static /*readonly*/ Word
        eq = new Word(Tag.EQ, "=="),
        ne = new Word(Tag.NE, "!="),
        le = new Word(Tag.LE, "<="),
        ge = new Word(Tag.GE, ">=");
    }
}
