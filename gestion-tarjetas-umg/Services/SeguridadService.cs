using gestion_tarjetas_umg.Models.Domain;
using System.Security.Claims;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace gestion_tarjetas_umg.Services
{
    public class SeguridadService
    {
        private readonly IConfiguration _configuration;

        public SeguridadService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string generarJwt(Usuario usuario)
        {
            
            //crear informacion de usuario para el token
            var userclaims = new[]
            {
                new Claim("claveUnica", usuario.claveUnica),
                new Claim("nombreUsuario", usuario.nombreUsuario),
                new Claim("dpi", $"{usuario.Cliente!.dpi}"),
            };

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //crear el detalle del token
            var jwtConfig = new JwtSecurityToken(
                claims: userclaims,
                expires: DateTime.UtcNow.AddMinutes(120),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
        }
    }
}
