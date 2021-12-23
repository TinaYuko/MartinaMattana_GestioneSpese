﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpese.Entities
{
    public class Categorie
    {
        public int Id { get; set; }
        public string Categoria { get; set; }

        public override string ToString()
        {
            return $" Id: {Id} - Categoria: {Categoria}";
        }
    }
}
