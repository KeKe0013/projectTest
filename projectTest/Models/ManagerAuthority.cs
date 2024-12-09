using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;
using projectTest.Attributes;

namespace projectTest.Models;

public partial class ManagerAuthority
{
    public string EmployeeNo { get; set; }
    public string Authority { get; set; }
}