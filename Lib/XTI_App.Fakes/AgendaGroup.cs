using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using XTI_App.Api;

namespace XTI_App.Fakes
{
    public sealed class AgendaGroup : AppApiGroupWrapper
    {
        public AgendaGroup(AppApiGroup source, IServiceProvider sp) : base(source)
        {
            var actions = new AppApiActionFactory(source);
            FirstAgendaItem = source.AddAction
            (
                actions.Action
                (
                    nameof(FirstAgendaItem),
                    () => new FirstAgendaItemAction(sp.GetService<FirstAgendaItemCounter>())
                )
            );
            SecondAgendaItem = source.AddAction
            (
                actions.Action
                (
                    nameof(SecondAgendaItem),
                    () => new SecondAgendaItemAction()
                )
            );
            ThirdAgendaItem = source.AddAction
            (
                actions.Action
                (
                    nameof(ThirdAgendaItem),
                    () => new ThirdAgendaItemAction()
                )
            );
            Stop = source.AddAction
            (
                actions.Action
                (
                    nameof(Stop),
                    () => new StopApplicationAction(sp.GetService<IHostApplicationLifetime>())
                )
            );
        }

        public AppApiAction<EmptyRequest, EmptyActionResult> FirstAgendaItem { get; }
        public AppApiAction<EmptyRequest, EmptyActionResult> SecondAgendaItem { get; }
        public AppApiAction<EmptyRequest, EmptyActionResult> ThirdAgendaItem { get; }
        public AppApiAction<EmptyRequest, EmptyActionResult> Stop { get; }
    }
}
