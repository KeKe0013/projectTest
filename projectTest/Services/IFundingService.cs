using Microsoft.AspNetCore.Mvc.Rendering;
using projectTest.Models;

namespace projectTest.Services
{
    public interface IFundingService
    {
        List<SelectListItem> GetPlansByFundingSource(string fundingSource);
        Object GetPlanDetail(string planName,string fundingSource);
        Studentinfo GetStudentInfo(string studentId);

    }
}
