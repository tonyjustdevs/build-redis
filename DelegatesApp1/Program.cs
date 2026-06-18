namespace DelegatesApp1;

delegate void TPDG_Shouter(string input_str);
internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello Delegates!");
        TPDG_Shouter shouter_variable = Console.WriteLine;
        shouter_variable("sups [of TPDG_Shouter shouter_variable(\"sups\")");

        Action cool_action = ()=> Console.WriteLine("yoyoyo im cool_action via lamdba function");
        Action cool_action2 = ()=> 
        {
            Console.WriteLine("yoyoyo im cool_action via lamdba function");
        };

        Action<string> cool_action3 = (input1) =>
        {
            Console.WriteLine($"hey {input1} [an Action<string> with 1 arg: '{input1}']");
        };
        cool_action();
        cool_action2();
        cool_action3("cunt!");
        Func<int> int_returner = () =>
        {
            Console.WriteLine($"paramaterless Func<int> int_retuner() run and returns (exp 69): ");
            return 69;
        };
        var val = int_returner();
        Console.WriteLine(val);

    }
}
