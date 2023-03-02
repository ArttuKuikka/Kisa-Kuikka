using Kipa_plus.Data;
using Kipa_plus.Models;
using Kipa_plus.Models.DynamicAuth;
using Kipa_plus.Models.DynamicAuth.Custom;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Kipa_plus.Services
{
    
        public class RoleAccessStore : IRoleAccessStore
        {
            private readonly SqlOptions _options;
            private readonly ILogger<RoleAccessStore> _logger;
            private readonly ApplicationDbContext _context;

        public RoleAccessStore(SqlOptions options, ILogger<RoleAccessStore> logger, ApplicationDbContext context)
            {
                _options = options;
                _logger = logger;
            _context = context;
            }

            public async Task<bool> AddRoleAccessAsync(RoleAccess roleAccess)
            {
                try
                {
                    using (var conn = new SqlConnection(_options.ConnectionString))
                    {
                        const string insertCommand = "INSERT INTO RoleAccess VALUES(@RoleId, @Access, @RastiAccess)";
                        using (var cmd = new SqlCommand(insertCommand, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@RoleId", roleAccess.RoleId);

                            if(roleAccess.Controllers != null)
                        {
                            var access = JsonConvert.SerializeObject(roleAccess.Controllers);
                            cmd.Parameters.AddWithValue("@Access", access);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Access", DBNull.Value);
                        }

                           if(roleAccess.RastiAccess!= null)
                        {
                            var rastiAccess = JsonConvert.SerializeObject(roleAccess.RastiAccess);
                            cmd.Parameters.AddWithValue("@RastiAccess", rastiAccess);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@RastiAccess", DBNull.Value);
                        }

                            conn.Open();
                            var affectedRows = await cmd.ExecuteNonQueryAsync();
                            return affectedRows > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occurred while inserting access into RoleAccess table");
                    return false;
                }
            }

            public async Task<bool> EditRoleAccessAsync(RoleAccess roleAccess)
            {
                try
                {
                    int affectedRows;
                    using (var conn = new SqlConnection(_options.ConnectionString))
                    {
                        const string insertCommand = "UPDATE RoleAccess SET [Access] = @Access WHERE [RoleId] = @RoleId";
                        using (var cmd = new SqlCommand(insertCommand, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@RoleId", roleAccess.RoleId);
                            if (roleAccess.Controllers != null)
                            {
                                var access = JsonConvert.SerializeObject(roleAccess.Controllers);
                                cmd.Parameters.AddWithValue("@Access", access);
                            }
                            else
                                cmd.Parameters.AddWithValue("@Access", DBNull.Value);

                            conn.Open();
                            affectedRows = await cmd.ExecuteNonQueryAsync();
                        }
                    }

                    if (affectedRows > 0)
                        return true;

                    return await AddRoleAccessAsync(roleAccess);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occurred while editing access into RoleAccess table");
                    return false;
                }
            }

            public async Task<bool> RemoveRoleAccessAsync(string roleId)
            {
                try
                {
                    using (var conn = new SqlConnection(_options.ConnectionString))
                    {
                        const string insertCommand = "DELETE FROM RoleAccess WHERE [RoleId] = @RoleId";
                        using (var cmd = new SqlCommand(insertCommand, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@RoleId", roleId);

                            conn.Open();
                            var affectedRows = await cmd.ExecuteNonQueryAsync();

                            return affectedRows > 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occurred while deleting access from RoleAccess table");
                    return false;
                }
            }

            public async Task<RoleAccess> GetRoleAccessAsync(string roleId)
            {
                try
                {
                    using (var conn = new SqlConnection(_options.ConnectionString))
                    {
                        const string query = "SELECT [Id], [RoleId], [Access], [RastiAccess] FROM [RoleAccess] WHERE [RoleId] = @RoleId";
                        using (var cmd = new SqlCommand(query, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@RoleId", roleId);
                            conn.Open();
                            var reader = await cmd.ExecuteReaderAsync();
                            if (!reader.Read())
                                return null;

                            var roleAccess = new RoleAccess();
                            roleAccess.Id = int.Parse(reader[0].ToString());
                            roleAccess.RoleId = reader[1].ToString();
                            var json = reader[2].ToString();
                            roleAccess.Controllers = JsonConvert.DeserializeObject<IEnumerable<MvcControllerInfo>>(json);
                            var json2 = reader[3].ToString();
                            roleAccess.RastiAccess = JsonConvert.DeserializeObject<IEnumerable<MainController>>(json2);

                            return roleAccess;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occurred while getting data from RoleAccess table");
                    return null;
                }
            }

            public async Task<bool> HasAccessToActionAsync(string actionId, params string[] roles)
            {
                try
                {
                    using (var conn = new SqlConnection(_options.ConnectionString))
                    {
                        using (var cmd = new SqlCommand())
                        {
                            var parameters = new string[roles.Length];
                            for (var i = 0; i < roles.Length; i++)
                            {
                                parameters[i] = $"@RoleId{i}";
                                cmd.Parameters.AddWithValue(parameters[i], roles[i]);
                            }
                            var query = $"SELECT [Access] FROM [RoleAccess] WHERE [RoleId] IN ({string.Join(", ", parameters)})";

                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = query;
                            cmd.Connection = conn;

                            conn.Open();
                            var reader = await cmd.ExecuteReaderAsync();

                            var list = new List<MvcActionInfo>();
                            while (reader.Read())
                            {
                                var json = reader[0].ToString();
                                if (string.IsNullOrEmpty(json))
                                    continue;

                                var controllers = JsonConvert.DeserializeObject<IEnumerable<MvcControllerInfo>>(json);
                                list.AddRange(controllers.SelectMany(c => c.Actions));
                            }

                            return list.Any(a => a.Id == actionId);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error has occurred while getting data from RoleAccess table");
                    return false;
                }
            }

        public async Task<bool> HasAccessToCustomActionAsync(int rastiId, string controller, string action, int controllerType, string controllerGroup, params string[] roles)
        {
            try
            {
                using (var conn = new SqlConnection(_options.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        var parameters = new string[roles.Length];
                        for (var i = 0; i < roles.Length; i++)
                        {
                            parameters[i] = $"@RoleId{i}";
                            cmd.Parameters.AddWithValue(parameters[i], roles[i]);
                        }
                        var query = $"SELECT [RastiAccess] FROM [RoleAccess] WHERE [RoleId] IN ({string.Join(", ", parameters)})";

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query;
                        cmd.Connection = conn;

                        conn.Open();
                        var reader = await cmd.ExecuteReaderAsync();

                        if(controllerType== 1) //MainController
                        {
                            if(rastiId == 0) //toivottavasti ei oo 0 idllä olevia
                            {
                                return false;
                            }
                            var list = new List<Models.DynamicAuth.Custom.Action>();
                            while (reader.Read())
                            {
                                var json = reader[0].ToString();
                                if (string.IsNullOrEmpty(json))
                                    continue;

                                var controllers = JsonConvert.DeserializeObject<IEnumerable<MainController>>(json);
                                var idControllers = controllers.Where(x => x.RastiId == rastiId);
                                var notnullidController = idControllers.Where(x => x.Actions != null);
                                list.AddRange(notnullidController.SelectMany(c => c.Actions));
                            }

                            return list.Any(a => a.Name == action);
                        }
                        else if(controllerType== 2)//SubController 
                        {
                            if (rastiId == 0) //toivottavasti ei oo 0 idllä olevia
                            {
                                return false;
                            }
                            //en keksi miten tehdä dynaaminen authentikaatio hyvin tässä kohdassa joten tähän joutuu lisätä erikseen jokaiselle subcontrollerille osan
                            if (controller == "Tehtava")
                            {
                                var list = new List<Models.DynamicAuth.Custom.Action>();
                                while (reader.Read())
                                {
                                    var json = reader[0].ToString();
                                    if (string.IsNullOrEmpty(json))
                                        continue;

                                    var controllers = JsonConvert.DeserializeObject<IEnumerable<MainController>>(json);
                                    var tehtava = await _context.Tehtava.FindAsync(rastiId);
                                    if(tehtava == null)
                                    {
                                        return false;
                                    }

                                   if(controllers != null && controllers.Any())
                                    {
                                        var tehtcontroller = controllers.Where(x => x.RastiId == tehtava.RastiId).ToList();
                                        if(tehtcontroller != null && tehtcontroller.Any())
                                        {
                                            var firstsubcontroller = tehtcontroller.First();
                                            if(firstsubcontroller != null)
                                            {
                                                var subcontrollers = firstsubcontroller.SubControllers;
                                                if(subcontrollers != null && subcontrollers.Any())
                                                {
                                                    var subcontroller = subcontrollers.Where(x => x.Name == controller);
                                                    if(subcontroller != null && subcontroller.Any())
                                                    {
                                                        list = subcontroller.First().Actions.ToList();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                return list.Any(a => a.Name == action);
                            }
                           else if(controller == "Tag")
                            {
                                if (rastiId == 0) //toivottavasti ei oo 0 idllä olevia
                                {
                                    return false;
                                }
                                var list = new List<Models.DynamicAuth.Custom.Action>();
                                while (reader.Read())
                                {
                                    var json = reader[0].ToString();
                                    if (string.IsNullOrEmpty(json))
                                        continue;

                                    var controllers = JsonConvert.DeserializeObject<IEnumerable<MainController>>(json);
                                    if(controllers != null && controllers.Any())
                                    {
                                        var idControllers = controllers.Where(x => x.RastiId == rastiId);
                                        if(idControllers != null && idControllers.Any())
                                        {
                                            var firstIdController = idControllers.First();
                                           if(firstIdController != null)
                                            {
                                                var subcontrollers = firstIdController.SubControllers;
                                                if(subcontrollers != null && subcontrollers.Any())
                                                {
                                                    var subcontroller = subcontrollers.Where(x => x.Name == controller);
                                                    if(subcontroller != null && subcontroller.Any())
                                                    {
                                                        list = subcontroller.First().Actions.ToList();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                return list.Any(a => a.Name == action);
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while getting data from RoleAccess table");
                return false;
            }
        }

        public async Task<List<int>> HasAccessToRastiIdsAsync(params string[] roles)
        {
            try
            {
                using (var conn = new SqlConnection(_options.ConnectionString))
                {
                    using (var cmd = new SqlCommand())
                    {
                        var parameters = new string[roles.Length];
                        for (var i = 0; i < roles.Length; i++)
                        {
                            parameters[i] = $"@RoleId{i}";
                            cmd.Parameters.AddWithValue(parameters[i], roles[i]);
                        }
                        var query = $"SELECT [RastiAccess] FROM [RoleAccess] WHERE [RoleId] IN ({string.Join(", ", parameters)})";

                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = query;
                        cmd.Connection = conn;

                        conn.Open();
                        var reader = await cmd.ExecuteReaderAsync();

                        var list = new List<int>();
                        while (reader.Read())
                        {
                            var json = reader[0].ToString();
                            if (string.IsNullOrEmpty(json))
                                continue;

                            var controllers = JsonConvert.DeserializeObject<IEnumerable<MainController>>(json);
                            foreach(var con in controllers)
                            {
                                if (!list.Contains((int)con.RastiId))
                                {
                                    list.Add((int)con.RastiId);
                                }
                            }
                        }

                        return list;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error has occurred while getting data from RoleAccess table");
                return new List<int>();
            }
        }
    }
    
}
