public class Token
{
    public int Line { get; private set; }
    public TokenType Type { get; private set; }
    public dynamic Value { get; private set; }

    public Token(int line, TokenType type, dynamic value)
    {
        this.Line = line;
        this.Type = type;
        this.Value = value;
    }
}