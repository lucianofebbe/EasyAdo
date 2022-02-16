using EasyAdo.Console.Negocios;

namespace EasyAdo.Console
{
    public class Program
    {
        static void Main(string[] args)
        {
            var resultGetByActivedAutoConverter =
                new NEventos().GetByActivedAutoConverter(false);
            
            var resultGetByActivedManualConverter =
                new NEventos().GetByActivedManualConverter(false);
        }
    }
}
