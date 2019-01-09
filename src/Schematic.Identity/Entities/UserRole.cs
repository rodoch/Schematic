using System;

namespace Schematic.Identity
{
    public class UserRole
    {
        private DateTime? _dateCreated;

        public int ID { get; set; }

        public string Name { get; set; }

        public string DisplayTitle { get; set; }

        public DateTime DateCreated
        {
            get => _dateCreated ?? DateTime.UtcNow;
            set => _dateCreated = value;
        }

        public int CreatedBy { get; set; }
        
        public bool HasRole { get; set; } = false;
    }
}