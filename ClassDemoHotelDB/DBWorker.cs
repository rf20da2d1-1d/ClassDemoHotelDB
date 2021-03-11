using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using HotelModelLib.model;

namespace ClassDemoHotelDB
{
    public class DBWorker
    {
        private const String ConnectionString = @"Data Source=(localdb)\ProjectsV13;Initial Catalog=HotelDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public DBWorker()
        {
        }

        public void Start()
        {
            List<Guest> list = GetAllGuests();
            foreach (Guest g in list)
            {
                Console.WriteLine(g);
            }

            try
            {
                Guest guest = DeleteGuest(50);
                Console.WriteLine("Slettet " + guest);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public List<Guest> GetAllGuests()
        {
            List<Guest> liste = new List<Guest>();

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand("select * from Guest", conn))
                {
                    SqlDataReader reader = sql.ExecuteReader();
                    while (reader.Read())
                    {
                        Guest g = MakeGuest(reader);
                        liste.Add(g);
                    }
                }
            }

            return liste;
        }


        public Guest GetGuestsbyId(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand("select * from Guest where Guest_No = @ID", conn))
                {
                    sql.Parameters.AddWithValue("@ID", id);

                    SqlDataReader reader = sql.ExecuteReader();
                    if (reader.Read())
                    {
                        return MakeGuest(reader);
                    }
                }
            }

            throw new KeyNotFoundException("Der var ingen med id = " + id);
        }


        private const String DeleteGuestSQL = "delete from Guest where Guest_No = @ID";
        private Guest DeleteGuest(int id)
        {
            Guest g = GetGuestsbyId(id);

            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand(DeleteGuestSQL, conn))
                {
                    sql.Parameters.AddWithValue("@ID", id);

                    int rowsAffected = sql.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return g;
                    }
                }
            }
            throw new KeyNotFoundException("Der var ingen med id = " + id);
        }

        private Guest MakeGuest(SqlDataReader reader)
        {
            Guest g = new Guest();

            //g.Id = Convert.ToInt32(reader["Guest_No"]);
            //g.Name = Convert.ToString(reader["Name"]);
            //g.Address = Convert.ToString(reader["Address"]);

            g.Id = reader.GetInt32(0);
            g.Name = reader.GetString(1);
            g.Address = reader.GetString(2);

            return g; 
        }
    }
}