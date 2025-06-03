using BillPay.DataAccess.Data;
using BillPay.Models;
using BillPay.Utilities.CommonMethods;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BillPay.Utilities.Modules
{
    public static class SeedingModule
    {
        public static void RunWithProgramStart(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                using (AppDbContext _context = scope.ServiceProvider.GetRequiredService<AppDbContext>())
                {
                    var newScriptToRun = GetSqlScriptsFromFolder(env);
                    foreach (var script in newScriptToRun)
                    {
                        var scriptHash = script.Value.GetHashString();
                        var exists = true;
                        ProcedureSeedingLog existingProcedureLog = _context.ProcedureSeedingLog
                                        .OrderByDescending(x => x.ExecutedDatetime)
                                        .FirstOrDefault(x => x.ProcedureName == script.Key)!;
                        if (existingProcedureLog != null)
                            if(existingProcedureLog.HasError || !existingProcedureLog.ScriptHash.Equals(scriptHash))
                                exists = false;
                        if (existingProcedureLog == null)
                        {
                            exists = false;
                        }
                        if (!exists)
                        {
                            using (var tranScope = _context.Database.BeginTransaction())
                            {
                                try
                                {
                                    _context.Database.ExecuteSqlRaw(script.Value);
                                    ProcedureSeedingLog procedureSeedingLog = new ProcedureSeedingLog()
                                    {
                                        ProcedureName = script.Key,
                                        ScriptHash = scriptHash,
                                        ExecutedDatetime = DateTime.Now,
                                        HasError = false,
                                        ErrorMessage = string.Empty
                                    };
                                    _context.ProcedureSeedingLog.Add(procedureSeedingLog);
                                    _context.SaveChanges();
                                    tranScope.Commit();
                                }
                                catch (Exception ex)
                                {
                                    tranScope.Rollback();
                                    ProcedureSeedingLog procedureSeedingLog = new ProcedureSeedingLog()
                                    {
                                        ProcedureName = script.Key,
                                        ScriptHash = scriptHash,
                                        ExecutedDatetime = DateTime.Now,
                                        HasError = true,
                                        ErrorMessage = ex.Message
                                    };
                                    _context.ProcedureSeedingLog.Add(procedureSeedingLog);
                                    _context.SaveChanges();
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }
        private static Dictionary<string, string> GetSqlScriptsFromFolder(IWebHostEnvironment env)
        {
            DirectoryInfo currentDirInfo = new DirectoryInfo(env.ContentRootPath);
            string baseDirectory = currentDirInfo.Parent.FullName;
            string DataAccessDirPath = Path.Combine(baseDirectory, "BillPay.DataAccess");
            string proceduresDirPath = Path.Combine(DataAccessDirPath, "Procedures");
            if (Directory.Exists(proceduresDirPath) == false)
            {
                Directory.CreateDirectory(proceduresDirPath);
            }

            return Directory.GetFiles(proceduresDirPath, "*.sql", SearchOption.AllDirectories).ToDictionary(Path.GetFileName, File.ReadAllText);
        }
    }
}
