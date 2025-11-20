using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace sophieBeautyApi.Models
{
    public class treatment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public int price { get; set; }

        [Required]
        public int duration {get; set; }

        [Required]
        public string type { get; set; }

        [Required]
        [MinLength(20)]
        [MaxLength(50)]
        public string description {get; set; }

    }
}
