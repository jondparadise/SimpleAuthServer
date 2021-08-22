using Newtonsoft.Json;
using SimpleAuthServer.API.Models;
using SimpleAuthServer.API.Models.Configuration;
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
        private readonly FileStorageConfiguration config;

        public FileSystemDataService(FileStorageConfiguration configuration) 
        {
            this.config = configuration;
        }

        public List<RefreshToken> RetrieveRefreshTokens()
        {
            return LoadObjectsFromFile<List<RefreshToken>>(config.RefreshTokensFilepath);
        }

        public List<AuthorizedUser> RetrieveUsers()
        {
            return LoadObjectsFromFile<List<AuthorizedUser>>(config.UsersFilepath);
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
            this.SaveObjectToFile(tokens, config.RefreshTokensFilepath);
        }

        public void StoreUsers(IEnumerable<AuthorizedUser> users)
        {
            this.SaveObjectToFile(users, config.UsersFilepath);
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
