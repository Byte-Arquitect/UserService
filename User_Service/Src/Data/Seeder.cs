using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using User_Service.Src.Models;

namespace User_Service.Src.Data
{
    public class Seeder
    {
        
        
        
        
        public static void SeedData(DataContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            SeedUsers(context, options);
        }

        
        private static void SeedUsers(DataContext context, JsonSerializerOptions options)
        {
            var result = context.Users?.Any();
            if (result is true or null) return;
            var path = "Src/Data/DataSeeders/userSeed.json";
            var usersData = File.ReadAllText(path);
            var usersList = JsonSerializer.Deserialize<List<User>>(usersData, options) ??
                throw new Exception("UsersData.json is empty");
            // Normalize the name and code of the careers
            usersList.ForEach(s =>
            {
                s.Name = s.Name.ToLower();
                s.FirstLastName = s.FirstLastName.ToLower();
                s.SecondLastName = s.SecondLastName.ToLower();
                s.CreatedAt = DateTime.SpecifyKind(s.CreatedAt, DateTimeKind.Utc);
                s.UpdatedAt = DateTime.SpecifyKind(s.UpdatedAt, DateTimeKind.Utc);
                if (s.DeletedAt.HasValue)
                {
                    s.DeletedAt = DateTime.SpecifyKind(s.DeletedAt.Value, DateTimeKind.Utc);
                }
            });

            context.Users?.AddRange(usersList);
            context.SaveChanges();
        }
    }
}