using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Models
{
    public class UcabUserAuth
    {
        /// <summary>
        /// Nombre de usuario
        /// </summary>
        /// <value>289868</value>
        public string uid { get; set; }
        /// <summary>
        /// Si fue exitosa la búsqueda
        /// </summary>
        /// <value>true</value>
        public bool success { get; set; }
        /// <summary>
        /// Contraseña
        /// </summary>
        /// <value></value>
        public string password { get; set; }
        /// <summary>
        /// Constructor vacío
        /// </summary>
        public UcabUserAuth()
        {
        }
        /// <summary>
        /// Constructor completo
        /// </summary>
        /// <param name="uid">PIDM</param>
        /// <param name="success">Si fue exitosa la búsqueda</param>
        /// <param name="password">Contraseña</param>
        public UcabUserAuth(string uid, bool success, string password)
        {
            this.uid = uid;
            this.success = success;
            this.password = password;
        }
    }
}
