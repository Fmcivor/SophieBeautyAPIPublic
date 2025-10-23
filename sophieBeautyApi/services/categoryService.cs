using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using sophieBeautyApi.Models;

namespace sophieBeautyApi.services
{
    public class categoryService
    {
        private MongoClient _mongoClient;
        private IMongoCollection<category> categoryTable;

        public categoryService(MongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("SophieBeauty");
            categoryTable = database.GetCollection<category>("categories");
        }


        public async Task<List<category>> getAll()
        {
            var categories = await categoryTable.Find(c => true).ToListAsync();

            return categories;
        }

        public async Task<category> create(string name)
        {
            category categoryToAdd = new category(name);

            await categoryTable.InsertOneAsync(categoryToAdd);

            return categoryToAdd;
        }

        public async Task<bool> delete(category category)
        {
            var result = await categoryTable.DeleteOneAsync(c=> c.Id == category.Id);

            return result.DeletedCount == 1;
        }
    }

}