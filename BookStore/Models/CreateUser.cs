using System;
using System.Collections.Generic;

namespace Authentication.Models;

public class CreateUser
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
}