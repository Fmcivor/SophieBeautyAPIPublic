using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;
using sophieBeautyApi.Models;

namespace sophieBeautyApi.services
{
    public class treatmentService
    {
        private readonly IMongoCollection<treatment> treatmentTable;
        private readonly MongoClient _mongoClient;

        public treatmentService(MongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("SophieBeauty");
            treatmentTable = database.GetCollection<treatment>("services");
        }

        public async Task<IEnumerable<treatment>> getAll()
        {
            var treatments = await treatmentTable.Find(t => true).ToListAsync();

            return treatments;
        }

        public async Task<treatment> create(treatment newTreatment)
        {
            await treatmentTable.InsertOneAsync(newTreatment);

            return newTreatment;
        }

        public async Task<treatment?> getById(string id)
        {
            var filter = Builders<treatment>.Filter.Eq(t => t.Id, id);
            return await treatmentTable.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> update(treatment updatedTreatment)
        {
            var filter = Builders<treatment>.Filter.Eq(t => t.Id, updatedTreatment.Id);
            var result = await treatmentTable.ReplaceOneAsync(filter, updatedTreatment);
            return result.MatchedCount > 0;
        }

        public async Task<bool> delete(string id)
        {
            var filter = Builders<treatment>.Filter.Eq(t => t.Id, id);
            var result = await treatmentTable.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
    
        public async Task<IEnumerable<treatment>> getListByIds(List<string> ids)
        {

            var treatments = await treatmentTable.Find(t => ids.Contains(t.Id) == true).ToListAsync();


            return treatments;
        }
    
    }
}