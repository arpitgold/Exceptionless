﻿#region Copyright 2014 Exceptionless

// This program is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published 
// by the Free Software Foundation, either version 3 of the License, or 
// (at your option) any later version.
// 
//     http://www.gnu.org/licenses/agpl-3.0.html

#endregion

using System;
using CodeSmith.Core.Component;
using Exceptionless.Core.Plugins.EventProcessor;
using NLog.Fluent;

namespace Exceptionless.Core.Pipeline {
    [Priority(1)]
    public class DiscardEventsOutsideOfRetentionAction : EventPipelineActionBase {
        protected override bool ContinueOnError { get { return true; } }

        public override void Process(EventContext ctx) {
            if (ctx.Organization.RetentionDays <= 0)
                return;

            if (DateTimeOffset.Now.UtcDateTime.Subtract(ctx.Event.Date.UtcDateTime).Days <= ctx.Organization.RetentionDays)
                return;

            Log.Info().Project(ctx.Event.ProjectId).Message("Discarding event that occurred outside of your retention limit.").Write();
            ctx.IsCancelled = true;
        }
    }
}