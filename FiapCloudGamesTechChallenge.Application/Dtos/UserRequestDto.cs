namespace FiapCloudGamesTechChallenge.Application.Dtos;

public class UserRequestDto
{
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

public class UserUpdateRequestDto : UserRequestDto
{
    public string Password { get; set; } = "";
}