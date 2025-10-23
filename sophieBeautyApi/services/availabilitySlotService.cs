using Microsoft.AspNetCore.Http.HttpResults;
using MongoDB.Driver;
using sophieBeautyApi.Models;

namespace sophieBeautyApi.services
{
    public class availablilitySlotService
    {

        private readonly IMongoCollection<availablilitySlot> _availabilitySlotTable;
        private readonly MongoClient _mongoClient;

        public availablilitySlotService(MongoClient mongoClient)
        {
            _mongoClient = mongoClient;
            var database = _mongoClient.GetDatabase("SophieBeauty");
            _availabilitySlotTable = database.GetCollection<availablilitySlot>("availabilitySlots");
        }


        public async Task<IEnumerable<availablilitySlot>> getAll()
        {
            var availabilitySlots = await _availabilitySlotTable.Find(a => true).ToListAsync();

            return availabilitySlots;
        }


        public async Task<availablilitySlot> create(availablilitySlot newSlot)
        {
            await _availabilitySlotTable.InsertOneAsync(newSlot);

            return newSlot;
        }

        public async Task<bool> bookingWithinAvailabilitySlot(DateTime apptTime)
        {
            var ukZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");

            var bookingUkFullDate = TimeZoneInfo.ConvertTimeFromUtc(apptTime, ukZone);

            var slotsOnDate = await getSlotsByDate(bookingUkFullDate);

            if (slotsOnDate == null)
            {
                return false;
            }


            var bookingTime = bookingUkFullDate.TimeOfDay;

            foreach (availablilitySlot slot in slotsOnDate)
            {


                if (bookingTime >= slot.startTime && bookingTime <= slot.endTime)
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<bool> delete(availablilitySlot slot)
        {
            var filter = Builders<availablilitySlot>.Filter.Eq(a => a.Id, slot.Id);

            var result = await _availabilitySlotTable.DeleteOneAsync(filter);

            return result.DeletedCount == 1;
        }





        public async Task<IEnumerable<availablilitySlot>> getSlotsByDate(DateTime date)
        {
            var slots = await _availabilitySlotTable.Find(a => a.date == date.ToString("yyyy-MM-dd")).ToListAsync();

            return slots;
        }



        //dev only

        public async Task<bool> deleteAll()
        {
            await _availabilitySlotTable.DeleteManyAsync(a => true);
            return true;
        }

    }
}