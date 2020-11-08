public class Parameter : TMNode
{
    private Type retType;
    private Token name;

    public Parameter(Type retType, Token name)
    {
        this.retType = retType;
        this.name = name;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("Type", retType);
        p.print("Name", name);
    }
}