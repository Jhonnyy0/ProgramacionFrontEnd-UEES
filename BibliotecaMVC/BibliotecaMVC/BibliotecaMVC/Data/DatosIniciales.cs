using Microsoft.AspNetCore.Identity;

namespace BibliotecaMVC.Data
{
    public static class DatosIniciales
    {
        public const string RolAdmin = "Admin";
        public const string RolBibliotecario = "Bibliotecario";
        public const string RolUsuario = "Usuario";

        public const string RolesGestion = RolAdmin + "," + RolBibliotecario;

        public static async Task InicializarAsync(IServiceProvider servicios)
        {
            var roleManager = servicios.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = servicios.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { RolAdmin, RolBibliotecario, RolUsuario };

            foreach (var rol in roles)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                }
            }

            await CrearUsuarioAsync(userManager, "admin", "admin@biblioteca.sv", "Admin123", RolAdmin);
            await CrearUsuarioAsync(userManager, "bibliotecario", "bibliotecario@biblioteca.sv", "Biblio123", RolBibliotecario);
            await CrearUsuarioAsync(userManager, "lector", "lector@biblioteca.sv", "Lector123", RolUsuario);

            foreach (var usuario in userManager.Users.ToList())
            {
                var rolesDelUsuario = await userManager.GetRolesAsync(usuario);

                var rolesInvalidos = rolesDelUsuario.Where(r => !roles.Contains(r)).ToList();

                if (rolesInvalidos.Count > 0)
                {
                    await userManager.RemoveFromRolesAsync(usuario, rolesInvalidos);
                    rolesDelUsuario = await userManager.GetRolesAsync(usuario);
                }

                if (rolesDelUsuario.Count == 0)
                {
                    await userManager.AddToRoleAsync(usuario, RolUsuario);
                }
            }
        }

        private static async Task CrearUsuarioAsync(
            UserManager<IdentityUser> userManager,
            string nombreUsuario,
            string correo,
            string password,
            string rol)
        {
            var usuario = await userManager.FindByEmailAsync(correo);

            if (usuario == null)
            {
                usuario = new IdentityUser
                {
                    UserName = nombreUsuario,
                    Email = correo,
                    EmailConfirmed = true
                };

                var resultado = await userManager.CreateAsync(usuario, password);

                if (!resultado.Succeeded)
                {
                    return;
                }
            }

            if (!await userManager.IsInRoleAsync(usuario, rol))
            {
                await userManager.AddToRoleAsync(usuario, rol);
            }
        }
    }
}
