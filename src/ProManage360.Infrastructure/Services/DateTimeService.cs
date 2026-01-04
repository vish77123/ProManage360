namespace ProManage360.Infrastructure.Services;

using ProManage360.Application.Common.Interfaces;
using ProManage360.Application.Common.Interfaces.Service;
using System;

public class DateTimeService : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}