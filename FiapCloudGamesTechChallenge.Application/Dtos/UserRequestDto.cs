namespace FiapCloudGamesTechChallenge.Application.Dtos;

public class UserRequestDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
}

public class UserUpdateRequestDto : UserRequestDto
{
    public string Password { get; set; } = default!;
}