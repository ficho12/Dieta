using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dieta
{
    [Serializable]
    public class Comida
    {
        public string comida { get; set; }

        public double calorias { get; set; }

        public Comida(string comida, double calorias)
        {
            this.comida = comida;
            this.calorias = calorias;
        }

    }

    [Serializable]
    public class Fecha 
    {
        public DateTime fecha { set; get; }
        

        public double totalCalorias { set; get; }

        public List<Comida> Comidas { set; get; }

        public Fecha(DateTime fecha)
        {
            this.Comidas = new List<Comida>();
            this.fecha = fecha.Date;
            this.totalCalorias = 0;
        }

    }
}
