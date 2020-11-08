using System.Collections.Generic;

public class ASTPrinter
{
    private int identLevel;

    public ASTPrinter()
    {
        this.identLevel = 0;
    }

    public void print(string title, System.Object obj)
    {
        if(obj is TMNode)
        {
            printNode(title, (TMNode) obj);
        }
        else if(obj is Token)
        {
            printToken(title, (Token) obj);
        }
        else if(obj is List<Declaration>){
            printList(title, (List<Declaration>) obj);
        }
        else if(obj is List<Parameter>){
            printList(title, (List<Parameter>) obj);
        }
        else if(obj is List<Statement>){
            printList(title, (List<Statement>) obj);
        }
        else if(!(obj is System.Object))
        {
            printText(title, "NIL");
        }
        else
        {
            System.Console.WriteLine("Invalid print argument {0}", obj.GetType());
        }
    }

    private void printNode(string title, TMNode node)
    {
        printText(title, node.GetType().Name);
        this.identLevel++;
        node.printNode(this);
        this.identLevel--;
    }

    private void printToken(string title, Token token)
    {
        string text = token.Value.ToString();
        printText(title, text);
    }

    public void printText(string title, string text)
    {
        string prefix = new string(' ', identLevel);
        System.Console.WriteLine("{0}{1}: {2}", prefix, title, text);
    }

    public void printList<T>(string title, List<T> list)
    {
        if(list.Count == 0)
        {
            printText(title, "[]");
        }

        foreach(var elem in list)
        {
            string elem_title = title;
            print(elem_title, elem);
        }
    }
}
