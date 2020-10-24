using STAN.Client;

namespace UserApplication.Dtos.Request
{
    public class DeleteUserDto
    {

        public DeleteUserDto(string uuid)
        {
            Uuid = uuid;
        }
        
        public string Uuid { get; set; }
        
    }
}