namespace UserApplication.Dtos.Request
{
    public class CreateUserDto
    {

        public CreateUserDto(string uuid, string username, string email)
        {
            Uuid = uuid;

            Username = username;

            Email = email;
        }
        
        public string Uuid { get; }
        
        public string Username { get; }
        
        public string Email { get; }
        
    }
}