namespace Сradle.lexer
{
    public class Token
    {
        public int tag;
        public Token(int tag)
        {
            this.tag = tag;
        }
        public virtual string GetValue()
        {
            return "" + (char)tag;
        }
    }
}
