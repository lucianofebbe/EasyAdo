using EasyAdo.Console.Modelos;
using EasyAdo.Console.Repositorios;
using System;
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

        /// <summary>
        /// Caso não queira utilizar os métodos padrão do contexto e ou
        /// o modelo nao corresponda a tabela em especifico
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int ManualInsert(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.ManualInsert(item);
                return result;
            }
        }


        /// <summary>
        /// Métodos Default do Context.
        /// Somente utilizar, caso o model que foi passado no parametro da classe
        /// seja identico a sua tabela correspondente na base de dados
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public List<Eventos> AutoGetAll(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.GetAll();
                return result;
            }
        }

        public int AutoInsert(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Insert(item);
                return result;
            }
        }

        public bool AutoUpdate(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Update(item, new Tuple<string, string>("Id", "2"));
                return result;
            }
        }

        public bool AutoDelete(Eventos item)
        {
            using (REventos rEventos = new REventos())
            {
                var result = rEventos.Delete(item, new Tuple<string, string>("Id", "1"));
                return result;
            }
        }
    }
}
