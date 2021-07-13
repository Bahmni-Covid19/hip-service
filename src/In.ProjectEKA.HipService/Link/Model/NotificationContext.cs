using System;
using System.Collections.Generic;
using In.ProjectEKA.HipService.Common.Model;

namespace In.ProjectEKA.HipService.Link.Model
{
    public class NotificationContext
    {
        public Patient Patient { get; }

        public NotificationCareContext NotificationCareContext { get; }

        public List<string> HiType { get; }

        public DateTime Date { get; }

        public HIPReference Hip { get; }


        public NotificationContext(Patient patient, NotificationCareContext notificationCareContext,
            List<string> hiType, DateTime dateTime, HIPReference hip)
        {
            Patient = patient;
            NotificationCareContext = notificationCareContext;
            HiType = hiType;
            Date = dateTime;
            Hip = hip;
        }
    }
}