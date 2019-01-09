using System;

namespace Schematic.Core
{
    public class Asset
    {
        public int ID { get; set; }

        public string FileName { get; set; }

        public string ContentType { get; set; }

        public string Title { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }
    }
}