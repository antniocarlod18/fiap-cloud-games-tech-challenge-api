using FiapCloudGamesTechChallenge.Application.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> GenerateJwtTokenAsync(AuthRequestDto authRequestDto);
        Guid GetUserIDAsync(HttpContext context);
    }
}
