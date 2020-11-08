public class TypePrim : Type
{

    Token kind;
    public TypePrim(Token kind)
    {
        this.kind = kind;
    }

    public override void printNode(ASTPrinter p)
    {
        string strkind = kind.Type.ToString();
        p.printText("kind", strkind);
    }
}