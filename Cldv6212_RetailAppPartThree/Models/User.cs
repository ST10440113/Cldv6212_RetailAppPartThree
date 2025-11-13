using System.ComponentModel.DataAnnotations;

namespace Cldv6212_RetailAppPartThree.Models
{
    
        public class User
        {
            [Key] public int Id { get; set; }


            [Required, MaxLength(64)]
            public string Username { get; set; } = default!;


            [Required, MaxLength(256), EmailAddress]
            public string Email { get; set; } = default!;


            // Store derived key (hash) and salt separately
            [Required]
            public byte[] PasswordHash { get; set; } = default!;


            [Required]
            public byte[] PasswordSalt { get; set; } = default!;


            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public bool IsActive { get; set; } = true;

            public string Role { get; set; }
        }
    }