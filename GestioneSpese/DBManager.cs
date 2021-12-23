using GestioneSpese.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestioneSpese
{
    public class DBManager
    {
        string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GestioneSpese;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        internal List<Categorie> GetAllCategorie()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select * from Categoria";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Categorie> categorie= new List<Categorie>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var categoria= reader.GetString(1);

                Categorie c= new Categorie();
                c.Id = id;  
                c.Categoria = categoria;

                categorie.Add(c);
            }

            conn.Close();
            return categorie;
        }

        internal bool AddSpesa(Spese spesa)
        {
            SqlConnection conn= new SqlConnection(connectionString);
            conn.Open();
            string query = "Insert Spesa values(@d, @catId, @descr, @u, @i, @a)";
            SqlCommand cmd= new SqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@d", spesa.Data);
            cmd.Parameters.AddWithValue("@catId", spesa.CategoriaId);
            cmd.Parameters.AddWithValue("@descr", spesa.Descrizione);
            cmd.Parameters.AddWithValue("@u", spesa.Utente);
            cmd.Parameters.AddWithValue("@i", spesa.Importo);
            cmd.Parameters.AddWithValue("@a", spesa.Approvato);

            int righe = cmd.ExecuteNonQuery();
            if (righe >= 1) return true;
            else return false;

            conn.Close();
            
        }

        internal List<Spese> GetSpeseDaApprovare()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select * from Spesa where Approvato=0";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Spese> spese = new List<Spese>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var data = reader.GetDateTime(1);
                var categoria = reader.GetInt32(2);
                var descrizione = reader.GetString(3);
                var utente = reader.GetString(4);
                var importo = reader.GetDecimal(5);
                var approvato = reader.GetBoolean(6);

                Spese s= new Spese();
                s.Id = id;
                s.Data = data;
                s.CategoriaId = categoria;
                s.Descrizione=descrizione;
                s.Utente = utente;
                s.Importo = importo;
                s.Approvato = approvato;
                spese.Add(s);
            }

            conn.Close();
            return spese;
        }

        //Disconnected Mode
        internal bool ApprovaSpesa(int id)
        {
            DataSet spesaDS=new DataSet();
            using SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
                Console.WriteLine("Connessi al DB");
            else
                Console.WriteLine("Non connessi");

            var spesaAdapter = InizializzaData(spesaDS, conn);
            conn.Close();
            Console.WriteLine("Connessione chiusa");

            DataRow rigaDaAggiornare = spesaDS.Tables["Spesa"].Rows.Find(id);
            if (rigaDaAggiornare != null)
            { rigaDaAggiornare.SetField("Approvato", 1); }

          spesaAdapter.Update(spesaDS, "Spesa");

            return true;
        }

        internal void GetTotaleByCategory()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select c.Categoria, sum(s.Importo) as Tot from Spesa s join Categoria c on s.CategoriaId=c.Id group by c.Categoria";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Spese> spese = new List<Spese>();

            while (reader.Read())
            {

                var categoria = (string)reader["Categoria"];
                var tot = (decimal)reader["Tot"];

                Console.WriteLine($"Categoria: {categoria} - Totale: {tot} euro");
                
            }

            conn.Close();
        }

        internal List<Spese> GetAllSpese()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select * from Spesa";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Spese> spese = new List<Spese>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var data = reader.GetDateTime(1);
                var categoria = reader.GetInt32(2);
                var descrizione = reader.GetString(3);
                var utente = reader.GetString(4);
                var importo = reader.GetDecimal(5);
                var approvato = reader.GetBoolean(6);

                Spese s = new Spese();
                s.Id = id;
                s.Data = data;
                s.CategoriaId = categoria;
                s.Descrizione = descrizione;
                s.Utente = utente;
                s.Importo = importo;
                s.Approvato = approvato;
                spese.Add(s);
            }

            conn.Close();
            return spese;
        }

        //Disconnected Mode
        internal bool DeleteSpesa(int id)
        {
            DataSet spesaDS = new DataSet();
            using SqlConnection conn = new SqlConnection(connectionString);

            conn.Open();
            if (conn.State == System.Data.ConnectionState.Open)
                Console.WriteLine("Connessi al DB");
            else
                Console.WriteLine("Non connessi");

            var spesaAdapter = InizializzaData(spesaDS, conn);
            conn.Close();
            Console.WriteLine("Connessione chiusa");

            DataRow rigaDaEliminare = spesaDS.Tables["Spesa"].Rows.Find(id);
            if (rigaDaEliminare != null)
            { rigaDaEliminare.Delete(); }

            spesaAdapter.Update(spesaDS, "Spesa");

            return true;
        }

        private static SqlDataAdapter InizializzaData(DataSet spesaDS, SqlConnection conn)
        {
            SqlDataAdapter spesaAdapter= new SqlDataAdapter();
            //Fill
            spesaAdapter.SelectCommand = new SqlCommand("Select * from Spesa", conn);
            spesaAdapter.MissingSchemaAction = MissingSchemaAction.AddWithKey;
            //Update
            spesaAdapter.UpdateCommand = GeneraUpdateCommand(conn);
            //Delete
            spesaAdapter.DeleteCommand = GeneraDeleteCommand(conn);


            spesaAdapter.Fill(spesaDS, "Spesa");
           return spesaAdapter;
        
        }

        private static SqlCommand GeneraDeleteCommand(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Delete from Spesa where Id=@id";

            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int, 0, "Id"));

            return cmd;
        }

        private static SqlCommand GeneraUpdateCommand(SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "Update Spesa set Data=@d, CategoriaId=@c, Descrizione=@descr, Utente=@u, Importo=@u, Approvato=1 where Id=@id";

            cmd.Parameters.Add(new SqlParameter("@d", SqlDbType.Date, 0, "Data"));
            cmd.Parameters.Add(new SqlParameter("@c", SqlDbType.Int, 0, "CategoriaId"));
            cmd.Parameters.Add(new SqlParameter("@descr", SqlDbType.VarChar, 500, "Descrizione"));
            cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.VarChar, 100, "Utente"));
            cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.Decimal, 9, "Importo"));
            cmd.Parameters.Add(new SqlParameter("@a", SqlDbType.Bit, 1, "Approvato"));
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int, 0, "Id"));

            return cmd;
        }

        internal List<Spese> GetSpeseApprovate()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select * from Spesa where Approvato=1";
            SqlCommand cmd = new SqlCommand(query, conn);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Spese> spese = new List<Spese>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var data = reader.GetDateTime(1);
                var categoria = reader.GetInt32(2);
                var descrizione = reader.GetString(3);
                var utente = reader.GetString(4);
                var importo = reader.GetDecimal(5);
                var approvato = reader.GetBoolean(6);

                Spese s = new Spese();
                s.Id = id;
                s.Data = data;
                s.CategoriaId = categoria;
                s.Descrizione = descrizione;
                s.Utente = utente;
                s.Importo = importo;
                s.Approvato = approvato;
                spese.Add(s);
            }

            conn.Close();
            return spese;
        }

        internal List<Spese> GetSpeseUtente(string? utenteToCheck)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string query = "select * from Spesa where Utente=@u";

            SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@u", utenteToCheck);

            SqlDataReader reader = cmd.ExecuteReader();
            List<Spese> spese = new List<Spese>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var data = reader.GetDateTime(1);
                var categoria = reader.GetInt32(2);
                var descrizione = reader.GetString(3);
                var utente = reader.GetString(4);
                var importo = reader.GetDecimal(5);
                var approvato = reader.GetBoolean(6);

                Spese s = new Spese();
                s.Id = id;
                s.Data = data;
                s.CategoriaId = categoria;
                s.Descrizione = descrizione;
                s.Utente = utente;
                s.Importo = importo;
                s.Approvato = approvato;
                spese.Add(s);
            }

            conn.Close();
            return spese;
        }
    }
}
