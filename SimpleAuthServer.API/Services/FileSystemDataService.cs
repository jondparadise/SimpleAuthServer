using Newtonsoft.Json;
using SimpleAuthServer.API.Models;
using SimpleAuthServer.API.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleAuthServer.API.Services
{
    public class FileSystemDataService : IUserStore
    {
        const string TokenFileName = "tokens.json";
        const string UserFileName = "users.json";

        public FileSystemDataService() { }

        public List<RefreshToken> RetrieveRefreshTokens()
        {
            return LoadObjectsFromFile<List<RefreshToken>>(TokenFileName);
        }

        public List<AuthorizedUser> RetrieveUsers()
        {
            return LoadObjectsFromFile<List<AuthorizedUser>>(UserFileName);
        }

        private T LoadObjectsFromFile<T>(string filename)
        {
            T retVal = default;
            string jsonContent = string.Empty;
            try
            {
                using (var fStream = new FileStream(filename, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fStream))
                    {
                        jsonContent = reader.ReadToEnd();
                    }
                }

                if (!string.IsNullOrWhiteSpace(jsonContent))
                {
                    var parseResult = JsonConvert.DeserializeObject<T>(jsonContent);
                    if (parseResult != null)
                    {
                        retVal = parseResult;
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Attempt to load file {filename} failed.");
            }
            
            return retVal;
        }

        public void StoreRefreshTokens(IEnumerable<RefreshToken> tokens)
        {
            this.SaveObjectToFile(tokens, TokenFileName);
        }

        public void StoreUsers(IEnumerable<AuthorizedUser> users)
        {
            this.SaveObjectToFile(users, UserFileName);
        }

        private void SaveObjectToFile(object itemToSave, string fileName)
        {
            using (var fStream = new FileStream(fileName, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fStream))
                {
                    writer.WriteLine(JsonConvert.SerializeObject(itemToSave));
                }
            }
        }
    }
}
