create table SpikeAggregation
(
    Id int not null identity primary key,
    ApplicationId int not null constraint FK_SpikeAggregation_Applications foreign key references Applications(Id) on delete cascade,
    TrackedDate Date not null,
    ReportCount int not null,
    Notified bit not null default 0
);

insert into SpikeAggregation (ApplicationId, TrackedDate, ReportCount, Notified)
select i.ApplicationID, convert(date, ir.ReceivedAtUtc), count(*), 0
from IncidentReports ir
join incidents i on (incidentid =i.id)
group by i.ApplicationId, convert(date, ir.ReceivedAtUtc)
