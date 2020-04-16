using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SblendersAPI.Models
{
    public class Restaurante
    {
        public string restauranteNome;
        public int restauranteID;
        public decimal restauranteLat;
        public decimal restauranteLong;

        public Restaurante(string restauranteNome, int restauranteID, decimal restauranteLat, decimal restauranteLong)
        {
            this.restauranteNome = restauranteNome;
            this.restauranteID = restauranteID;
            this.restauranteLat = restauranteLat;
            this.restauranteLong = restauranteLong;
        }
    }
}
