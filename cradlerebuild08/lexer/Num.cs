namespace Сradle.lexer
{
    public class Num : Token
    {
        int value;     
        public Num(int value) : base (Tag.NUM)
        {
            this.value = value;
        }
        public override string GetValue()
        {
            return "" + value;
        }
    }
}
