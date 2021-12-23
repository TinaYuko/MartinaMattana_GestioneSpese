using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpese.Entities
{
    public class Spese
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string Descrizione { get; set; }
        public decimal Importo { get; set; }
        public bool Approvato { get; set; }
        public int CategoriaId { get; set; }
        public string Utente { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} - Descrizione: {Descrizione} - Data: {Data.ToShortDateString()} - Importo: {Importo} euro" +
                $" - Utente: {Utente}";
        }
    }
}
