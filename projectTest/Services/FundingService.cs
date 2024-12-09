using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Net.Http.Headers;
using PdfSharp.Pdf.Content.Objects;
using projectTest.Models;
using System.Data;


namespace projectTest.Services
{
    public class FundingService : IFundingService
    {
        private readonly dbContext _db;
        public FundingService(dbContext context)
        {
            _db = context ?? throw new ArgumentNullException(nameof(context));
        }
        public List<SelectListItem> GetPlansByFundingSource(string fundingSource)
        {
            List<SelectListItem> plans = new List<SelectListItem>();

            if (fundingSource == "(經常門)計畫預算")
            {
                var parameter = new SqlParameter("@p_UserCard", SqlDbType.VarChar, 12) { Value = 2026 };

                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC [dbo].[Pur_spGetProjectBudget] @p_UserCard";
                    command.Parameters.Add(parameter);
                    _db.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["sourcetype"]?.ToString() == "1")
                            {
                                plans.Add(new SelectListItem
                                {
                                    Value = reader["projectname"].ToString(),
                                    Text = reader["projectname"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            else if (fundingSource == "(經常門)部門預算")
            {
                var parameter = new SqlParameter("@p_UserCard", SqlDbType.VarChar, 12) { Value = 2026 };

                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "EXEC [dbo].[Pur_spGetDepBudget] @p_UserCard";
                    command.Parameters.Add(parameter);
                    _db.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["sourcetype"]?.ToString() == "1")
                            {
                                plans.Add(new SelectListItem
                                {
                                    Value = reader["fundname"].ToString(),
                                    Text = reader["fundname"].ToString()
                                });
                            }
                        }
                    }
                }
            }

            //if (fundingSource == "(經常門)部門預算")
            //{
            //    var Plans = _db.ProjectBudget.Select(o => o.projectname).Distinct().ToList();

            //    foreach (var plan in Plans)
            //    {
            //        plans.Add(new SelectListItem { Value = plan, Text = plan });
            //    }
            //}



            //else if (fundingSource == "(資本門)部門預算")
            //{
            //    var Plans = _db.DepBudget.Select(o => o.fundname).Distinct().ToList();

            //    foreach (var plan in Plans)
            //    {
            //        plans.Add(new SelectListItem { Value = plan, Text = plan });
            //    }
            //}

            return plans;
        }


        public object GetPlanDetail(string planName, string fundingSource)
        {
            if (string.IsNullOrEmpty(planName))
            {
                throw new ArgumentException("Plan name cannot be null or empty.", nameof(planName));
            }

            if (string.IsNullOrEmpty(fundingSource))
            {
                throw new ArgumentException("Funding source cannot be null or empty.", nameof(fundingSource));
            }

            string storedProcedure;
            if (fundingSource == "(經常門)部門預算")
            {
                storedProcedure = "EXEC [dbo].[Pur_spGetDepBudget] @p_UserCard";
            }
            else if (fundingSource == "(經常門)計畫預算")
            {
                storedProcedure = "EXEC [dbo].[Pur_spGetProjectBudget] @p_UserCard";
            }
            else
            {
                throw new ArgumentException("Invalid funding source.", nameof(fundingSource));
            }

            object planDetail = null;

            var parameter = new SqlParameter("@p_UserCard", SqlDbType.VarChar, 255) { Value = 2026 };

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = storedProcedure;
                command.Parameters.Add(parameter);
                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (fundingSource == "(經常門)部門預算")
                        {
                            var depBudget = new DepBudget
                            {
                                projectno = reader["ProjectNo"]?.ToString(), //會計編號
                                fundname = reader["FundName"]?.ToString(),   //經費用途說明
                            };

                            if (depBudget.fundname == planName)
                            {
                                planDetail = depBudget;
                                break;
                            }
                        }
                        else if (fundingSource == "(經常門)計畫預算")
                        {
                            var projectBudget = new ProjectBudget
                            {
                                projectno = reader["projectno"]?.ToString(), //會計編號
                                projno = reader["projno"]?.ToString(),       //計畫編號
                                comname = reader["comname"]?.ToString(),
                                startdate = reader["startdate"]?.ToString(),
                                enddate = reader["enddate"]?.ToString(),
                                extenddate = reader["extenddate"]?.ToString(),
                                projectname = reader["projectname"]?.ToString(),    // 計畫名稱
                                leadername = reader["leadername"]?.ToString()
                            };

                            if (projectBudget.projectname == planName)
                            {
                                planDetail = projectBudget;
                                break;
                            }
                        }
                    }
                }

                _db.Database.CloseConnection();
            }

            return planDetail;
        }

        //public Object GetPlanDetail(string planName, string fundingSource)
        //{
        //    var planDetail = _db.ProjectBudget
        //        .Where(p => p.projectname == planName)
        //        .Select(p => new ProjectBudget
        //        {
        //            projectno = p.projectno,
        //            projno = p.projno,
        //            comname = p.comname,
        //            startdate = p.startdate,
        //            enddate = p.enddate,
        //            extenddate = p.extenddate
        //        }).FirstOrDefault();

        //    return planDetail;
        //}


        //public Studentinfo GetStudentInfo(string studentid)
        //{
        //    var studentInfo = _db.Studentinfo
        //        .Where(s => s.studentID == studentid)
        //        .Select(s => new Studentinfo
        //        {
        //            studentID = s.studentID,
        //            cname = s.cname,
        //            ID = s.ID,
        //            phone = s.phone,
        //            birthday = s.birthday
        //        }).FirstOrDefault();

        //    return studentInfo;
        //}

        public Studentinfo GetStudentInfo(string studentid)
        {
            var parameter = new SqlParameter("@p_Classno", SqlDbType.VarChar, 12) { Value = studentid };

            var result = _db.Database.ExecuteSqlRaw("EXEC [dbo].[sp_StudentInfo] @p_Classno", parameter);

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "EXEC [dbo].[sp_StudentInfo] @p_Classno";
                command.Parameters.Add(parameter);
                _db.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Studentinfo
                        {
                            studentID = reader["Student_ID"].ToString(),
                            cname = reader["Cname"].ToString(),
                            ID = reader["ID"].ToString(),
                            phone = reader["Phone"].ToString(),
                            birthday = reader["Birthday"].ToString(),
                        };
                    }
                }
            }

            return null;
        }


    }

}
