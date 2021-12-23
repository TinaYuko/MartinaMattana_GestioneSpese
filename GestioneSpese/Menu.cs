using GestioneSpese.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpese
{
    public class Menu
    {
        private static DBManager bl = new DBManager();

        internal static void Start()
        {
            /*
            • L 'utente deve poter:
            - Inserire nuove Spese
            - Approvare una spesa
            - Cancellare una spesa
            - Visualizzare
            - l'elenco delle Spese Approvate
            - L'elenco delle Spese di uno specifico Utente
            - Il totale delle Spese per Categoria
            */

            bool continua = true;
            Console.WriteLine("Benvenuto nella gestione interna delle spese della Pittau Family!\n");
            while (continua)
            {
                Console.WriteLine("\nSi prega di premere: ");
                Console.WriteLine("[1] Per inserire nuove spese.");
                Console.WriteLine("[2] Per approvare una spesa.");
                Console.WriteLine("[3] Per cancellare una spesa.");
                Console.WriteLine("[4] Per visualizzare l'elenco delle spese approvate.");
                Console.WriteLine("[5] Per visualizzare l'elenco delle spese di un certo utente.");
                Console.WriteLine("[6] Per visualizzare il totale delle spese per categoria");
                Console.WriteLine("[0] Per uscire");
                int scelta;
                do
                {
                    Console.WriteLine("Si prega di scegliere");
                } while (!(int.TryParse(Console.ReadLine(), out scelta) && scelta >= 0 && scelta <= 7));

                switch (scelta)
                {
                    case 1:
                        InserisciSpesa();
                        break;
                    case 2:
                        ApprovaSpesa(); //Disconnected Mode
                        break;
                    case 3:
                        CancellaSpesa(); //Disconnected Mode
                        break;
                    case 4:
                        VisualizzaSpeseApprovate();
                        break;
                    case 5:
                        VisualizzareSpeseUtente();
                        break;
                    case 6:
                        VisualizzaTot();
                        break;
                    case 0:
                        Console.WriteLine("A si biri!!");
                        continua = false;
                        break;
                    default:
                        Console.WriteLine("Scelta non valida, attenzione!");
                        break;
                }
            }
        }

        private static void CancellaSpesa()
        {
            VisualizzaSpese();
            int id;
            Console.Write("Quale spesa vuoi cancellare? Inserisci l'id: ");
            while (!(int.TryParse(Console.ReadLine(), out id) && id > 0))
            {
                Console.WriteLine("Valore errato. Riprova:");
            }

            bool esito = bl.DeleteSpesa(id);

            if (esito)
                Console.WriteLine("Spesa cancellata correttamente!");
            else
                Console.WriteLine("Oops, abbiamo trovato un problema!");

        }

        private static void VisualizzaSpese()
        {
            List<Spese> spese = bl.GetAllSpese();
            if (spese.Count == 0)
            {
                Console.WriteLine("Non ci sono spese");
            }
            else
            {
                foreach (var item in spese)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        private static void VisualizzaTot()
        {
            
           bl.GetTotaleByCategory();
       
        }

        private static void ApprovaSpesa()
        {
            VisualizzaSpeseDaApprovare();
            int id;
            Console.Write("Quale spesa vuoi approvare? Inserisci l'id: ");
            while (!(int.TryParse(Console.ReadLine(), out id) && id > 0))
            {
                Console.WriteLine("Valore errato. Riprova:");
            }

            bool esito = bl.ApprovaSpesa(id);

            if (esito)
                Console.WriteLine("Spesa approvata correttamente!");
            else
                Console.WriteLine("Oops, abbiamo trovato un problema!");

        }

        private static void VisualizzaSpeseDaApprovare()
        {
            List<Spese> speseDaApprovare = bl.GetSpeseDaApprovare();
            if (speseDaApprovare.Count == 0)
            {
                Console.WriteLine("Non ci sono spese da approvare!");
            }
            else
            {
                foreach (var item in speseDaApprovare)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        private static void InserisciSpesa()
        {
            
            Console.WriteLine("Per quale categoria desideri aggiungere una spesa?");
            VisualizzaCategorie();
            int idCat;
            Console.Write("Inserire l'Id: ");
            while (!(int.TryParse(Console.ReadLine(), out idCat) && idCat > 0))
            {
                Console.WriteLine("Valore errato. Riprova:");
            }
            DateTime data;
            Console.Write("Inserire data: ");
            while (!(DateTime.TryParse(Console.ReadLine(), out data)))
            {
                Console.WriteLine("Data non valida!");
            }
            Console.Write("Inserisci descrizione: ");
            string descrizione = Console.ReadLine();

            decimal importo;
            Console.Write("Inserire importo: ");
            while (!(decimal.TryParse(Console.ReadLine(), out importo) && importo > 0))
            {
                Console.WriteLine("Valore errato!!");
            }
            Console.Write("Inserisci utente: ");
            string utente=Console.ReadLine();

            Spese spesa = new Spese();

            spesa.CategoriaId = idCat;
            spesa.Data = data;
            spesa.Descrizione = descrizione;
            spesa.Importo = importo;
            spesa.Utente = utente;
            spesa.Approvato = false;

            bool esito = bl.AddSpesa(spesa);

            if (esito)
            {
                Console.WriteLine("Spesa aggiunta correttamente! Ecco un riepilogo:");
                Console.WriteLine(spesa.ToString());
            }
            else
                Console.WriteLine("Oops, abbiamo trovato un problema!");
        }

        private static void VisualizzaCategorie()
        {
            List<Categorie> categorie = bl.GetAllCategorie();
            if (categorie.Count == 0)
            {
                Console.WriteLine("Non ci sono categorie");
            }
            else
            {
                foreach (var item in categorie)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }

        private static void VisualizzareSpeseUtente()
        {
            Console.WriteLine("Di quale utente vuoi vedere le spese?");
            string utente = Console.ReadLine();
            List<Spese> speseUtente = bl.GetSpeseUtente(utente);
            foreach (var item in speseUtente)
            {
                Console.WriteLine(item.ToString());
            }
        }


        private static void VisualizzaSpeseApprovate()
        {
            List<Spese> speseApprovate = bl.GetSpeseApprovate();
            if (speseApprovate.Count == 0)
            {
                Console.WriteLine("Non abbiamo spese approvate");
            }
            else
            {
                foreach (var item in speseApprovate)
                {
                    Console.WriteLine(item.ToString());
                }
            }
        }
    }
}
