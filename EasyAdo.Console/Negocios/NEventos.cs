using EasyAdo.Console.Modelos;
using EasyAdo.Console.Repositorios;
using System.Collections.Generic;

namespace EasyAdo.Console.Negocios
{
    public class NEventos
    {
        public List<Eventos> GetByActivedAutoConverter(bool actived)
        {
            using(REventos rEventos = new REventos())
            {
                var result = rEventos.GetByActivedAutoConverter(actived);
                return result;
            }
        }

        public List<Eventos> GetByActivedManualConverter(bool actived)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.GetByActivedAutoConverter(actived);
                return result;
            }
        }
    }
}
