using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AgarioServer.Game
{
    public class Database
    {
        private static Database _instance;
        private MySqlConnection _connection;

        private Database()
        {}

        public void Connect()
        {
            String connString = String.Format("Server={0}; Database={1}; Uid={2}; Pwd={3};", new String[] { 
                Config.MySqlHost,
                Config.MySqlName,
                Config.MySqlUser,
                Config.MySqlPass,
            });

            this._connection = new MySqlConnection(connString);
            this._connection.StateChange += (s, e) =>
            {
                if (e.CurrentState == System.Data.ConnectionState.Closed)
                {
                    Log.Error("Error with the connection to the database");
                    Log.Info("Reconnecting to the database");
                    this.Connect();
                }
            };

            try
            {
                this._connection.Open();
            } 
            catch(MySqlException ex)
            {
                Log.Fatal("Error connecting to the database: " + ex.Message);
            }
        }

        public static Database Instance
        {
            get
            {
                if (Database._instance == null)
                    Database._instance = new Database();
                return Database._instance;
            }
        }

        public void AddPlayer(UInt32 sid)
        {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET players=players+1 WHERE id=@Id";
            cmd.Parameters.AddWithValue("@Id", sid);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error updating the amount of players: " + ex.Message);
            }
        }
        
        public void RemovePlayer(UInt32 sid)
        {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET players=players-1 WHERE id=@Id AND players>=1";
            cmd.Parameters.AddWithValue("@Id", sid);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error updating the amount of players: " + ex.Message);
            }
        }
        
        public void AddRealm(UInt32 sid)
        {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET realms=realms+1 WHERE id=@Id";
            cmd.Parameters.AddWithValue("@Id", sid);
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error adding realm: " + ex.Message);
            }
        }
        
        public void RemoveRealm(UInt32 sid)
        {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET realms=realms-1 WHERE id=@Id AND realms>=1";
            cmd.Parameters.AddWithValue("@Id", sid);
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error removing the realm: " + ex.Message);
            }
        }

        public void ServerStatus(UInt32 sid, String listen, Boolean status)
        {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET host=@Host, online=@Status WHERE id=@Id";
            cmd.Parameters.AddWithValue("@Host", listen);
            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@Id", sid);
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error updating the server status: " + ex.Message);
            }
        }

        public void CleanServerStatus(UInt32 sid) {
            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "UPDATE server SET host=NULL, online=0, realms=0 WHERE id=@Id";
            cmd.Parameters.AddWithValue("@Id", sid);
            
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error when cleaning the server status: " + ex.Message);
            }
        }

        public String GetServerRegion(UInt32 sid)
        {
            String region = String.Empty;

            MySqlCommand cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT r.name FROM server s JOIN region r ON r.id=s.region_id WHERE s.id=@Id";
            cmd.Parameters.AddWithValue("@Id", sid);
            MySqlDataReader reader;

            try
            {
                reader = cmd.ExecuteReader();

                if (reader.Read())
                    region = reader.GetString("name");

                reader.Close();
            }
            catch (MySqlException ex)
            {
                Log.Error("Error getting server region: " + ex.Message);
            }

            return region;
        }
    }
}
