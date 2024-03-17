using DSI;

namespace MyProgram
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            DSIStack<int> stack = new();

            stack.Add(5);
            stack.Add(20);

            Console.WriteLine($"Valor do topo apenas obtendo o valor: {stack.Peek()}");

            stack.Add(10);

            stack.Add(50);
            stack.Remove();

            Console.WriteLine($"Stack ainda tem 50? {stack.Contains(50)}");

            Console.WriteLine($"Stack ainda tem 10? {stack.Contains(10)}");

            Console.WriteLine($"Valor do topo removido obtendo o valor: {stack.Pop()}");

            for (int i = 0; i < stack.Count; i++)
            {
                Console.WriteLine($"Valor loop com indice {i}: {stack[i]}");                
            }

            foreach (int i in stack)
            {
                Console.WriteLine($"Valor foreach: {i}");
            }

            Console.WriteLine($"stack count enumerable: {stack.Count()}");

            stack.Clear();

           foreach (int i in stack)
            {
                Console.WriteLine($"Valor foreach depois do Clear: {i}");
            }

            return 0;
        }
    }
}