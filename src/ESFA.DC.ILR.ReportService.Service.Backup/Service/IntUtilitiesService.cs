﻿using System;
using ESFA.DC.ILR.ReportService.Interface.Service;

namespace ESFA.DC.ILR.ReportService.Service.Service
{
    public sealed class IntUtilitiesService : IIntUtilitiesService
    {
        public int ObjectToInt(object value)
        {
            if (value is int i)
            {
                return i;
            }

            return Convert.ToInt32(value);
        }
    }
}
