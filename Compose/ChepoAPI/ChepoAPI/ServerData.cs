﻿using System.ComponentModel.DataAnnotations;

namespace ChepoAPI
{
    public class ServerData
    {
        [Key]
        public Guid uuid { get; set; }
        public string name { get; set; }
        public string ip { get; set; }
    }
}