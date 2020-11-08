using System.Collections.Generic;

public class FnDeclaration : Declaration
{
    private Type retType;
    private Token name;
    private List<Parameter> parameters;
    private Body body;

    public FnDeclaration(Type retType, Token name, List<Parameter> parameter, Body body)
    {
        this.retType = retType;
        this.name = name;
        this.parameters = parameter;
        this.body = body;
    }

    public override void printNode(ASTPrinter p)
    {
        p.print("RetType", retType);
        p.print("Name", name);
        p.printList("Parameters", parameters);
        p.print("Body", body);
    }
}