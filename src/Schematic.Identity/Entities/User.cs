using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Schematic.Identity;

namespace Schematic.Identity
{
    [SchematicUser]
    public class User : ISchematicUser
    {
        public User()
        {
            Roles = new List<UserRole>();
        }

        public int ID { get; set; }

        public string FullName { get; set; }
        
        private string _forenames;
        [Required(ErrorMessage = "A first name is required")]
        [Display(Name = "First name(s)")]
        public string Forenames
        {
            get => _forenames;
            set
            {
                _forenames = value;
                UpdateFullName();
            }
        }

        private string _surnames;
        [Display(Name = "Surname")]
        public string Surnames 
        {
            get => _surnames;
            set
            {
                _surnames = value;
                UpdateFullName();
            }
        }

        [Required(ErrorMessage = "You must provide an e-mail address")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "E-mail")]
        public string Email { get; set; }

        public string PassHash { get; set; }

        private DateTime? _dateCreated;
        public DateTime DateCreated
        {
            get => _dateCreated ?? DateTime.UtcNow;
            set => _dateCreated = value;
        }

        public int CreatedBy { get; set; }

        public List<UserRole> Roles { get; set; }

        private void UpdateFullName()
        {
            FullName = Forenames + " " + Surnames;
        }
    }
}