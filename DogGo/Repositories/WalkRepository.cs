using DogGo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DogGo.Repositories
{
    public class WalkRepository : IWalkRepository
    {
        private readonly IConfiguration _config;

        public WalkRepository(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        public List<Walk> GetWalksByWalkerId(int walkerId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id, w.[Date], w.Duration, w.WalkerId, o.id AS OwnerId, o.[Name] AS OwnerName, d.Id AS DogId, d.[Name] AS DogName
                                          FROM Walks w
                                          LEFT JOIN Dog d ON w.DogId = d.Id
                                          LEFT JOIN Owner o ON d.OwnerId = o.Id
                                         WHERE w.WalkerId = @id
                                         ORDER BY OwnerName";
                    cmd.Parameters.AddWithValue("@id", walkerId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Walk> walks = new List<Walk>();

                    while (reader.Read())
                    {
                        Walk walk = new Walk()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                            Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                            WalkerId = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                            DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                            Dog = new Dog
                            {
                                Name = reader.GetString(reader.GetOrdinal("DogName")),
                            },
                            Owner = new Owner
                            {
                                Name = reader.GetString(reader.GetOrdinal("OwnerName"))
                            }
                          
                        };

                        walks.Add(walk);
                    }
                    reader.Close();
                    return walks;
                }
            }
        }

        public void AddWalk(Walk newWalk)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Walks (Date, Duration, WalkerId, DogId)
                                        OUTPUT INSERTED.ID
                                        VALUES (@Date, @Duration, @WalkerId, @DogId)";
                    cmd.Parameters.AddWithValue("@Date", newWalk.Date);
                    cmd.Parameters.AddWithValue("@Duration", newWalk.Duration);
                    cmd.Parameters.AddWithValue("@WalkerId", newWalk.WalkerId);
                    cmd.Parameters.AddWithValue("@DogId", newWalk.DogId);

                    newWalk.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

    }
}
