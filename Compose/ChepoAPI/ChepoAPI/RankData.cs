﻿using System.ComponentModel.DataAnnotations;

namespace ChepoAPI
{
    public class RankData
    {
        [Key]
        public Guid uuid { get; set; }
        public string name { get; set; }
        public int mmr_value { get; set; }
    }
}
