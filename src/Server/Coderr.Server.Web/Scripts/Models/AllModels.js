var codeRR;
(function (codeRR) {
    var Web;
    (function (Web) {
        var Overview;
        (function (Overview) {
            var Queries;
            (function (Queries) {
                var GetOverview = /** @class */ (function () {
                    function GetOverview() {
                    }
                    GetOverview.TYPE_NAME = 'GetOverview';
                    return GetOverview;
                }());
                Queries.GetOverview = GetOverview;
                var GetOverviewApplicationResult = /** @class */ (function () {
                    function GetOverviewApplicationResult(label, startDate, days) {
                        this.Label = label;
                    }
                    GetOverviewApplicationResult.TYPE_NAME = 'GetOverviewApplicationResult';
                    return GetOverviewApplicationResult;
                }());
                Queries.GetOverviewApplicationResult = GetOverviewApplicationResult;
                var GetOverviewResult = /** @class */ (function () {
                    function GetOverviewResult() {
                    }
                    GetOverviewResult.TYPE_NAME = 'GetOverviewResult';
                    return GetOverviewResult;
                }());
                Queries.GetOverviewResult = GetOverviewResult;
                var OverviewStatSummary = /** @class */ (function () {
                    function OverviewStatSummary() {
                    }
                    OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                    return OverviewStatSummary;
                }());
                Queries.OverviewStatSummary = OverviewStatSummary;
            })(Queries = Overview.Queries || (Overview.Queries = {}));
        })(Overview = Web.Overview || (Web.Overview = {}));
    })(Web = codeRR.Web || (codeRR.Web = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Web;
    (function (Web) {
        var Feedback;
        (function (Feedback) {
            var Queries;
            (function (Queries) {
                var GetFeedbackForApplicationPage = /** @class */ (function () {
                    function GetFeedbackForApplicationPage(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetFeedbackForApplicationPage.TYPE_NAME = 'GetFeedbackForApplicationPage';
                    return GetFeedbackForApplicationPage;
                }());
                Queries.GetFeedbackForApplicationPage = GetFeedbackForApplicationPage;
                var GetFeedbackForApplicationPageResult = /** @class */ (function () {
                    function GetFeedbackForApplicationPageResult() {
                    }
                    GetFeedbackForApplicationPageResult.TYPE_NAME = 'GetFeedbackForApplicationPageResult';
                    return GetFeedbackForApplicationPageResult;
                }());
                Queries.GetFeedbackForApplicationPageResult = GetFeedbackForApplicationPageResult;
                var GetFeedbackForApplicationPageResultItem = /** @class */ (function () {
                    function GetFeedbackForApplicationPageResultItem() {
                    }
                    GetFeedbackForApplicationPageResultItem.TYPE_NAME = 'GetFeedbackForApplicationPageResultItem';
                    return GetFeedbackForApplicationPageResultItem;
                }());
                Queries.GetFeedbackForApplicationPageResultItem = GetFeedbackForApplicationPageResultItem;
                var GetFeedbackForDashboardPage = /** @class */ (function () {
                    function GetFeedbackForDashboardPage() {
                    }
                    GetFeedbackForDashboardPage.TYPE_NAME = 'GetFeedbackForDashboardPage';
                    return GetFeedbackForDashboardPage;
                }());
                Queries.GetFeedbackForDashboardPage = GetFeedbackForDashboardPage;
                var GetFeedbackForDashboardPageResult = /** @class */ (function () {
                    function GetFeedbackForDashboardPageResult() {
                    }
                    GetFeedbackForDashboardPageResult.TYPE_NAME = 'GetFeedbackForDashboardPageResult';
                    return GetFeedbackForDashboardPageResult;
                }());
                Queries.GetFeedbackForDashboardPageResult = GetFeedbackForDashboardPageResult;
                var GetFeedbackForDashboardPageResultItem = /** @class */ (function () {
                    function GetFeedbackForDashboardPageResultItem() {
                    }
                    GetFeedbackForDashboardPageResultItem.TYPE_NAME = 'GetFeedbackForDashboardPageResultItem';
                    return GetFeedbackForDashboardPageResultItem;
                }());
                Queries.GetFeedbackForDashboardPageResultItem = GetFeedbackForDashboardPageResultItem;
                var GetIncidentFeedback = /** @class */ (function () {
                    function GetIncidentFeedback(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncidentFeedback.TYPE_NAME = 'GetIncidentFeedback';
                    return GetIncidentFeedback;
                }());
                Queries.GetIncidentFeedback = GetIncidentFeedback;
                var GetIncidentFeedbackResult = /** @class */ (function () {
                    function GetIncidentFeedbackResult(items, emails) {
                        this.Items = items;
                        this.Emails = emails;
                    }
                    GetIncidentFeedbackResult.TYPE_NAME = 'GetIncidentFeedbackResult';
                    return GetIncidentFeedbackResult;
                }());
                Queries.GetIncidentFeedbackResult = GetIncidentFeedbackResult;
                var GetIncidentFeedbackResultItem = /** @class */ (function () {
                    function GetIncidentFeedbackResultItem() {
                    }
                    GetIncidentFeedbackResultItem.TYPE_NAME = 'GetIncidentFeedbackResultItem';
                    return GetIncidentFeedbackResultItem;
                }());
                Queries.GetIncidentFeedbackResultItem = GetIncidentFeedbackResultItem;
            })(Queries = Feedback.Queries || (Feedback.Queries = {}));
        })(Feedback = Web.Feedback || (Web.Feedback = {}));
    })(Web = codeRR.Web || (codeRR.Web = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Versions;
        (function (Versions) {
            var Queries;
            (function (Queries) {
                var GetApplicationVersions = /** @class */ (function () {
                    function GetApplicationVersions(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetApplicationVersions.TYPE_NAME = 'GetApplicationVersions';
                    return GetApplicationVersions;
                }());
                Queries.GetApplicationVersions = GetApplicationVersions;
                var GetApplicationVersionsResult = /** @class */ (function () {
                    function GetApplicationVersionsResult() {
                    }
                    GetApplicationVersionsResult.TYPE_NAME = 'GetApplicationVersionsResult';
                    return GetApplicationVersionsResult;
                }());
                Queries.GetApplicationVersionsResult = GetApplicationVersionsResult;
                var GetApplicationVersionsResultItem = /** @class */ (function () {
                    function GetApplicationVersionsResultItem() {
                    }
                    GetApplicationVersionsResultItem.TYPE_NAME = 'GetApplicationVersionsResultItem';
                    return GetApplicationVersionsResultItem;
                }());
                Queries.GetApplicationVersionsResultItem = GetApplicationVersionsResultItem;
            })(Queries = Versions.Queries || (Versions.Queries = {}));
        })(Versions = Modules.Versions || (Modules.Versions = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var LastTriggerActionDTO;
            (function (LastTriggerActionDTO) {
                LastTriggerActionDTO[LastTriggerActionDTO["ExecuteActions"] = 0] = "ExecuteActions";
                LastTriggerActionDTO[LastTriggerActionDTO["AbortTrigger"] = 1] = "AbortTrigger";
            })(LastTriggerActionDTO = Triggers.LastTriggerActionDTO || (Triggers.LastTriggerActionDTO = {}));
            var TriggerActionDataDTO = /** @class */ (function () {
                function TriggerActionDataDTO() {
                }
                TriggerActionDataDTO.TYPE_NAME = 'TriggerActionDataDTO';
                return TriggerActionDataDTO;
            }());
            Triggers.TriggerActionDataDTO = TriggerActionDataDTO;
            var TriggerContextRule = /** @class */ (function () {
                function TriggerContextRule() {
                }
                TriggerContextRule.TYPE_NAME = 'TriggerContextRule';
                return TriggerContextRule;
            }());
            Triggers.TriggerContextRule = TriggerContextRule;
            var TriggerDTO = /** @class */ (function () {
                function TriggerDTO() {
                }
                TriggerDTO.TYPE_NAME = 'TriggerDTO';
                return TriggerDTO;
            }());
            Triggers.TriggerDTO = TriggerDTO;
            var TriggerExceptionRule = /** @class */ (function () {
                function TriggerExceptionRule() {
                }
                TriggerExceptionRule.TYPE_NAME = 'TriggerExceptionRule';
                return TriggerExceptionRule;
            }());
            Triggers.TriggerExceptionRule = TriggerExceptionRule;
            var TriggerFilterCondition;
            (function (TriggerFilterCondition) {
                TriggerFilterCondition[TriggerFilterCondition["StartsWith"] = 0] = "StartsWith";
                TriggerFilterCondition[TriggerFilterCondition["EndsWith"] = 1] = "EndsWith";
                TriggerFilterCondition[TriggerFilterCondition["Contains"] = 2] = "Contains";
                TriggerFilterCondition[TriggerFilterCondition["DoNotContain"] = 3] = "DoNotContain";
                TriggerFilterCondition[TriggerFilterCondition["Equals"] = 4] = "Equals";
            })(TriggerFilterCondition = Triggers.TriggerFilterCondition || (Triggers.TriggerFilterCondition = {}));
            var TriggerRuleAction;
            (function (TriggerRuleAction) {
                TriggerRuleAction[TriggerRuleAction["AbortTrigger"] = 0] = "AbortTrigger";
                TriggerRuleAction[TriggerRuleAction["ContinueWithNextRule"] = 1] = "ContinueWithNextRule";
                TriggerRuleAction[TriggerRuleAction["ExecuteActions"] = 2] = "ExecuteActions";
            })(TriggerRuleAction = Triggers.TriggerRuleAction || (Triggers.TriggerRuleAction = {}));
            var TriggerRuleBase = /** @class */ (function () {
                function TriggerRuleBase() {
                }
                TriggerRuleBase.TYPE_NAME = 'TriggerRuleBase';
                return TriggerRuleBase;
            }());
            Triggers.TriggerRuleBase = TriggerRuleBase;
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var Queries;
            (function (Queries) {
                var GetContextCollectionMetadata = /** @class */ (function () {
                    function GetContextCollectionMetadata(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetContextCollectionMetadata.TYPE_NAME = 'GetContextCollectionMetadata';
                    return GetContextCollectionMetadata;
                }());
                Queries.GetContextCollectionMetadata = GetContextCollectionMetadata;
                var GetContextCollectionMetadataItem = /** @class */ (function () {
                    function GetContextCollectionMetadataItem() {
                    }
                    GetContextCollectionMetadataItem.TYPE_NAME = 'GetContextCollectionMetadataItem';
                    return GetContextCollectionMetadataItem;
                }());
                Queries.GetContextCollectionMetadataItem = GetContextCollectionMetadataItem;
                var GetTrigger = /** @class */ (function () {
                    function GetTrigger(id) {
                        this.Id = id;
                    }
                    GetTrigger.TYPE_NAME = 'GetTrigger';
                    return GetTrigger;
                }());
                Queries.GetTrigger = GetTrigger;
                var GetTriggerDTO = /** @class */ (function () {
                    function GetTriggerDTO() {
                    }
                    GetTriggerDTO.TYPE_NAME = 'GetTriggerDTO';
                    return GetTriggerDTO;
                }());
                Queries.GetTriggerDTO = GetTriggerDTO;
                var GetTriggersForApplication = /** @class */ (function () {
                    function GetTriggersForApplication(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetTriggersForApplication.TYPE_NAME = 'GetTriggersForApplication';
                    return GetTriggersForApplication;
                }());
                Queries.GetTriggersForApplication = GetTriggersForApplication;
            })(Queries = Triggers.Queries || (Triggers.Queries = {}));
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var Commands;
            (function (Commands) {
                var CreateTrigger = /** @class */ (function () {
                    function CreateTrigger(applicationId, name) {
                        this.ApplicationId = applicationId;
                        this.Name = name;
                    }
                    CreateTrigger.TYPE_NAME = 'CreateTrigger';
                    return CreateTrigger;
                }());
                Commands.CreateTrigger = CreateTrigger;
                var DeleteTrigger = /** @class */ (function () {
                    function DeleteTrigger(id) {
                        this.Id = id;
                    }
                    DeleteTrigger.TYPE_NAME = 'DeleteTrigger';
                    return DeleteTrigger;
                }());
                Commands.DeleteTrigger = DeleteTrigger;
                var UpdateTrigger = /** @class */ (function () {
                    function UpdateTrigger(id, name) {
                        this.Id = id;
                        this.Name = name;
                    }
                    UpdateTrigger.TYPE_NAME = 'UpdateTrigger';
                    return UpdateTrigger;
                }());
                Commands.UpdateTrigger = UpdateTrigger;
            })(Commands = Triggers.Commands || (Triggers.Commands = {}));
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var TagDTO = /** @class */ (function () {
                function TagDTO() {
                }
                TagDTO.TYPE_NAME = 'TagDTO';
                return TagDTO;
            }());
            Tagging.TagDTO = TagDTO;
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var Queries;
            (function (Queries) {
                var GetTagsForApplication = /** @class */ (function () {
                    function GetTagsForApplication(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetTagsForApplication.TYPE_NAME = 'GetTagsForApplication';
                    return GetTagsForApplication;
                }());
                Queries.GetTagsForApplication = GetTagsForApplication;
                var GetTagsForIncident = /** @class */ (function () {
                    function GetTagsForIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetTagsForIncident.TYPE_NAME = 'GetTagsForIncident';
                    return GetTagsForIncident;
                }());
                Queries.GetTagsForIncident = GetTagsForIncident;
            })(Queries = Tagging.Queries || (Tagging.Queries = {}));
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var Events;
            (function (Events) {
                var TagAttachedToIncident = /** @class */ (function () {
                    function TagAttachedToIncident(incidentId, tags) {
                        this.IncidentId = incidentId;
                        this.Tags = tags;
                    }
                    TagAttachedToIncident.TYPE_NAME = 'TagAttachedToIncident';
                    return TagAttachedToIncident;
                }());
                Events.TagAttachedToIncident = TagAttachedToIncident;
            })(Events = Tagging.Events || (Tagging.Events = {}));
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var ErrorOrigins;
        (function (ErrorOrigins) {
            var Queries;
            (function (Queries) {
                var GetOriginsForIncident = /** @class */ (function () {
                    function GetOriginsForIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetOriginsForIncident.TYPE_NAME = 'GetOriginsForIncident';
                    return GetOriginsForIncident;
                }());
                Queries.GetOriginsForIncident = GetOriginsForIncident;
                var GetOriginsForIncidentResult = /** @class */ (function () {
                    function GetOriginsForIncidentResult() {
                    }
                    GetOriginsForIncidentResult.TYPE_NAME = 'GetOriginsForIncidentResult';
                    return GetOriginsForIncidentResult;
                }());
                Queries.GetOriginsForIncidentResult = GetOriginsForIncidentResult;
                var GetOriginsForIncidentResultItem = /** @class */ (function () {
                    function GetOriginsForIncidentResultItem() {
                    }
                    GetOriginsForIncidentResultItem.TYPE_NAME = 'GetOriginsForIncidentResultItem';
                    return GetOriginsForIncidentResultItem;
                }());
                Queries.GetOriginsForIncidentResultItem = GetOriginsForIncidentResultItem;
            })(Queries = ErrorOrigins.Queries || (ErrorOrigins.Queries = {}));
        })(ErrorOrigins = Modules.ErrorOrigins || (Modules.ErrorOrigins = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Modules;
    (function (Modules) {
        var ContextData;
        (function (ContextData) {
            var Queries;
            (function (Queries) {
                var GetSimilarities = /** @class */ (function () {
                    function GetSimilarities(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetSimilarities.TYPE_NAME = 'GetSimilarities';
                    return GetSimilarities;
                }());
                Queries.GetSimilarities = GetSimilarities;
                var GetSimilaritiesCollection = /** @class */ (function () {
                    function GetSimilaritiesCollection() {
                    }
                    GetSimilaritiesCollection.TYPE_NAME = 'GetSimilaritiesCollection';
                    return GetSimilaritiesCollection;
                }());
                Queries.GetSimilaritiesCollection = GetSimilaritiesCollection;
                var GetSimilaritiesResult = /** @class */ (function () {
                    function GetSimilaritiesResult() {
                    }
                    GetSimilaritiesResult.TYPE_NAME = 'GetSimilaritiesResult';
                    return GetSimilaritiesResult;
                }());
                Queries.GetSimilaritiesResult = GetSimilaritiesResult;
                var GetSimilaritiesSimilarity = /** @class */ (function () {
                    function GetSimilaritiesSimilarity(name) {
                        this.Name = name;
                    }
                    GetSimilaritiesSimilarity.TYPE_NAME = 'GetSimilaritiesSimilarity';
                    return GetSimilaritiesSimilarity;
                }());
                Queries.GetSimilaritiesSimilarity = GetSimilaritiesSimilarity;
                var GetSimilaritiesValue = /** @class */ (function () {
                    function GetSimilaritiesValue(value, percentage, count) {
                        this.Value = value;
                        this.Percentage = percentage;
                        this.Count = count;
                    }
                    GetSimilaritiesValue.TYPE_NAME = 'GetSimilaritiesValue';
                    return GetSimilaritiesValue;
                }());
                Queries.GetSimilaritiesValue = GetSimilaritiesValue;
            })(Queries = ContextData.Queries || (ContextData.Queries = {}));
        })(ContextData = Modules.ContextData || (Modules.ContextData = {}));
    })(Modules = codeRR.Modules || (codeRR.Modules = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var EnumExtensions = /** @class */ (function () {
            function EnumExtensions() {
            }
            EnumExtensions.TYPE_NAME = 'EnumExtensions';
            return EnumExtensions;
        }());
        Core.EnumExtensions = EnumExtensions;
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var NotificationSettings = /** @class */ (function () {
                function NotificationSettings() {
                }
                NotificationSettings.TYPE_NAME = 'NotificationSettings';
                return NotificationSettings;
            }());
            Users.NotificationSettings = NotificationSettings;
            var NotificationState;
            (function (NotificationState) {
                NotificationState[NotificationState["UseGlobalSetting"] = 0] = "UseGlobalSetting";
                NotificationState[NotificationState["Disabled"] = 1] = "Disabled";
                NotificationState[NotificationState["Cellphone"] = 2] = "Cellphone";
                NotificationState[NotificationState["Email"] = 3] = "Email";
            })(NotificationState = Users.NotificationState || (Users.NotificationState = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var Queries;
            (function (Queries) {
                var GetUserSettings = /** @class */ (function () {
                    function GetUserSettings() {
                    }
                    GetUserSettings.TYPE_NAME = 'GetUserSettings';
                    return GetUserSettings;
                }());
                Queries.GetUserSettings = GetUserSettings;
                var GetUserSettingsResult = /** @class */ (function () {
                    function GetUserSettingsResult() {
                    }
                    GetUserSettingsResult.TYPE_NAME = 'GetUserSettingsResult';
                    return GetUserSettingsResult;
                }());
                Queries.GetUserSettingsResult = GetUserSettingsResult;
            })(Queries = Users.Queries || (Users.Queries = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var Commands;
            (function (Commands) {
                var UpdateNotifications = /** @class */ (function () {
                    function UpdateNotifications() {
                    }
                    UpdateNotifications.TYPE_NAME = 'UpdateNotifications';
                    return UpdateNotifications;
                }());
                Commands.UpdateNotifications = UpdateNotifications;
                var UpdatePersonalSettings = /** @class */ (function () {
                    function UpdatePersonalSettings() {
                    }
                    UpdatePersonalSettings.TYPE_NAME = 'UpdatePersonalSettings';
                    return UpdatePersonalSettings;
                }());
                Commands.UpdatePersonalSettings = UpdatePersonalSettings;
            })(Commands = Users.Commands || (Users.Commands = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Support;
        (function (Support) {
            var SendSupportRequest = /** @class */ (function () {
                function SendSupportRequest() {
                }
                SendSupportRequest.TYPE_NAME = 'SendSupportRequest';
                return SendSupportRequest;
            }());
            Support.SendSupportRequest = SendSupportRequest;
        })(Support = Core.Support || (Core.Support = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Reports;
        (function (Reports) {
            var ContextCollectionDTO = /** @class */ (function () {
                function ContextCollectionDTO(name, items) {
                    this.Name = name;
                }
                ContextCollectionDTO.TYPE_NAME = 'ContextCollectionDTO';
                return ContextCollectionDTO;
            }());
            Reports.ContextCollectionDTO = ContextCollectionDTO;
            var ReportDTO = /** @class */ (function () {
                function ReportDTO() {
                }
                ReportDTO.TYPE_NAME = 'ReportDTO';
                return ReportDTO;
            }());
            Reports.ReportDTO = ReportDTO;
            var ReportExeptionDTO = /** @class */ (function () {
                function ReportExeptionDTO() {
                }
                ReportExeptionDTO.TYPE_NAME = 'ReportExeptionDTO';
                return ReportExeptionDTO;
            }());
            Reports.ReportExeptionDTO = ReportExeptionDTO;
        })(Reports = Core.Reports || (Core.Reports = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Reports;
        (function (Reports) {
            var Queries;
            (function (Queries) {
                var GetReport = /** @class */ (function () {
                    function GetReport(reportId) {
                        this.ReportId = reportId;
                    }
                    GetReport.TYPE_NAME = 'GetReport';
                    return GetReport;
                }());
                Queries.GetReport = GetReport;
                var GetReportException = /** @class */ (function () {
                    function GetReportException() {
                    }
                    GetReportException.TYPE_NAME = 'GetReportException';
                    return GetReportException;
                }());
                Queries.GetReportException = GetReportException;
                var GetReportList = /** @class */ (function () {
                    function GetReportList(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetReportList.TYPE_NAME = 'GetReportList';
                    return GetReportList;
                }());
                Queries.GetReportList = GetReportList;
                var GetReportListResult = /** @class */ (function () {
                    function GetReportListResult(items) {
                        this.Items = items;
                    }
                    GetReportListResult.TYPE_NAME = 'GetReportListResult';
                    return GetReportListResult;
                }());
                Queries.GetReportListResult = GetReportListResult;
                var GetReportListResultItem = /** @class */ (function () {
                    function GetReportListResultItem() {
                    }
                    GetReportListResultItem.TYPE_NAME = 'GetReportListResultItem';
                    return GetReportListResultItem;
                }());
                Queries.GetReportListResultItem = GetReportListResultItem;
                var GetReportResult = /** @class */ (function () {
                    function GetReportResult() {
                    }
                    GetReportResult.TYPE_NAME = 'GetReportResult';
                    return GetReportResult;
                }());
                Queries.GetReportResult = GetReportResult;
                var GetReportResultContextCollection = /** @class */ (function () {
                    function GetReportResultContextCollection(name, properties) {
                        this.Name = name;
                        this.Properties = properties;
                    }
                    GetReportResultContextCollection.TYPE_NAME = 'GetReportResultContextCollection';
                    return GetReportResultContextCollection;
                }());
                Queries.GetReportResultContextCollection = GetReportResultContextCollection;
                var KeyValuePair = /** @class */ (function () {
                    function KeyValuePair(key, value) {
                        this.Key = key;
                        this.Value = value;
                    }
                    KeyValuePair.TYPE_NAME = 'KeyValuePair';
                    return KeyValuePair;
                }());
                Queries.KeyValuePair = KeyValuePair;
            })(Queries = Reports.Queries || (Reports.Queries = {}));
        })(Reports = Core.Reports || (Core.Reports = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Notifications;
        (function (Notifications) {
            var AddNotification = /** @class */ (function () {
                function AddNotification(accountId, message) {
                    this.AccountId = accountId;
                    this.Message = message;
                }
                AddNotification.TYPE_NAME = 'AddNotification';
                return AddNotification;
            }());
            Notifications.AddNotification = AddNotification;
        })(Notifications = Core.Notifications || (Core.Notifications = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Messaging;
        (function (Messaging) {
            var EmailAddress = /** @class */ (function () {
                function EmailAddress(address) {
                    this.Address = address;
                }
                EmailAddress.TYPE_NAME = 'EmailAddress';
                return EmailAddress;
            }());
            Messaging.EmailAddress = EmailAddress;
            var EmailMessage = /** @class */ (function () {
                function EmailMessage() {
                }
                EmailMessage.TYPE_NAME = 'EmailMessage';
                return EmailMessage;
            }());
            Messaging.EmailMessage = EmailMessage;
            var EmailResource = /** @class */ (function () {
                function EmailResource(name, content) {
                    this.Name = name;
                    this.Content = content;
                }
                EmailResource.TYPE_NAME = 'EmailResource';
                return EmailResource;
            }());
            Messaging.EmailResource = EmailResource;
        })(Messaging = Core.Messaging || (Core.Messaging = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Messaging;
        (function (Messaging) {
            var Commands;
            (function (Commands) {
                var SendEmail = /** @class */ (function () {
                    function SendEmail() {
                    }
                    SendEmail.TYPE_NAME = 'SendEmail';
                    return SendEmail;
                }());
                Commands.SendEmail = SendEmail;
                var SendSms = /** @class */ (function () {
                    function SendSms(phoneNumber, message) {
                        this.PhoneNumber = phoneNumber;
                        this.Message = message;
                    }
                    SendSms.TYPE_NAME = 'SendSms';
                    return SendSms;
                }());
                Commands.SendSms = SendSms;
                var SendTemplateEmail = /** @class */ (function () {
                    function SendTemplateEmail(mailTitle, templateName) {
                        this.MailTitle = mailTitle;
                        this.TemplateName = templateName;
                    }
                    SendTemplateEmail.TYPE_NAME = 'SendTemplateEmail';
                    return SendTemplateEmail;
                }());
                Commands.SendTemplateEmail = SendTemplateEmail;
            })(Commands = Messaging.Commands || (Messaging.Commands = {}));
        })(Messaging = Core.Messaging || (Core.Messaging = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Invitations;
        (function (Invitations) {
            var Queries;
            (function (Queries) {
                var GetInvitationByKey = /** @class */ (function () {
                    function GetInvitationByKey(invitationKey) {
                        this.InvitationKey = invitationKey;
                    }
                    GetInvitationByKey.TYPE_NAME = 'GetInvitationByKey';
                    return GetInvitationByKey;
                }());
                Queries.GetInvitationByKey = GetInvitationByKey;
                var GetInvitationByKeyResult = /** @class */ (function () {
                    function GetInvitationByKeyResult() {
                    }
                    GetInvitationByKeyResult.TYPE_NAME = 'GetInvitationByKeyResult';
                    return GetInvitationByKeyResult;
                }());
                Queries.GetInvitationByKeyResult = GetInvitationByKeyResult;
            })(Queries = Invitations.Queries || (Invitations.Queries = {}));
        })(Invitations = Core.Invitations || (Core.Invitations = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Invitations;
        (function (Invitations) {
            var Commands;
            (function (Commands) {
                var InviteUser = /** @class */ (function () {
                    function InviteUser(applicationId, emailAddress) {
                        this.ApplicationId = applicationId;
                        this.EmailAddress = emailAddress;
                    }
                    InviteUser.TYPE_NAME = 'InviteUser';
                    return InviteUser;
                }());
                Commands.InviteUser = InviteUser;
            })(Commands = Invitations.Commands || (Invitations.Commands = {}));
        })(Invitations = Core.Invitations || (Core.Invitations = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var IncidentOrder;
            (function (IncidentOrder) {
                IncidentOrder[IncidentOrder["Newest"] = 0] = "Newest";
                IncidentOrder[IncidentOrder["MostReports"] = 1] = "MostReports";
                IncidentOrder[IncidentOrder["MostFeedback"] = 2] = "MostFeedback";
            })(IncidentOrder = Incidents.IncidentOrder || (Incidents.IncidentOrder = {}));
            var IncidentSummaryDTO = /** @class */ (function () {
                function IncidentSummaryDTO(id, name) {
                    this.Id = id;
                    this.Name = name;
                }
                IncidentSummaryDTO.TYPE_NAME = 'IncidentSummaryDTO';
                return IncidentSummaryDTO;
            }());
            Incidents.IncidentSummaryDTO = IncidentSummaryDTO;
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Queries;
            (function (Queries) {
                var FindIncidents = /** @class */ (function () {
                    function FindIncidents() {
                    }
                    FindIncidents.TYPE_NAME = 'FindIncidents';
                    return FindIncidents;
                }());
                Queries.FindIncidents = FindIncidents;
                var FindIncidentsResult = /** @class */ (function () {
                    function FindIncidentsResult() {
                    }
                    FindIncidentsResult.TYPE_NAME = 'FindIncidentsResult';
                    return FindIncidentsResult;
                }());
                Queries.FindIncidentsResult = FindIncidentsResult;
                var FindIncidentsResultItem = /** @class */ (function () {
                    function FindIncidentsResultItem(id, name) {
                        this.Id = id;
                        this.Name = name;
                    }
                    FindIncidentsResultItem.TYPE_NAME = 'FindIncidentsResultItem';
                    return FindIncidentsResultItem;
                }());
                Queries.FindIncidentsResultItem = FindIncidentsResultItem;
                var GetIncident = /** @class */ (function () {
                    function GetIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncident.TYPE_NAME = 'GetIncident';
                    return GetIncident;
                }());
                Queries.GetIncident = GetIncident;
                var GetIncidentForClosePage = /** @class */ (function () {
                    function GetIncidentForClosePage(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncidentForClosePage.TYPE_NAME = 'GetIncidentForClosePage';
                    return GetIncidentForClosePage;
                }());
                Queries.GetIncidentForClosePage = GetIncidentForClosePage;
                var GetIncidentForClosePageResult = /** @class */ (function () {
                    function GetIncidentForClosePageResult() {
                    }
                    GetIncidentForClosePageResult.TYPE_NAME = 'GetIncidentForClosePageResult';
                    return GetIncidentForClosePageResult;
                }());
                Queries.GetIncidentForClosePageResult = GetIncidentForClosePageResult;
                var GetIncidentResult = /** @class */ (function () {
                    function GetIncidentResult() {
                    }
                    GetIncidentResult.TYPE_NAME = 'GetIncidentResult';
                    return GetIncidentResult;
                }());
                Queries.GetIncidentResult = GetIncidentResult;
                var GetIncidentStatistics = /** @class */ (function () {
                    function GetIncidentStatistics() {
                    }
                    GetIncidentStatistics.TYPE_NAME = 'GetIncidentStatistics';
                    return GetIncidentStatistics;
                }());
                Queries.GetIncidentStatistics = GetIncidentStatistics;
                var GetIncidentStatisticsResult = /** @class */ (function () {
                    function GetIncidentStatisticsResult() {
                    }
                    GetIncidentStatisticsResult.TYPE_NAME = 'GetIncidentStatisticsResult';
                    return GetIncidentStatisticsResult;
                }());
                Queries.GetIncidentStatisticsResult = GetIncidentStatisticsResult;
                var HighlightedContextData = /** @class */ (function () {
                    function HighlightedContextData() {
                    }
                    HighlightedContextData.TYPE_NAME = 'HighlightedContextData';
                    return HighlightedContextData;
                }());
                Queries.HighlightedContextData = HighlightedContextData;
                var QuickFact = /** @class */ (function () {
                    function QuickFact() {
                    }
                    QuickFact.TYPE_NAME = 'QuickFact';
                    return QuickFact;
                }());
                Queries.QuickFact = QuickFact;
                var ReportDay = /** @class */ (function () {
                    function ReportDay() {
                    }
                    ReportDay.TYPE_NAME = 'ReportDay';
                    return ReportDay;
                }());
                Queries.ReportDay = ReportDay;
                var SuggestedIncidentSolution = /** @class */ (function () {
                    function SuggestedIncidentSolution() {
                    }
                    SuggestedIncidentSolution.TYPE_NAME = 'SuggestedIncidentSolution';
                    return SuggestedIncidentSolution;
                }());
                Queries.SuggestedIncidentSolution = SuggestedIncidentSolution;
            })(Queries = Incidents.Queries || (Incidents.Queries = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Events;
            (function (Events) {
                var IncidentAssigned = /** @class */ (function () {
                    function IncidentAssigned(incidentId, assignedById, assignedToId) {
                        this.IncidentId = incidentId;
                        this.AssignedById = assignedById;
                        this.AssignedToId = assignedToId;
                    }
                    IncidentAssigned.TYPE_NAME = 'IncidentAssigned';
                    return IncidentAssigned;
                }());
                Events.IncidentAssigned = IncidentAssigned;
                var IncidentIgnored = /** @class */ (function () {
                    function IncidentIgnored(incidentId, accountId, userName) {
                        this.IncidentId = incidentId;
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    IncidentIgnored.TYPE_NAME = 'IncidentIgnored';
                    return IncidentIgnored;
                }());
                Events.IncidentIgnored = IncidentIgnored;
                var IncidentReOpened = /** @class */ (function () {
                    function IncidentReOpened(applicationId, incidentId, createdAtUtc) {
                        this.ApplicationId = applicationId;
                        this.IncidentId = incidentId;
                        this.CreatedAtUtc = createdAtUtc;
                    }
                    IncidentReOpened.TYPE_NAME = 'IncidentReOpened';
                    return IncidentReOpened;
                }());
                Events.IncidentReOpened = IncidentReOpened;
                var ReportAddedToIncident = /** @class */ (function () {
                    function ReportAddedToIncident(incident, report, isReOpened) {
                        this.Incident = incident;
                        this.Report = report;
                        this.IsReOpened = isReOpened;
                    }
                    ReportAddedToIncident.TYPE_NAME = 'ReportAddedToIncident';
                    return ReportAddedToIncident;
                }());
                Events.ReportAddedToIncident = ReportAddedToIncident;
            })(Events = Incidents.Events || (Incidents.Events = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Commands;
            (function (Commands) {
                var AssignIncident = /** @class */ (function () {
                    function AssignIncident(incidentId, assignedTo, assignedBy) {
                        this.IncidentId = incidentId;
                        this.AssignedTo = assignedTo;
                        this.AssignedBy = assignedBy;
                    }
                    AssignIncident.TYPE_NAME = 'AssignIncident';
                    return AssignIncident;
                }());
                Commands.AssignIncident = AssignIncident;
                var CloseIncident = /** @class */ (function () {
                    function CloseIncident(solution, incidentId) {
                        this.Solution = solution;
                        this.IncidentId = incidentId;
                    }
                    CloseIncident.TYPE_NAME = 'CloseIncident';
                    return CloseIncident;
                }());
                Commands.CloseIncident = CloseIncident;
                var IgnoreIncident = /** @class */ (function () {
                    function IgnoreIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    IgnoreIncident.TYPE_NAME = 'IgnoreIncident';
                    return IgnoreIncident;
                }());
                Commands.IgnoreIncident = IgnoreIncident;
                var ReOpenIncident = /** @class */ (function () {
                    function ReOpenIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    ReOpenIncident.TYPE_NAME = 'ReOpenIncident';
                    return ReOpenIncident;
                }());
                Commands.ReOpenIncident = ReOpenIncident;
            })(Commands = Incidents.Commands || (Incidents.Commands = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Feedback;
        (function (Feedback) {
            var Events;
            (function (Events) {
                var FeedbackAttachedToIncident = /** @class */ (function () {
                    function FeedbackAttachedToIncident() {
                    }
                    FeedbackAttachedToIncident.TYPE_NAME = 'FeedbackAttachedToIncident';
                    return FeedbackAttachedToIncident;
                }());
                Events.FeedbackAttachedToIncident = FeedbackAttachedToIncident;
            })(Events = Feedback.Events || (Feedback.Events = {}));
        })(Feedback = Core.Feedback || (Core.Feedback = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Feedback;
        (function (Feedback) {
            var Commands;
            (function (Commands) {
                var SubmitFeedback = /** @class */ (function () {
                    function SubmitFeedback(errorId, remoteAddress) {
                        this.ErrorId = errorId;
                        this.RemoteAddress = remoteAddress;
                    }
                    SubmitFeedback.TYPE_NAME = 'SubmitFeedback';
                    return SubmitFeedback;
                }());
                Commands.SubmitFeedback = SubmitFeedback;
            })(Commands = Feedback.Commands || (Feedback.Commands = {}));
        })(Feedback = Core.Feedback || (Core.Feedback = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var ApplicationListItem = /** @class */ (function () {
                function ApplicationListItem(id, name) {
                    this.Id = id;
                    this.Name = name;
                }
                ApplicationListItem.TYPE_NAME = 'ApplicationListItem';
                return ApplicationListItem;
            }());
            Applications.ApplicationListItem = ApplicationListItem;
            var TypeOfApplication;
            (function (TypeOfApplication) {
                TypeOfApplication[TypeOfApplication["Mobile"] = 0] = "Mobile";
                TypeOfApplication[TypeOfApplication["DesktopApplication"] = 1] = "DesktopApplication";
                TypeOfApplication[TypeOfApplication["Server"] = 2] = "Server";
            })(TypeOfApplication = Applications.TypeOfApplication || (Applications.TypeOfApplication = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Queries;
            (function (Queries) {
                var GetApplicationIdByKey = /** @class */ (function () {
                    function GetApplicationIdByKey(applicationKey) {
                        this.ApplicationKey = applicationKey;
                    }
                    GetApplicationIdByKey.TYPE_NAME = 'GetApplicationIdByKey';
                    return GetApplicationIdByKey;
                }());
                Queries.GetApplicationIdByKey = GetApplicationIdByKey;
                var GetApplicationIdByKeyResult = /** @class */ (function () {
                    function GetApplicationIdByKeyResult() {
                    }
                    GetApplicationIdByKeyResult.TYPE_NAME = 'GetApplicationIdByKeyResult';
                    return GetApplicationIdByKeyResult;
                }());
                Queries.GetApplicationIdByKeyResult = GetApplicationIdByKeyResult;
                var GetApplicationInfo = /** @class */ (function () {
                    function GetApplicationInfo() {
                    }
                    GetApplicationInfo.TYPE_NAME = 'GetApplicationInfo';
                    return GetApplicationInfo;
                }());
                Queries.GetApplicationInfo = GetApplicationInfo;
                var GetApplicationInfoResult = /** @class */ (function () {
                    function GetApplicationInfoResult() {
                    }
                    GetApplicationInfoResult.TYPE_NAME = 'GetApplicationInfoResult';
                    return GetApplicationInfoResult;
                }());
                Queries.GetApplicationInfoResult = GetApplicationInfoResult;
                var GetApplicationList = /** @class */ (function () {
                    function GetApplicationList() {
                    }
                    GetApplicationList.TYPE_NAME = 'GetApplicationList';
                    return GetApplicationList;
                }());
                Queries.GetApplicationList = GetApplicationList;
                var GetApplicationOverview = /** @class */ (function () {
                    function GetApplicationOverview(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetApplicationOverview.TYPE_NAME = 'GetApplicationOverview';
                    return GetApplicationOverview;
                }());
                Queries.GetApplicationOverview = GetApplicationOverview;
                var GetApplicationOverviewResult = /** @class */ (function () {
                    function GetApplicationOverviewResult() {
                    }
                    GetApplicationOverviewResult.TYPE_NAME = 'GetApplicationOverviewResult';
                    return GetApplicationOverviewResult;
                }());
                Queries.GetApplicationOverviewResult = GetApplicationOverviewResult;
                var GetApplicationTeam = /** @class */ (function () {
                    function GetApplicationTeam(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetApplicationTeam.TYPE_NAME = 'GetApplicationTeam';
                    return GetApplicationTeam;
                }());
                Queries.GetApplicationTeam = GetApplicationTeam;
                var GetApplicationTeamMember = /** @class */ (function () {
                    function GetApplicationTeamMember() {
                    }
                    GetApplicationTeamMember.TYPE_NAME = 'GetApplicationTeamMember';
                    return GetApplicationTeamMember;
                }());
                Queries.GetApplicationTeamMember = GetApplicationTeamMember;
                var GetApplicationTeamResult = /** @class */ (function () {
                    function GetApplicationTeamResult() {
                    }
                    GetApplicationTeamResult.TYPE_NAME = 'GetApplicationTeamResult';
                    return GetApplicationTeamResult;
                }());
                Queries.GetApplicationTeamResult = GetApplicationTeamResult;
                var GetApplicationTeamResultInvitation = /** @class */ (function () {
                    function GetApplicationTeamResultInvitation() {
                    }
                    GetApplicationTeamResultInvitation.TYPE_NAME = 'GetApplicationTeamResultInvitation';
                    return GetApplicationTeamResultInvitation;
                }());
                Queries.GetApplicationTeamResultInvitation = GetApplicationTeamResultInvitation;
                var OverviewStatSummary = /** @class */ (function () {
                    function OverviewStatSummary() {
                    }
                    OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                    return OverviewStatSummary;
                }());
                Queries.OverviewStatSummary = OverviewStatSummary;
            })(Queries = Applications.Queries || (Applications.Queries = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Events;
            (function (Events) {
                var ApplicationCreated = /** @class */ (function () {
                    function ApplicationCreated(id, name, createdById, appKey, sharedSecret) {
                        this.CreatedById = createdById;
                        this.AppKey = appKey;
                        this.SharedSecret = sharedSecret;
                    }
                    ApplicationCreated.TYPE_NAME = 'ApplicationCreated';
                    return ApplicationCreated;
                }());
                Events.ApplicationCreated = ApplicationCreated;
                var ApplicationDeleted = /** @class */ (function () {
                    function ApplicationDeleted() {
                    }
                    ApplicationDeleted.TYPE_NAME = 'ApplicationDeleted';
                    return ApplicationDeleted;
                }());
                Events.ApplicationDeleted = ApplicationDeleted;
                var UserAddedToApplication = /** @class */ (function () {
                    function UserAddedToApplication(applicationId, accountId) {
                        this.ApplicationId = applicationId;
                        this.AccountId = accountId;
                    }
                    UserAddedToApplication.TYPE_NAME = 'UserAddedToApplication';
                    return UserAddedToApplication;
                }());
                Events.UserAddedToApplication = UserAddedToApplication;
                var UserInvitedToApplication = /** @class */ (function () {
                    function UserInvitedToApplication(invitationKey, applicationId, applicationName, emailAddress, invitedBy) {
                        this.InvitationKey = invitationKey;
                        this.ApplicationId = applicationId;
                        this.ApplicationName = applicationName;
                        this.EmailAddress = emailAddress;
                        this.InvitedBy = invitedBy;
                    }
                    UserInvitedToApplication.TYPE_NAME = 'UserInvitedToApplication';
                    return UserInvitedToApplication;
                }());
                Events.UserInvitedToApplication = UserInvitedToApplication;
            })(Events = Applications.Events || (Applications.Events = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Commands;
            (function (Commands) {
                var CreateApplication = /** @class */ (function () {
                    function CreateApplication(name, typeOfApplication) {
                        this.Name = name;
                        this.TypeOfApplication = typeOfApplication;
                    }
                    CreateApplication.TYPE_NAME = 'CreateApplication';
                    return CreateApplication;
                }());
                Commands.CreateApplication = CreateApplication;
                var DeleteApplication = /** @class */ (function () {
                    function DeleteApplication(id) {
                        this.Id = id;
                    }
                    DeleteApplication.TYPE_NAME = 'DeleteApplication';
                    return DeleteApplication;
                }());
                Commands.DeleteApplication = DeleteApplication;
                var RemoveTeamMember = /** @class */ (function () {
                    function RemoveTeamMember(applicationId, userToRemove) {
                        this.ApplicationId = applicationId;
                        this.UserToRemove = userToRemove;
                    }
                    RemoveTeamMember.TYPE_NAME = 'RemoveTeamMember';
                    return RemoveTeamMember;
                }());
                Commands.RemoveTeamMember = RemoveTeamMember;
                var UpdateApplication = /** @class */ (function () {
                    function UpdateApplication(applicationId, name) {
                        this.ApplicationId = applicationId;
                        this.Name = name;
                    }
                    UpdateApplication.TYPE_NAME = 'UpdateApplication';
                    return UpdateApplication;
                }());
                Commands.UpdateApplication = UpdateApplication;
            })(Commands = Applications.Commands || (Applications.Commands = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Queries;
            (function (Queries) {
                var GetApiKey = /** @class */ (function () {
                    function GetApiKey(id) {
                        this.Id = id;
                    }
                    GetApiKey.TYPE_NAME = 'GetApiKey';
                    return GetApiKey;
                }());
                Queries.GetApiKey = GetApiKey;
                var GetApiKeyResult = /** @class */ (function () {
                    function GetApiKeyResult() {
                    }
                    GetApiKeyResult.TYPE_NAME = 'GetApiKeyResult';
                    return GetApiKeyResult;
                }());
                Queries.GetApiKeyResult = GetApiKeyResult;
                var GetApiKeyResultApplication = /** @class */ (function () {
                    function GetApiKeyResultApplication() {
                    }
                    GetApiKeyResultApplication.TYPE_NAME = 'GetApiKeyResultApplication';
                    return GetApiKeyResultApplication;
                }());
                Queries.GetApiKeyResultApplication = GetApiKeyResultApplication;
                var ListApiKeys = /** @class */ (function () {
                    function ListApiKeys() {
                    }
                    ListApiKeys.TYPE_NAME = 'ListApiKeys';
                    return ListApiKeys;
                }());
                Queries.ListApiKeys = ListApiKeys;
                var ListApiKeysResult = /** @class */ (function () {
                    function ListApiKeysResult() {
                    }
                    ListApiKeysResult.TYPE_NAME = 'ListApiKeysResult';
                    return ListApiKeysResult;
                }());
                Queries.ListApiKeysResult = ListApiKeysResult;
                var ListApiKeysResultItem = /** @class */ (function () {
                    function ListApiKeysResultItem() {
                    }
                    ListApiKeysResultItem.TYPE_NAME = 'ListApiKeysResultItem';
                    return ListApiKeysResultItem;
                }());
                Queries.ListApiKeysResultItem = ListApiKeysResultItem;
            })(Queries = ApiKeys.Queries || (ApiKeys.Queries = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Events;
            (function (Events) {
                var ApiKeyCreated = /** @class */ (function () {
                    function ApiKeyCreated(applicationNameForTheAppUsingTheKey, apiKey, sharedSecret, applicationIds, createdById) {
                        this.ApplicationNameForTheAppUsingTheKey = applicationNameForTheAppUsingTheKey;
                        this.ApiKey = apiKey;
                        this.SharedSecret = sharedSecret;
                        this.ApplicationIds = applicationIds;
                        this.CreatedById = createdById;
                    }
                    ApiKeyCreated.TYPE_NAME = 'ApiKeyCreated';
                    return ApiKeyCreated;
                }());
                Events.ApiKeyCreated = ApiKeyCreated;
                var ApiKeyRemoved = /** @class */ (function () {
                    function ApiKeyRemoved() {
                    }
                    ApiKeyRemoved.TYPE_NAME = 'ApiKeyRemoved';
                    return ApiKeyRemoved;
                }());
                Events.ApiKeyRemoved = ApiKeyRemoved;
            })(Events = ApiKeys.Events || (ApiKeys.Events = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Commands;
            (function (Commands) {
                var CreateApiKey = /** @class */ (function () {
                    function CreateApiKey(applicationName, apiKey, sharedSecret, applicationIds) {
                        this.ApplicationName = applicationName;
                        this.ApiKey = apiKey;
                        this.SharedSecret = sharedSecret;
                        this.ApplicationIds = applicationIds;
                    }
                    CreateApiKey.TYPE_NAME = 'CreateApiKey';
                    return CreateApiKey;
                }());
                Commands.CreateApiKey = CreateApiKey;
                var DeleteApiKey = /** @class */ (function () {
                    function DeleteApiKey(id) {
                        this.Id = id;
                    }
                    DeleteApiKey.TYPE_NAME = 'DeleteApiKey';
                    return DeleteApiKey;
                }());
                Commands.DeleteApiKey = DeleteApiKey;
                var EditApiKey = /** @class */ (function () {
                    function EditApiKey(id) {
                        this.Id = id;
                    }
                    EditApiKey.TYPE_NAME = 'EditApiKey';
                    return EditApiKey;
                }());
                Commands.EditApiKey = EditApiKey;
            })(Commands = ApiKeys.Commands || (ApiKeys.Commands = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var RegisterSimple = /** @class */ (function () {
                function RegisterSimple(emailAddress) {
                    this.EmailAddress = emailAddress;
                }
                RegisterSimple.TYPE_NAME = 'RegisterSimple';
                return RegisterSimple;
            }());
            Accounts.RegisterSimple = RegisterSimple;
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Requests;
            (function (Requests) {
                var AcceptInvitation = /** @class */ (function () {
                    function AcceptInvitation(userName, password, invitationKey) {
                        this.UserName = userName;
                        this.Password = password;
                        this.InvitationKey = invitationKey;
                    }
                    AcceptInvitation.TYPE_NAME = 'AcceptInvitation';
                    return AcceptInvitation;
                }());
                Requests.AcceptInvitation = AcceptInvitation;
                var ChangePassword = /** @class */ (function () {
                    function ChangePassword(currentPassword, newPassword) {
                        this.CurrentPassword = currentPassword;
                        this.NewPassword = newPassword;
                    }
                    ChangePassword.TYPE_NAME = 'ChangePassword';
                    return ChangePassword;
                }());
                Requests.ChangePassword = ChangePassword;
                var ValidateNewLoginReply = /** @class */ (function () {
                    function ValidateNewLoginReply() {
                    }
                    ValidateNewLoginReply.TYPE_NAME = 'ValidateNewLoginReply';
                    return ValidateNewLoginReply;
                }());
                Requests.ValidateNewLoginReply = ValidateNewLoginReply;
            })(Requests = Accounts.Requests || (Accounts.Requests = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Queries;
            (function (Queries) {
                var AccountDTO = /** @class */ (function () {
                    function AccountDTO() {
                    }
                    AccountDTO.TYPE_NAME = 'AccountDTO';
                    return AccountDTO;
                }());
                Queries.AccountDTO = AccountDTO;
                var AccountState;
                (function (AccountState) {
                    AccountState[AccountState["VerificationRequired"] = 0] = "VerificationRequired";
                    AccountState[AccountState["Active"] = 1] = "Active";
                    AccountState[AccountState["Locked"] = 2] = "Locked";
                    AccountState[AccountState["ResetPassword"] = 3] = "ResetPassword";
                })(AccountState = Queries.AccountState || (Queries.AccountState = {}));
                var FindAccountByUserName = /** @class */ (function () {
                    function FindAccountByUserName(userName) {
                        this.UserName = userName;
                    }
                    FindAccountByUserName.TYPE_NAME = 'FindAccountByUserName';
                    return FindAccountByUserName;
                }());
                Queries.FindAccountByUserName = FindAccountByUserName;
                var FindAccountByUserNameResult = /** @class */ (function () {
                    function FindAccountByUserNameResult(accountId, displayName) {
                        this.AccountId = accountId;
                        this.DisplayName = displayName;
                    }
                    FindAccountByUserNameResult.TYPE_NAME = 'FindAccountByUserNameResult';
                    return FindAccountByUserNameResult;
                }());
                Queries.FindAccountByUserNameResult = FindAccountByUserNameResult;
                var GetAccountById = /** @class */ (function () {
                    function GetAccountById(accountId) {
                        this.AccountId = accountId;
                    }
                    GetAccountById.TYPE_NAME = 'GetAccountById';
                    return GetAccountById;
                }());
                Queries.GetAccountById = GetAccountById;
                var GetAccountEmailById = /** @class */ (function () {
                    function GetAccountEmailById(accountId) {
                        this.AccountId = accountId;
                    }
                    GetAccountEmailById.TYPE_NAME = 'GetAccountEmailById';
                    return GetAccountEmailById;
                }());
                Queries.GetAccountEmailById = GetAccountEmailById;
            })(Queries = Accounts.Queries || (Accounts.Queries = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Events;
            (function (Events) {
                var AccountActivated = /** @class */ (function () {
                    function AccountActivated(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    AccountActivated.TYPE_NAME = 'AccountActivated';
                    return AccountActivated;
                }());
                Events.AccountActivated = AccountActivated;
                var AccountRegistered = /** @class */ (function () {
                    function AccountRegistered(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    AccountRegistered.TYPE_NAME = 'AccountRegistered';
                    return AccountRegistered;
                }());
                Events.AccountRegistered = AccountRegistered;
                var InvitationAccepted = /** @class */ (function () {
                    function InvitationAccepted(accountId, invitedByUserName, userName) {
                        this.AccountId = accountId;
                        this.InvitedByUserName = invitedByUserName;
                        this.UserName = userName;
                    }
                    InvitationAccepted.TYPE_NAME = 'InvitationAccepted';
                    return InvitationAccepted;
                }());
                Events.InvitationAccepted = InvitationAccepted;
                var LoginFailed = /** @class */ (function () {
                    function LoginFailed(userName) {
                        this.UserName = userName;
                    }
                    LoginFailed.TYPE_NAME = 'LoginFailed';
                    return LoginFailed;
                }());
                Events.LoginFailed = LoginFailed;
            })(Events = Accounts.Events || (Accounts.Events = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
(function (codeRR) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Commands;
            (function (Commands) {
                var DeclineInvitation = /** @class */ (function () {
                    function DeclineInvitation(invitationId) {
                        this.InvitationId = invitationId;
                    }
                    DeclineInvitation.TYPE_NAME = 'DeclineInvitation';
                    return DeclineInvitation;
                }());
                Commands.DeclineInvitation = DeclineInvitation;
                var RegisterAccount = /** @class */ (function () {
                    function RegisterAccount(userName, password, email) {
                        this.UserName = userName;
                        this.Password = password;
                        this.Email = email;
                    }
                    RegisterAccount.TYPE_NAME = 'RegisterAccount';
                    return RegisterAccount;
                }());
                Commands.RegisterAccount = RegisterAccount;
                var RequestPasswordReset = /** @class */ (function () {
                    function RequestPasswordReset(emailAddress) {
                        this.EmailAddress = emailAddress;
                    }
                    RequestPasswordReset.TYPE_NAME = 'RequestPasswordReset';
                    return RequestPasswordReset;
                }());
                Commands.RequestPasswordReset = RequestPasswordReset;
            })(Commands = Accounts.Commands || (Accounts.Commands = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = codeRR.Core || (codeRR.Core = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=AllModels.js.map