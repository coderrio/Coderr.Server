using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Security;
using Coderr.Server.Api.Core.Accounts.Queries;
using Coderr.Server.Api.Core.Applications;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Api.Insights.Queries;
using Coderr.Server.App.Insights;
using Coderr.Server.App.Insights.Keyfigures;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;

namespace Coderr.Server.App.IncidentInsights
{
    public class GetInsightsHandler : IQueryHandler<GetInsights, GetInsightsResult>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IApplicationRepository _applicationRepository;
        private readonly Dictionary<string, string> _cachedValueIds = new Dictionary<string, string>();
        private readonly IEnumerable<IKeyPerformanceIndicatorGenerator> _generators;
        private readonly IQueryBus _queryBus;
        private ApplicationListItem[] _apps;
        private readonly Dictionary<int, string> _userCache = new Dictionary<int, string>();

        public GetInsightsHandler(IQueryBus queryBus,
            IEnumerable<IKeyPerformanceIndicatorGenerator> generators, IApplicationRepository applicationRepository,
            IAccountRepository accountRepository)
        {
            _queryBus = queryBus;
            _generators = generators;
            _applicationRepository = applicationRepository;
            _accountRepository = accountRepository;
        }

        public async Task<GetInsightsResult> HandleAsync(IMessageContext context,
            GetInsights query)
        {
            _apps = await _queryBus.QueryAsync(new GetApplicationList
            {
                AccountId = context.Principal.GetAccountId()
            });
            await AddMembersToAppsWithoutConfiguredDevelopers(_apps);


            //var now = new DateTime(2019, 03, 30);
            var now = DateTime.UtcNow;
            var dates = new List<DateTime>();
            for (var i = 0; i <= 12; i++)
                dates.Add(now.ToFirstDayOfMonth().AddMonths(-12 + i));



            var indicatorsPerApplication = new Dictionary<int, List<KeyPerformanceIndicator>>();
            var ctx = new KeyPerformanceIndicatorContext(indicatorsPerApplication)
            {
                ApplicationIds = _apps.Select(x => x.Id).ToArray(),
                PeriodStartDate = now.Date.AddDays(-90),
                PeriodEndDate = now,
                ValueStartDate = now.Date.AddDays(-30),
                ValueEndDate = now,
                StartDate = dates.FirstOrDefault(),
                EndDate = now,
                TrendDates = dates.ToArray()
            };

            foreach (var generator in _generators)
                await generator.CollectAsync(ctx);

            AddNormalizedTrendValues(indicatorsPerApplication);

            var systemInsights = new List<GetInsightResultIndicator>();
            if (indicatorsPerApplication.TryGetValue(0, out var systemIndicators))
            {
                AddNormalizedTrendValues(systemIndicators);
                systemInsights = new List<GetInsightResultIndicator>();
                foreach (var indicator in systemIndicators)
                {
                    var dto = await ConvertIndicator(indicator);
                    await LoadValueName(indicator, dto);
                    systemInsights.Add(dto);
                }
            }

            //var systemInsights2 = await GenerateSystemInsights(indicatorsPerApplication);
            //systemInsights.AddRange(systemInsights2);

            var apps = new List<GetInsightResultApplication>();
            foreach (var pair in indicatorsPerApplication.Where(x => x.Key != 0))
            {
                var dto = await ConvertApplication(pair);
                apps.Add(dto);
            }


            var result = new GetInsightsResult
            {
                Indicators = systemInsights.ToArray(),
                ApplicationInsights = apps.ToArray(),
                TrendDates = ctx.TrendDates.Select(x => x.ToShortDateString()).ToArray()
            };

            return result;
        }

        private void AddNormalizedTrendValues(List<KeyPerformanceIndicator> indicators)
        {
            foreach (var indicator in indicators)
            {
                if (!indicator.CanBeNormalized || indicator.ValueIdType != ValueIdType.ApplicationId)
                    continue;

                //only app indicators can be normalized
                foreach (var trendLine in indicator.TrendLines)
                {
                    foreach (var value in trendLine.TrendValues)
                    {
                        
                        var app = _apps.First(x => x.Id == trendLine.DisplayNameId);
                        if (app.NumberOfDevelopers == null)
                            continue;

                        value.Normalized =
                            Convert.ToInt32(GetDouble(value.Value) / (double)app.NumberOfDevelopers.Value);
                    }
                }
            }
        }

        private void AddNormalizedTrendValues(Dictionary<int, List<KeyPerformanceIndicator>> indicatorsPerApplication)
        {
            foreach (var indicators in indicatorsPerApplication)
            {
                //Ignore system indicators.
                if (indicators.Key == 0)
                    continue;

                foreach (var indicator in indicators.Value)
                {
                    if (!indicator.CanBeNormalized)
                        continue;

                    //only app indicators can be normalized
                    foreach (var trendLine in indicator.TrendLines)
                    {
                        foreach (var value in trendLine.TrendValues)
                        {
                            var app = _apps.First(x => x.Id == indicators.Key);
                            if (app.NumberOfDevelopers == null)
                                continue;

                            value.Normalized = Convert.ToInt32(GetDouble(value.Value) / (double)app.NumberOfDevelopers.Value);
                        }
                    }
                }
            }
        }

        private double GetNormalizedValue(KeyPerformanceIndicator keyPerformanceIndicator)
        {
            if (keyPerformanceIndicator.ValueIdType != ValueIdType.ApplicationId)
                return GetDouble(keyPerformanceIndicator.Value);

            var devs = GetDevs(keyPerformanceIndicator.ValueId);
            var value = GetDouble(keyPerformanceIndicator.Value);
            return value.DivideWith((double)devs);
        }

        private decimal GetDevs(int? applicationId)
        {
            if (applicationId == null)
                return 0;

            var app = _apps.FirstOrDefault(x => x.Id == applicationId.Value);
            if (app == null)
                return 0;

            return app.NumberOfDevelopers ?? 0;
        }

        /// <summary>
        /// Boxing galore!
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private double GetDouble(object value)
        {
            if (value == null)
                return 0d;

            if (value is double d)
                return d;

            if (value is int i)
                return i;

            if (value is decimal dec)
                return (double)dec;

            return (double)Convert.ChangeType(value, typeof(double));

        }

        private int? RoundValue(object value, ValueIdType valueType)
        {
            if (value is null)
            {
                return null;
            }

            if (value is int)
                return (int)value;

            return (int)Convert.ChangeType(GetDouble(value), typeof(int));
        }

        private async Task LoadValueName(KeyPerformanceIndicator indicator, GetInsightResultIndicator dto)
        {
            if (indicator.ValueIdType == ValueIdType.AccountId)
            {
                if (indicator.ValueId == 0 || indicator.ValueId == null)
                {
                    dto.ValueName = "No one";
                    return;
                }

                if (!_userCache.TryGetValue((int)indicator.ValueId, out var name))
                {
                    var userId = (int)indicator.ValueId;
                    name = (await _accountRepository.GetByIdAsync(userId)).UserName;
                    _userCache[userId] = name;
                }

                dto.ValueName = name;
            }
            else if (indicator.ValueIdType == ValueIdType.AccountId)
            {
                if (indicator.ValueId == 0)
                {
                    dto.ValueName = "None";
                    return;
                }

                var app = _apps.First(x => x.Id == indicator.ValueId);
                dto.ValueName = app.Name;
            }
        }

        private async Task AddMembersToAppsWithoutConfiguredDevelopers(ApplicationListItem[] apps)
        {
            foreach (var item in apps.Where(x => x.NumberOfDevelopers == null))
            {
                var members = await _applicationRepository.GetTeamMembersAsync(item.Id);
                item.NumberOfDevelopers = members.Count;
            }
        }

        private async Task<GetInsightResultApplication> ConvertApplication(
            KeyValuePair<int, List<KeyPerformanceIndicator>> arg)
        {
            var app = _apps.First(x => x.Id == arg.Key);
            var indicators = new List<GetInsightResultIndicator>();
            foreach (var indicator in arg.Value)
            {
                var converted = await ConvertIndicator(indicator);
                indicators.Add(converted);
            }

            return new GetInsightResultApplication
            {
                Id = app.Id,
                Name = app.Name,
                NumberOfDevelopers =
                    app.NumberOfDevelopers.Value, // we've added Team members for apps that doesn't specify FTEs
                Indicators = indicators.ToArray()

            };
        }

        private async Task<GetInsightResultIndicator> ConvertIndicator(KeyPerformanceIndicator arg)
        {
            var toplist = arg.Toplist != null
                ? await ConvertToplist(arg)
                : new List<IndicatorToplistItem>();

            await AddNameToTrendlines(arg.ValueIdType, arg.TrendLines);

            var dto = new GetInsightResultIndicator
            {
                CanBeNormalized = arg.CanBeNormalized,
                Name = arg.Name,
                Description = arg.Description,
                Comment = arg.Comment,
                PeriodValue = RoundValue(arg.PeriodValue, arg.ValueIdType),
                Title = arg.Title,
                IsAlternative = arg.IsVariation,
                TrendLines = arg.TrendLines,
                Toplist = toplist.ToArray(),
                ValueUnit = arg.ValueUnit,
                Value = RoundValue(arg.Value, arg.ValueIdType),
                IsValueNameApplicationName = arg.ValueIdType == ValueIdType.ApplicationId
            };

            if (arg.ValueIdType == ValueIdType.AccountId && arg.ValueId == null)
                dto.ValueName = "No one";
            if (arg.ValueIdType == ValueIdType.AccountId && arg.PeriodValueId == null)
                dto.PeriodValueName = "No one";
            if (arg.ValueIdType == ValueIdType.ApplicationId && arg.ValueId == null)
                dto.ValueName = "None";
            if (arg.ValueIdType == ValueIdType.ApplicationId && arg.PeriodValueId == null)
                dto.PeriodValueName = "None";


            if (arg.ValueId != null)
                dto.ValueName = await GetValueName(arg.ValueIdType, arg.ValueId);
            if (arg.PeriodValueId != null)
                dto.PeriodValueName = await GetValueName(arg.ValueIdType, arg.PeriodValueId);

            switch (arg.ComparisonType)
            {
                case IndicatorValueComparison.DoNotCompare:
                    dto.HigherValueIsBetter = null;
                    break;
                case IndicatorValueComparison.HigherIsBetter:
                    dto.HigherValueIsBetter = true;
                    break;
                case IndicatorValueComparison.LowerIsBetter:
                    dto.HigherValueIsBetter = false;
                    break;
            }

            return dto;
        }

        private async Task AddNameToTrendlines(ValueIdType type, TrendLine[] trendLines)
        {
            foreach (var line in trendLines)
            {
                if (line.DisplayNameId > 0)
                {
                    line.DisplayName = await GetValueName(type, line.DisplayNameId);
                }
            }
        }

        private async Task<List<IndicatorToplistItem>> ConvertToplist(KeyPerformanceIndicator arg)
        {
            var toplist = new List<IndicatorToplistItem>();
            foreach (var item in arg.Toplist)
            {
                var dtoItem = new IndicatorToplistItem
                {
                    Value = item.Value is double ? Convert.ToInt32(item.Value) : item.Value,
                    Comment = item.Comment,
                };
                if (item.ValueId != null)
                    dtoItem.Title = await GetValueName(arg.ToplistValueType, item.ValueId);
                else
                    dtoItem.Title = item.ValueName;
                toplist.Add(dtoItem);
            }

            return toplist;
        }

        private async Task<string> GetValueName(ValueIdType valueType, int? id)
        {
            if (id == null || 0.Equals(id))
            {
                return valueType == ValueIdType.AccountId ? "No one" : "No application";
            }

            if (_cachedValueIds.TryGetValue($"{valueType}_{id}", out var value))
                return value;

            switch (valueType)
            {
                case ValueIdType.AccountId:
                    {
                        var result = await _queryBus.QueryAsync(new GetAccountById(id.Value));
                        _cachedValueIds[$"{valueType}_{id}"] = result.UserName;
                        return result.UserName;
                    }
                case ValueIdType.ApplicationId:
                    {
                        return _apps.First(x => x.Id == id.Value).Name;
                    }
                default:
                    return null;
            }
        }
    }
}