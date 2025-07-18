﻿using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Views.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Views.Finders
{
    public class FinderPostView
    {
        public string Id
        {
            get; set;
        }
        public string Title
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public DateTimeOffset? StartTime
        {
            get; set;
        }

        public DateTimeOffset? EndTime
        {
            get; set;
        }

        public int StartPrice
        {
            get; set;
        }

        public int? EndPrice
        {
            get; set;
        }

        public double? Lat
        {
            get; set;
        }

        public double? Lng
        {
            get; set;
        }

        public string? PlaceName
        {
            get; set;
        }

        public bool HideAddress
        {
            get; set;
        }

        public RepeatingType RepeatType
        {
            get; set;
        }

        public FinderPostStatus Status
        {
            get; set;
        }

        public int NotiCount
        {
            get; set;
        }

        public DateTimeOffset CreatedAt
        {
            get; set;
        }

        public DateTimeOffset LastModified
        {
            get; set;
        }
    }
}
