using FiapCloudGamesTechChallenge.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Application.Dtos
{
    public class GameRequestDto
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Developer { get; set; }
        public string? Distributor { get; set; }
        public IList<string> GamePlatforms { get; set; } = [];
        public string? GameVersion { get; set; }
        public bool Available { get; set; }
    }
}
