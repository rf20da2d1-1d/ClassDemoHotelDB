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
        // findes i SQL server objekt browser -> database -> properties -> ConnectionString 
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

            Guest nyGuest = new Guest(55, "Peter", "Roskilde");
            Console.WriteLine("guest oprettet " + AddGuest(nyGuest));
            Console.WriteLine("guest findes oprettet " + AddGuest(new Guest(5, "Peter", "Roskilde")));

            nyGuest.Address = "4000 Roskilde";
            nyGuest.Name = "Peter Levinsky";
            Console.WriteLine("Guest opdateret " + UpdateGuest(nyGuest));
            Console.WriteLine("guest 55 = " + GetGuestsbyId(55));

            try
            {
                Guest guest = DeleteGuest(55);
                Console.WriteLine("Slettet " + guest);
            }
            catch (KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }

            Guest guestFindesIkke = new Guest(155, "Peter2", "Roskilde2");
            Console.WriteLine("Guest opdateret " + UpdateGuest(guestFindesIkke));
        }


        private const string GetAllSQL = "select * from Guest";
        public List<Guest> GetAllGuests()
        {
            List<Guest> liste = new List<Guest>();

            // opretter en forbindelse til databasen (lokalt eller i skyen afhængig af connectionstring)
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                // laver en SQL kommando som kan sendes til databasen
                using (SqlCommand sql = new SqlCommand(GetAllSQL, conn))
                {
                    // her sendes sql kommando til database og der kommer et svar
                    SqlDataReader reader = sql.ExecuteReader();

                    // læser samtlige rækker i svaret fra databasen
                    while (reader.Read())
                    {
                        // mapper een række til eet objekt
                        Guest g = MakeGuest(reader);
                        liste.Add(g);
                    }
                }
            }

            return liste;
        }


        // sql request med en parameter (@ID) til at indsætte et id 
        private const string GetGuestByIdSQL = "select * from Guest where Guest_No = @ID";
        public Guest GetGuestsbyId(int id)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand(GetGuestByIdSQL, conn))
                {
                    // giver parameteren @ID en konkret værdi id
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

                    // her sendes sql kommando (insert,delete eller update) til databasen
                    // svaret er antal rækker der er berørt af denne sql request fx. 0 = ingen slettet 1 = en er slettet
                    int rowsAffected = sql.ExecuteNonQuery();

                    if (rowsAffected == 1)
                    {
                        return g;
                    }
                }
            }
            throw new KeyNotFoundException("Der var ingen med id = " + id);
        }

        private const String AddGuestSQL = "insert into Guest (Guest_No, Name, Address) values (@ID, @NAME, @ADDRESS)";
        private bool AddGuest(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand(AddGuestSQL, conn))
                {
                    sql.Parameters.AddWithValue("@ID", guest.Id);
                    sql.Parameters.AddWithValue("@NAME", guest.Name);
                    sql.Parameters.AddWithValue("@ADDRESS", guest.Address);

                    // her sendes sql kommando (insert,delete eller update) til databasen
                    // svaret er antal rækker der er berørt af denne sql request fx. 0 = ingen slettet; 1 = en er slettet
                    try
                    {
                        int rowsAffected = sql.ExecuteNonQuery();
                        return true;
                    }
                    catch (Exception e)
                    {
                        return false;
                    }
                }
            }
        }

        private const String UpdateGuestSQL = "update Guest set Guest_No =@ID, Name = @NAME, Address = @ADDRESS where Guest_No = @GID";
        private bool UpdateGuest(Guest guest)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand sql = new SqlCommand(UpdateGuestSQL, conn))
                {
                    sql.Parameters.AddWithValue("@ID", guest.Id);
                    sql.Parameters.AddWithValue("@NAME", guest.Name);
                    sql.Parameters.AddWithValue("@ADDRESS", guest.Address);
                    sql.Parameters.AddWithValue("@GID", guest.Id);


                    // her sendes sql kommando (insert,delete eller update) til databasen
                    // svaret er antal rækker der er berørt af denne sql request fx. 0 = ingen opdateret;  1 = en er opdateret
                    int rowsAffected = sql.ExecuteNonQuery();

                    return (rowsAffected == 1);
                }
            }
        }



        // mapper een række til eet objekt
        private Guest MakeGuest(SqlDataReader reader)
        {
            Guest g = new Guest();

            /*
             * version 1
             */
            //g.Id = Convert.ToInt32(reader["Guest_No"]);
            //g.Name = Convert.ToString(reader["Name"]);
            //g.Address = Convert.ToString(reader["Address"]);

            /*
             * version 2
             */
            g.Id = reader.GetInt32(0);
            g.Name = reader.GetString(1);
            g.Address = reader.GetString(2);

            return g; 
        }
    }
}