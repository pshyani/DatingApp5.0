using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class RegisterDTO
    { 
        [Required]
        public string UserName { get; set; }

        [Required]  
        public string Password { get; set; }       
        
    }
}