using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Data
{
    public static class DatosIniciales
    {
        public const string RolAdministrador = "Administrador";
        public const string RolUsuario = "Usuario";

        public static async Task InicializarAsync(IServiceProvider servicios)
        {
            var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = servicios.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var rol in new[] { RolAdministrador, RolUsuario })
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            const string correoAdmin = "admin@biblioteca.sv";

            var administrador = await userManager.FindByEmailAsync(correoAdmin);

            if (administrador == null)
            {
                administrador = new IdentityUser
                {
                    UserName = "admin",
                    Email = correoAdmin,
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(administrador, "Admin123");
            }

            if (!await userManager.IsInRoleAsync(administrador, RolAdministrador))
            {
                await userManager.AddToRoleAsync(administrador, RolAdministrador);
            }

            foreach (var usuario in userManager.Users.ToList())
            {
                var roles = await userManager.GetRolesAsync(usuario);

                if (roles.Count == 0)
                {
                    await userManager.AddToRoleAsync(usuario, RolUsuario);
                }
            }
        }
    }
}
