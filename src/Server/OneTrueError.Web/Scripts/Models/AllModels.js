var OneTrueError;
(function (OneTrueError) {
    var Web;
    (function (Web) {
        var Overview;
        (function (Overview) {
            var Queries;
            (function (Queries) {
                var GetOverview = (function () {
                    function GetOverview() {
                    }
                    GetOverview.TYPE_NAME = 'GetOverview';
                    return GetOverview;
                }());
                Queries.GetOverview = GetOverview;
                var GetOverviewApplicationResult = (function () {
                    function GetOverviewApplicationResult(label, startDate, days) {
                        this.Label = label;
                    }
                    GetOverviewApplicationResult.TYPE_NAME = 'GetOverviewApplicationResult';
                    return GetOverviewApplicationResult;
                }());
                Queries.GetOverviewApplicationResult = GetOverviewApplicationResult;
                var GetOverviewResult = (function () {
                    function GetOverviewResult() {
                    }
                    GetOverviewResult.TYPE_NAME = 'GetOverviewResult';
                    return GetOverviewResult;
                }());
                Queries.GetOverviewResult = GetOverviewResult;
                var OverviewStatSummary = (function () {
                    function OverviewStatSummary() {
                    }
                    OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                    return OverviewStatSummary;
                }());
                Queries.OverviewStatSummary = OverviewStatSummary;
            })(Queries = Overview.Queries || (Overview.Queries = {}));
        })(Overview = Web.Overview || (Web.Overview = {}));
    })(Web = OneTrueError.Web || (OneTrueError.Web = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Web;
    (function (Web) {
        var Feedback;
        (function (Feedback) {
            var Queries;
            (function (Queries) {
                var GetFeedbackForApplicationPage = (function () {
                    function GetFeedbackForApplicationPage(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetFeedbackForApplicationPage.TYPE_NAME = 'GetFeedbackForApplicationPage';
                    return GetFeedbackForApplicationPage;
                }());
                Queries.GetFeedbackForApplicationPage = GetFeedbackForApplicationPage;
                var GetFeedbackForApplicationPageResult = (function () {
                    function GetFeedbackForApplicationPageResult() {
                    }
                    GetFeedbackForApplicationPageResult.TYPE_NAME = 'GetFeedbackForApplicationPageResult';
                    return GetFeedbackForApplicationPageResult;
                }());
                Queries.GetFeedbackForApplicationPageResult = GetFeedbackForApplicationPageResult;
                var GetFeedbackForApplicationPageResultItem = (function () {
                    function GetFeedbackForApplicationPageResultItem() {
                    }
                    GetFeedbackForApplicationPageResultItem.TYPE_NAME = 'GetFeedbackForApplicationPageResultItem';
                    return GetFeedbackForApplicationPageResultItem;
                }());
                Queries.GetFeedbackForApplicationPageResultItem = GetFeedbackForApplicationPageResultItem;
                var GetIncidentFeedback = (function () {
                    function GetIncidentFeedback(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncidentFeedback.TYPE_NAME = 'GetIncidentFeedback';
                    return GetIncidentFeedback;
                }());
                Queries.GetIncidentFeedback = GetIncidentFeedback;
                var GetIncidentFeedbackResult = (function () {
                    function GetIncidentFeedbackResult(items, emails) {
                        this.Items = items;
                        this.Emails = emails;
                    }
                    GetIncidentFeedbackResult.TYPE_NAME = 'GetIncidentFeedbackResult';
                    return GetIncidentFeedbackResult;
                }());
                Queries.GetIncidentFeedbackResult = GetIncidentFeedbackResult;
                var GetIncidentFeedbackResultItem = (function () {
                    function GetIncidentFeedbackResultItem() {
                    }
                    GetIncidentFeedbackResultItem.TYPE_NAME = 'GetIncidentFeedbackResultItem';
                    return GetIncidentFeedbackResultItem;
                }());
                Queries.GetIncidentFeedbackResultItem = GetIncidentFeedbackResultItem;
                var GetFeedbackForDashboardPage = (function () {
                    function GetFeedbackForDashboardPage() {
                    }
                    GetFeedbackForDashboardPage.TYPE_NAME = 'GetFeedbackForDashboardPage';
                    return GetFeedbackForDashboardPage;
                }());
                Queries.GetFeedbackForDashboardPage = GetFeedbackForDashboardPage;
                var GetFeedbackForDashboardPageResult = (function () {
                    function GetFeedbackForDashboardPageResult() {
                    }
                    GetFeedbackForDashboardPageResult.TYPE_NAME = 'GetFeedbackForDashboardPageResult';
                    return GetFeedbackForDashboardPageResult;
                }());
                Queries.GetFeedbackForDashboardPageResult = GetFeedbackForDashboardPageResult;
                var GetFeedbackForDashboardPageResultItem = (function () {
                    function GetFeedbackForDashboardPageResultItem() {
                    }
                    GetFeedbackForDashboardPageResultItem.TYPE_NAME = 'GetFeedbackForDashboardPageResultItem';
                    return GetFeedbackForDashboardPageResultItem;
                }());
                Queries.GetFeedbackForDashboardPageResultItem = GetFeedbackForDashboardPageResultItem;
            })(Queries = Feedback.Queries || (Feedback.Queries = {}));
        })(Feedback = Web.Feedback || (Web.Feedback = {}));
    })(Web = OneTrueError.Web || (OneTrueError.Web = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            (function (LastTriggerActionDTO) {
                LastTriggerActionDTO[LastTriggerActionDTO["ExecuteActions"] = 0] = "ExecuteActions";
                LastTriggerActionDTO[LastTriggerActionDTO["AbortTrigger"] = 1] = "AbortTrigger";
            })(Triggers.LastTriggerActionDTO || (Triggers.LastTriggerActionDTO = {}));
            var LastTriggerActionDTO = Triggers.LastTriggerActionDTO;
            var TriggerActionDataDTO = (function () {
                function TriggerActionDataDTO() {
                }
                TriggerActionDataDTO.TYPE_NAME = 'TriggerActionDataDTO';
                return TriggerActionDataDTO;
            }());
            Triggers.TriggerActionDataDTO = TriggerActionDataDTO;
            var TriggerContextRule = (function () {
                function TriggerContextRule() {
                }
                TriggerContextRule.TYPE_NAME = 'TriggerContextRule';
                return TriggerContextRule;
            }());
            Triggers.TriggerContextRule = TriggerContextRule;
            var TriggerExceptionRule = (function () {
                function TriggerExceptionRule() {
                }
                TriggerExceptionRule.TYPE_NAME = 'TriggerExceptionRule';
                return TriggerExceptionRule;
            }());
            Triggers.TriggerExceptionRule = TriggerExceptionRule;
            (function (TriggerFilterCondition) {
                TriggerFilterCondition[TriggerFilterCondition["StartsWith"] = 0] = "StartsWith";
                TriggerFilterCondition[TriggerFilterCondition["EndsWith"] = 1] = "EndsWith";
                TriggerFilterCondition[TriggerFilterCondition["Contains"] = 2] = "Contains";
                TriggerFilterCondition[TriggerFilterCondition["DoNotContain"] = 3] = "DoNotContain";
                TriggerFilterCondition[TriggerFilterCondition["Equals"] = 4] = "Equals";
            })(Triggers.TriggerFilterCondition || (Triggers.TriggerFilterCondition = {}));
            var TriggerFilterCondition = Triggers.TriggerFilterCondition;
            var TriggerDTO = (function () {
                function TriggerDTO() {
                }
                TriggerDTO.TYPE_NAME = 'TriggerDTO';
                return TriggerDTO;
            }());
            Triggers.TriggerDTO = TriggerDTO;
            (function (TriggerRuleAction) {
                TriggerRuleAction[TriggerRuleAction["AbortTrigger"] = 0] = "AbortTrigger";
                TriggerRuleAction[TriggerRuleAction["ContinueWithNextRule"] = 1] = "ContinueWithNextRule";
                TriggerRuleAction[TriggerRuleAction["ExecuteActions"] = 2] = "ExecuteActions";
            })(Triggers.TriggerRuleAction || (Triggers.TriggerRuleAction = {}));
            var TriggerRuleAction = Triggers.TriggerRuleAction;
            var TriggerRuleBase = (function () {
                function TriggerRuleBase() {
                }
                TriggerRuleBase.TYPE_NAME = 'TriggerRuleBase';
                return TriggerRuleBase;
            }());
            Triggers.TriggerRuleBase = TriggerRuleBase;
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var Queries;
            (function (Queries) {
                var GetContextCollectionMetadata = (function () {
                    function GetContextCollectionMetadata(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetContextCollectionMetadata.TYPE_NAME = 'GetContextCollectionMetadata';
                    return GetContextCollectionMetadata;
                }());
                Queries.GetContextCollectionMetadata = GetContextCollectionMetadata;
                var GetContextCollectionMetadataItem = (function () {
                    function GetContextCollectionMetadataItem() {
                    }
                    GetContextCollectionMetadataItem.TYPE_NAME = 'GetContextCollectionMetadataItem';
                    return GetContextCollectionMetadataItem;
                }());
                Queries.GetContextCollectionMetadataItem = GetContextCollectionMetadataItem;
                var GetTrigger = (function () {
                    function GetTrigger(id) {
                        this.Id = id;
                    }
                    GetTrigger.TYPE_NAME = 'GetTrigger';
                    return GetTrigger;
                }());
                Queries.GetTrigger = GetTrigger;
                var GetTriggerDTO = (function () {
                    function GetTriggerDTO() {
                    }
                    GetTriggerDTO.TYPE_NAME = 'GetTriggerDTO';
                    return GetTriggerDTO;
                }());
                Queries.GetTriggerDTO = GetTriggerDTO;
                var GetTriggersForApplication = (function () {
                    function GetTriggersForApplication(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetTriggersForApplication.TYPE_NAME = 'GetTriggersForApplication';
                    return GetTriggersForApplication;
                }());
                Queries.GetTriggersForApplication = GetTriggersForApplication;
            })(Queries = Triggers.Queries || (Triggers.Queries = {}));
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var Commands;
            (function (Commands) {
                var CreateTrigger = (function () {
                    function CreateTrigger(applicationId, name) {
                        this.ApplicationId = applicationId;
                        this.Name = name;
                    }
                    CreateTrigger.TYPE_NAME = 'CreateTrigger';
                    return CreateTrigger;
                }());
                Commands.CreateTrigger = CreateTrigger;
                var DeleteTrigger = (function () {
                    function DeleteTrigger(id) {
                        this.Id = id;
                    }
                    DeleteTrigger.TYPE_NAME = 'DeleteTrigger';
                    return DeleteTrigger;
                }());
                Commands.DeleteTrigger = DeleteTrigger;
                var UpdateTrigger = (function () {
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
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var TagDTO = (function () {
                function TagDTO() {
                }
                TagDTO.TYPE_NAME = 'TagDTO';
                return TagDTO;
            }());
            Tagging.TagDTO = TagDTO;
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var Queries;
            (function (Queries) {
                var GetTagsForIncident = (function () {
                    function GetTagsForIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetTagsForIncident.TYPE_NAME = 'GetTagsForIncident';
                    return GetTagsForIncident;
                }());
                Queries.GetTagsForIncident = GetTagsForIncident;
            })(Queries = Tagging.Queries || (Tagging.Queries = {}));
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var Events;
            (function (Events) {
                var TagAttachedToIncident = (function () {
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
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var ContextData;
        (function (ContextData) {
            var Queries;
            (function (Queries) {
                var GetSimilarities = (function () {
                    function GetSimilarities(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetSimilarities.TYPE_NAME = 'GetSimilarities';
                    return GetSimilarities;
                }());
                Queries.GetSimilarities = GetSimilarities;
                var GetSimilaritiesCollection = (function () {
                    function GetSimilaritiesCollection() {
                    }
                    GetSimilaritiesCollection.TYPE_NAME = 'GetSimilaritiesCollection';
                    return GetSimilaritiesCollection;
                }());
                Queries.GetSimilaritiesCollection = GetSimilaritiesCollection;
                var GetSimilaritiesResult = (function () {
                    function GetSimilaritiesResult() {
                    }
                    GetSimilaritiesResult.TYPE_NAME = 'GetSimilaritiesResult';
                    return GetSimilaritiesResult;
                }());
                Queries.GetSimilaritiesResult = GetSimilaritiesResult;
                var GetSimilaritiesSimilarity = (function () {
                    function GetSimilaritiesSimilarity(name) {
                        this.Name = name;
                    }
                    GetSimilaritiesSimilarity.TYPE_NAME = 'GetSimilaritiesSimilarity';
                    return GetSimilaritiesSimilarity;
                }());
                Queries.GetSimilaritiesSimilarity = GetSimilaritiesSimilarity;
                var GetSimilaritiesValue = (function () {
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
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var ErrorOrigins;
        (function (ErrorOrigins) {
            var Queries;
            (function (Queries) {
                var GetOriginsForIncident = (function () {
                    function GetOriginsForIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetOriginsForIncident.TYPE_NAME = 'GetOriginsForIncident';
                    return GetOriginsForIncident;
                }());
                Queries.GetOriginsForIncident = GetOriginsForIncident;
                var GetOriginsForIncidentResult = (function () {
                    function GetOriginsForIncidentResult() {
                    }
                    GetOriginsForIncidentResult.TYPE_NAME = 'GetOriginsForIncidentResult';
                    return GetOriginsForIncidentResult;
                }());
                Queries.GetOriginsForIncidentResult = GetOriginsForIncidentResult;
                var GetOriginsForIncidentResultItem = (function () {
                    function GetOriginsForIncidentResultItem() {
                    }
                    GetOriginsForIncidentResultItem.TYPE_NAME = 'GetOriginsForIncidentResultItem';
                    return GetOriginsForIncidentResultItem;
                }());
                Queries.GetOriginsForIncidentResultItem = GetOriginsForIncidentResultItem;
            })(Queries = ErrorOrigins.Queries || (ErrorOrigins.Queries = {}));
        })(ErrorOrigins = Modules.ErrorOrigins || (Modules.ErrorOrigins = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var IgnoreFieldAttribute = (function () {
            function IgnoreFieldAttribute() {
            }
            IgnoreFieldAttribute.TYPE_NAME = 'IgnoreFieldAttribute';
            return IgnoreFieldAttribute;
        }());
        Core.IgnoreFieldAttribute = IgnoreFieldAttribute;
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var NotificationSettings = (function () {
                function NotificationSettings() {
                }
                NotificationSettings.TYPE_NAME = 'NotificationSettings';
                return NotificationSettings;
            }());
            Users.NotificationSettings = NotificationSettings;
            (function (NotificationState) {
                NotificationState[NotificationState["UseGlobalSetting"] = 0] = "UseGlobalSetting";
                NotificationState[NotificationState["Disabled"] = 1] = "Disabled";
                NotificationState[NotificationState["Cellphone"] = 2] = "Cellphone";
                NotificationState[NotificationState["Email"] = 3] = "Email";
            })(Users.NotificationState || (Users.NotificationState = {}));
            var NotificationState = Users.NotificationState;
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var Queries;
            (function (Queries) {
                var GetUserSettings = (function () {
                    function GetUserSettings() {
                    }
                    GetUserSettings.TYPE_NAME = 'GetUserSettings';
                    return GetUserSettings;
                }());
                Queries.GetUserSettings = GetUserSettings;
                var GetUserSettingsResult = (function () {
                    function GetUserSettingsResult() {
                    }
                    GetUserSettingsResult.TYPE_NAME = 'GetUserSettingsResult';
                    return GetUserSettingsResult;
                }());
                Queries.GetUserSettingsResult = GetUserSettingsResult;
            })(Queries = Users.Queries || (Users.Queries = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var Commands;
            (function (Commands) {
                var UpdateNotifications = (function () {
                    function UpdateNotifications() {
                    }
                    UpdateNotifications.TYPE_NAME = 'UpdateNotifications';
                    return UpdateNotifications;
                }());
                Commands.UpdateNotifications = UpdateNotifications;
                var UpdatePersonalSettings = (function () {
                    function UpdatePersonalSettings() {
                    }
                    UpdatePersonalSettings.TYPE_NAME = 'UpdatePersonalSettings';
                    return UpdatePersonalSettings;
                }());
                Commands.UpdatePersonalSettings = UpdatePersonalSettings;
            })(Commands = Users.Commands || (Users.Commands = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Support;
        (function (Support) {
            var SendSupportRequest = (function () {
                function SendSupportRequest() {
                }
                SendSupportRequest.TYPE_NAME = 'SendSupportRequest';
                return SendSupportRequest;
            }());
            Support.SendSupportRequest = SendSupportRequest;
        })(Support = Core.Support || (Core.Support = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Reports;
        (function (Reports) {
            var ContextCollectionDTO = (function () {
                function ContextCollectionDTO(name, items) {
                    this.Name = name;
                }
                ContextCollectionDTO.TYPE_NAME = 'ContextCollectionDTO';
                return ContextCollectionDTO;
            }());
            Reports.ContextCollectionDTO = ContextCollectionDTO;
            var ReportDTO = (function () {
                function ReportDTO() {
                }
                ReportDTO.TYPE_NAME = 'ReportDTO';
                return ReportDTO;
            }());
            Reports.ReportDTO = ReportDTO;
            var ReportExeptionDTO = (function () {
                function ReportExeptionDTO() {
                }
                ReportExeptionDTO.TYPE_NAME = 'ReportExeptionDTO';
                return ReportExeptionDTO;
            }());
            Reports.ReportExeptionDTO = ReportExeptionDTO;
        })(Reports = Core.Reports || (Core.Reports = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Reports;
        (function (Reports) {
            var Queries;
            (function (Queries) {
                var GetReport = (function () {
                    function GetReport(reportId) {
                        this.ReportId = reportId;
                    }
                    GetReport.TYPE_NAME = 'GetReport';
                    return GetReport;
                }());
                Queries.GetReport = GetReport;
                var GetReportException = (function () {
                    function GetReportException() {
                    }
                    GetReportException.TYPE_NAME = 'GetReportException';
                    return GetReportException;
                }());
                Queries.GetReportException = GetReportException;
                var GetReportList = (function () {
                    function GetReportList(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetReportList.TYPE_NAME = 'GetReportList';
                    return GetReportList;
                }());
                Queries.GetReportList = GetReportList;
                var GetReportListResult = (function () {
                    function GetReportListResult(items) {
                        this.Items = items;
                    }
                    GetReportListResult.TYPE_NAME = 'GetReportListResult';
                    return GetReportListResult;
                }());
                Queries.GetReportListResult = GetReportListResult;
                var GetReportListResultItem = (function () {
                    function GetReportListResultItem() {
                    }
                    GetReportListResultItem.TYPE_NAME = 'GetReportListResultItem';
                    return GetReportListResultItem;
                }());
                Queries.GetReportListResultItem = GetReportListResultItem;
                var GetReportResult = (function () {
                    function GetReportResult() {
                    }
                    GetReportResult.TYPE_NAME = 'GetReportResult';
                    return GetReportResult;
                }());
                Queries.GetReportResult = GetReportResult;
                var GetReportResultContextCollection = (function () {
                    function GetReportResultContextCollection(name, properties) {
                        this.Name = name;
                        this.Properties = properties;
                    }
                    GetReportResultContextCollection.TYPE_NAME = 'GetReportResultContextCollection';
                    return GetReportResultContextCollection;
                }());
                Queries.GetReportResultContextCollection = GetReportResultContextCollection;
                var KeyValuePair = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Messaging;
        (function (Messaging) {
            var EmailAddress = (function () {
                function EmailAddress(address) {
                    this.Address = address;
                }
                EmailAddress.TYPE_NAME = 'EmailAddress';
                return EmailAddress;
            }());
            Messaging.EmailAddress = EmailAddress;
            var EmailMessage = (function () {
                function EmailMessage() {
                }
                EmailMessage.TYPE_NAME = 'EmailMessage';
                return EmailMessage;
            }());
            Messaging.EmailMessage = EmailMessage;
            var EmailResource = (function () {
                function EmailResource(name, content) {
                    this.Name = name;
                    this.Content = content;
                }
                EmailResource.TYPE_NAME = 'EmailResource';
                return EmailResource;
            }());
            Messaging.EmailResource = EmailResource;
        })(Messaging = Core.Messaging || (Core.Messaging = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Messaging;
        (function (Messaging) {
            var Commands;
            (function (Commands) {
                var SendSms = (function () {
                    function SendSms(phoneNumber, message) {
                        this.PhoneNumber = phoneNumber;
                        this.Message = message;
                    }
                    SendSms.TYPE_NAME = 'SendSms';
                    return SendSms;
                }());
                Commands.SendSms = SendSms;
                var SendEmail = (function () {
                    function SendEmail() {
                    }
                    SendEmail.TYPE_NAME = 'SendEmail';
                    return SendEmail;
                }());
                Commands.SendEmail = SendEmail;
                var SendTemplateEmail = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Invitations;
        (function (Invitations) {
            var Queries;
            (function (Queries) {
                var GetInvitationByKey = (function () {
                    function GetInvitationByKey(invitationKey) {
                        this.InvitationKey = invitationKey;
                    }
                    GetInvitationByKey.TYPE_NAME = 'GetInvitationByKey';
                    return GetInvitationByKey;
                }());
                Queries.GetInvitationByKey = GetInvitationByKey;
                var GetInvitationByKeyResult = (function () {
                    function GetInvitationByKeyResult() {
                    }
                    GetInvitationByKeyResult.TYPE_NAME = 'GetInvitationByKeyResult';
                    return GetInvitationByKeyResult;
                }());
                Queries.GetInvitationByKeyResult = GetInvitationByKeyResult;
            })(Queries = Invitations.Queries || (Invitations.Queries = {}));
        })(Invitations = Core.Invitations || (Core.Invitations = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Invitations;
        (function (Invitations) {
            var Commands;
            (function (Commands) {
                var InviteUser = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            (function (IncidentOrder) {
                IncidentOrder[IncidentOrder["Newest"] = 0] = "Newest";
                IncidentOrder[IncidentOrder["MostReports"] = 1] = "MostReports";
                IncidentOrder[IncidentOrder["MostFeedback"] = 2] = "MostFeedback";
            })(Incidents.IncidentOrder || (Incidents.IncidentOrder = {}));
            var IncidentOrder = Incidents.IncidentOrder;
            var IncidentSummaryDTO = (function () {
                function IncidentSummaryDTO(id, name) {
                    this.Id = id;
                    this.Name = name;
                }
                IncidentSummaryDTO.TYPE_NAME = 'IncidentSummaryDTO';
                return IncidentSummaryDTO;
            }());
            Incidents.IncidentSummaryDTO = IncidentSummaryDTO;
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Queries;
            (function (Queries) {
                var FindIncidentResult = (function () {
                    function FindIncidentResult() {
                    }
                    FindIncidentResult.TYPE_NAME = 'FindIncidentResult';
                    return FindIncidentResult;
                }());
                Queries.FindIncidentResult = FindIncidentResult;
                var FindIncidentResultItem = (function () {
                    function FindIncidentResultItem(id, name) {
                        this.Id = id;
                        this.Name = name;
                    }
                    FindIncidentResultItem.TYPE_NAME = 'FindIncidentResultItem';
                    return FindIncidentResultItem;
                }());
                Queries.FindIncidentResultItem = FindIncidentResultItem;
                var FindIncidents = (function () {
                    function FindIncidents() {
                    }
                    FindIncidents.TYPE_NAME = 'FindIncidents';
                    return FindIncidents;
                }());
                Queries.FindIncidents = FindIncidents;
                var GetIncident = (function () {
                    function GetIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncident.TYPE_NAME = 'GetIncident';
                    return GetIncident;
                }());
                Queries.GetIncident = GetIncident;
                var GetIncidentForClosePage = (function () {
                    function GetIncidentForClosePage(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    GetIncidentForClosePage.TYPE_NAME = 'GetIncidentForClosePage';
                    return GetIncidentForClosePage;
                }());
                Queries.GetIncidentForClosePage = GetIncidentForClosePage;
                var GetIncidentForClosePageResult = (function () {
                    function GetIncidentForClosePageResult() {
                    }
                    GetIncidentForClosePageResult.TYPE_NAME = 'GetIncidentForClosePageResult';
                    return GetIncidentForClosePageResult;
                }());
                Queries.GetIncidentForClosePageResult = GetIncidentForClosePageResult;
                var GetIncidentResult = (function () {
                    function GetIncidentResult() {
                    }
                    GetIncidentResult.TYPE_NAME = 'GetIncidentResult';
                    return GetIncidentResult;
                }());
                Queries.GetIncidentResult = GetIncidentResult;
                var GetIncidentStatistics = (function () {
                    function GetIncidentStatistics() {
                    }
                    GetIncidentStatistics.TYPE_NAME = 'GetIncidentStatistics';
                    return GetIncidentStatistics;
                }());
                Queries.GetIncidentStatistics = GetIncidentStatistics;
                var GetIncidentStatisticsResult = (function () {
                    function GetIncidentStatisticsResult() {
                    }
                    GetIncidentStatisticsResult.TYPE_NAME = 'GetIncidentStatisticsResult';
                    return GetIncidentStatisticsResult;
                }());
                Queries.GetIncidentStatisticsResult = GetIncidentStatisticsResult;
                var ReportDay = (function () {
                    function ReportDay() {
                    }
                    ReportDay.TYPE_NAME = 'ReportDay';
                    return ReportDay;
                }());
                Queries.ReportDay = ReportDay;
            })(Queries = Incidents.Queries || (Incidents.Queries = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Events;
            (function (Events) {
                var IncidentIgnored = (function () {
                    function IncidentIgnored(incidentId, accountId, userName) {
                        this.IncidentId = incidentId;
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    IncidentIgnored.TYPE_NAME = 'IncidentIgnored';
                    return IncidentIgnored;
                }());
                Events.IncidentIgnored = IncidentIgnored;
                var IncidentReOpened = (function () {
                    function IncidentReOpened(applicationId, incidentId, createdAtUtc) {
                        this.ApplicationId = applicationId;
                        this.IncidentId = incidentId;
                        this.CreatedAtUtc = createdAtUtc;
                    }
                    IncidentReOpened.TYPE_NAME = 'IncidentReOpened';
                    return IncidentReOpened;
                }());
                Events.IncidentReOpened = IncidentReOpened;
                var ReportAddedToIncident = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Incidents;
        (function (Incidents) {
            var Commands;
            (function (Commands) {
                var CloseIncident = (function () {
                    function CloseIncident(solution, incidentId) {
                        this.Solution = solution;
                        this.IncidentId = incidentId;
                    }
                    CloseIncident.TYPE_NAME = 'CloseIncident';
                    return CloseIncident;
                }());
                Commands.CloseIncident = CloseIncident;
                var IgnoreIncident = (function () {
                    function IgnoreIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    IgnoreIncident.TYPE_NAME = 'IgnoreIncident';
                    return IgnoreIncident;
                }());
                Commands.IgnoreIncident = IgnoreIncident;
            })(Commands = Incidents.Commands || (Incidents.Commands = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Feedback;
        (function (Feedback) {
            var Commands;
            (function (Commands) {
                var SubmitFeedback = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Feedback;
        (function (Feedback) {
            var Events;
            (function (Events) {
                var FeedbackAttachedToIncident = (function () {
                    function FeedbackAttachedToIncident() {
                    }
                    FeedbackAttachedToIncident.TYPE_NAME = 'FeedbackAttachedToIncident';
                    return FeedbackAttachedToIncident;
                }());
                Events.FeedbackAttachedToIncident = FeedbackAttachedToIncident;
            })(Events = Feedback.Events || (Feedback.Events = {}));
        })(Feedback = Core.Feedback || (Core.Feedback = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Queries;
            (function (Queries) {
                var GetApiKey = (function () {
                    function GetApiKey(id) {
                        this.Id = id;
                    }
                    GetApiKey.TYPE_NAME = 'GetApiKey';
                    return GetApiKey;
                }());
                Queries.GetApiKey = GetApiKey;
                var GetApiKeyResult = (function () {
                    function GetApiKeyResult() {
                    }
                    GetApiKeyResult.TYPE_NAME = 'GetApiKeyResult';
                    return GetApiKeyResult;
                }());
                Queries.GetApiKeyResult = GetApiKeyResult;
                var GetApiKeyResultApplication = (function () {
                    function GetApiKeyResultApplication() {
                    }
                    GetApiKeyResultApplication.TYPE_NAME = 'GetApiKeyResultApplication';
                    return GetApiKeyResultApplication;
                }());
                Queries.GetApiKeyResultApplication = GetApiKeyResultApplication;
                var ListApiKeys = (function () {
                    function ListApiKeys() {
                    }
                    ListApiKeys.TYPE_NAME = 'ListApiKeys';
                    return ListApiKeys;
                }());
                Queries.ListApiKeys = ListApiKeys;
                var ListApiKeysResult = (function () {
                    function ListApiKeysResult() {
                    }
                    ListApiKeysResult.TYPE_NAME = 'ListApiKeysResult';
                    return ListApiKeysResult;
                }());
                Queries.ListApiKeysResult = ListApiKeysResult;
                var ListApiKeysResultItem = (function () {
                    function ListApiKeysResultItem() {
                    }
                    ListApiKeysResultItem.TYPE_NAME = 'ListApiKeysResultItem';
                    return ListApiKeysResultItem;
                }());
                Queries.ListApiKeysResultItem = ListApiKeysResultItem;
            })(Queries = ApiKeys.Queries || (ApiKeys.Queries = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Events;
            (function (Events) {
                var ApiKeyCreated = (function () {
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
            })(Events = ApiKeys.Events || (ApiKeys.Events = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var ApiKeys;
        (function (ApiKeys) {
            var Commands;
            (function (Commands) {
                var CreateApiKey = (function () {
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
                var DeleteApiKey = (function () {
                    function DeleteApiKey(id) {
                        this.Id = id;
                    }
                    DeleteApiKey.TYPE_NAME = 'DeleteApiKey';
                    return DeleteApiKey;
                }());
                Commands.DeleteApiKey = DeleteApiKey;
            })(Commands = ApiKeys.Commands || (ApiKeys.Commands = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var ApplicationListItem = (function () {
                function ApplicationListItem(id, name) {
                    this.Id = id;
                    this.Name = name;
                }
                ApplicationListItem.TYPE_NAME = 'ApplicationListItem';
                return ApplicationListItem;
            }());
            Applications.ApplicationListItem = ApplicationListItem;
            (function (TypeOfApplication) {
                TypeOfApplication[TypeOfApplication["Mobile"] = 0] = "Mobile";
                TypeOfApplication[TypeOfApplication["DesktopApplication"] = 1] = "DesktopApplication";
                TypeOfApplication[TypeOfApplication["Server"] = 2] = "Server";
            })(Applications.TypeOfApplication || (Applications.TypeOfApplication = {}));
            var TypeOfApplication = Applications.TypeOfApplication;
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Queries;
            (function (Queries) {
                var GetApplicationTeamResult = (function () {
                    function GetApplicationTeamResult() {
                    }
                    GetApplicationTeamResult.TYPE_NAME = 'GetApplicationTeamResult';
                    return GetApplicationTeamResult;
                }());
                Queries.GetApplicationTeamResult = GetApplicationTeamResult;
                var OverviewStatSummary = (function () {
                    function OverviewStatSummary() {
                    }
                    OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                    return OverviewStatSummary;
                }());
                Queries.OverviewStatSummary = OverviewStatSummary;
                var GetApplicationIdByKey = (function () {
                    function GetApplicationIdByKey(applicationKey) {
                        this.ApplicationKey = applicationKey;
                    }
                    GetApplicationIdByKey.TYPE_NAME = 'GetApplicationIdByKey';
                    return GetApplicationIdByKey;
                }());
                Queries.GetApplicationIdByKey = GetApplicationIdByKey;
                var GetApplicationIdByKeyResult = (function () {
                    function GetApplicationIdByKeyResult() {
                    }
                    GetApplicationIdByKeyResult.TYPE_NAME = 'GetApplicationIdByKeyResult';
                    return GetApplicationIdByKeyResult;
                }());
                Queries.GetApplicationIdByKeyResult = GetApplicationIdByKeyResult;
                var GetApplicationInfo = (function () {
                    function GetApplicationInfo() {
                    }
                    GetApplicationInfo.TYPE_NAME = 'GetApplicationInfo';
                    return GetApplicationInfo;
                }());
                Queries.GetApplicationInfo = GetApplicationInfo;
                var GetApplicationInfoResult = (function () {
                    function GetApplicationInfoResult() {
                    }
                    GetApplicationInfoResult.TYPE_NAME = 'GetApplicationInfoResult';
                    return GetApplicationInfoResult;
                }());
                Queries.GetApplicationInfoResult = GetApplicationInfoResult;
                var GetApplicationList = (function () {
                    function GetApplicationList() {
                    }
                    GetApplicationList.TYPE_NAME = 'GetApplicationList';
                    return GetApplicationList;
                }());
                Queries.GetApplicationList = GetApplicationList;
                var GetApplicationOverviewResult = (function () {
                    function GetApplicationOverviewResult() {
                    }
                    GetApplicationOverviewResult.TYPE_NAME = 'GetApplicationOverviewResult';
                    return GetApplicationOverviewResult;
                }());
                Queries.GetApplicationOverviewResult = GetApplicationOverviewResult;
                var GetApplicationTeam = (function () {
                    function GetApplicationTeam(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetApplicationTeam.TYPE_NAME = 'GetApplicationTeam';
                    return GetApplicationTeam;
                }());
                Queries.GetApplicationTeam = GetApplicationTeam;
                var GetApplicationTeamMember = (function () {
                    function GetApplicationTeamMember() {
                    }
                    GetApplicationTeamMember.TYPE_NAME = 'GetApplicationTeamMember';
                    return GetApplicationTeamMember;
                }());
                Queries.GetApplicationTeamMember = GetApplicationTeamMember;
                var GetApplicationTeamResultInvitation = (function () {
                    function GetApplicationTeamResultInvitation() {
                    }
                    GetApplicationTeamResultInvitation.TYPE_NAME = 'GetApplicationTeamResultInvitation';
                    return GetApplicationTeamResultInvitation;
                }());
                Queries.GetApplicationTeamResultInvitation = GetApplicationTeamResultInvitation;
                var GetApplicationOverview = (function () {
                    function GetApplicationOverview(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    GetApplicationOverview.TYPE_NAME = 'GetApplicationOverview';
                    return GetApplicationOverview;
                }());
                Queries.GetApplicationOverview = GetApplicationOverview;
            })(Queries = Applications.Queries || (Applications.Queries = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Events;
            (function (Events) {
                var ApplicationDeleted = (function () {
                    function ApplicationDeleted() {
                    }
                    ApplicationDeleted.TYPE_NAME = 'ApplicationDeleted';
                    return ApplicationDeleted;
                }());
                Events.ApplicationDeleted = ApplicationDeleted;
                var ApplicationCreated = (function () {
                    function ApplicationCreated(id, name, createdById, appKey, sharedSecret) {
                        this.CreatedById = createdById;
                        this.AppKey = appKey;
                        this.SharedSecret = sharedSecret;
                    }
                    ApplicationCreated.TYPE_NAME = 'ApplicationCreated';
                    return ApplicationCreated;
                }());
                Events.ApplicationCreated = ApplicationCreated;
                var UserInvitedToApplication = (function () {
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
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError_1) {
    var Core;
    (function (Core_1) {
        var Applications;
        (function (Applications) {
            var Events;
            (function (Events_1) {
                var OneTrueError;
                (function (OneTrueError) {
                    var Api;
                    (function (Api) {
                        var Core;
                        (function (Core) {
                            var Accounts;
                            (function (Accounts) {
                                var Events;
                                (function (Events) {
                                    var UserAddedToApplication = (function () {
                                        function UserAddedToApplication(applicationId, accountId) {
                                            this.ApplicationId = applicationId;
                                            this.AccountId = accountId;
                                        }
                                        UserAddedToApplication.TYPE_NAME = 'UserAddedToApplication';
                                        return UserAddedToApplication;
                                    }());
                                    Events.UserAddedToApplication = UserAddedToApplication;
                                })(Events = Accounts.Events || (Accounts.Events = {}));
                            })(Accounts = Core.Accounts || (Core.Accounts = {}));
                        })(Core = Api.Core || (Api.Core = {}));
                    })(Api = OneTrueError.Api || (OneTrueError.Api = {}));
                })(OneTrueError = Events_1.OneTrueError || (Events_1.OneTrueError = {}));
            })(Events = Applications.Events || (Applications.Events = {}));
        })(Applications = Core_1.Applications || (Core_1.Applications = {}));
    })(Core = OneTrueError_1.Core || (OneTrueError_1.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Applications;
        (function (Applications) {
            var Commands;
            (function (Commands) {
                var RemoveTeamMember = (function () {
                    function RemoveTeamMember(applicationId, userToRemove) {
                        this.ApplicationId = applicationId;
                        this.UserToRemove = userToRemove;
                    }
                    RemoveTeamMember.TYPE_NAME = 'RemoveTeamMember';
                    return RemoveTeamMember;
                }());
                Commands.RemoveTeamMember = RemoveTeamMember;
                var UpdateApplication = (function () {
                    function UpdateApplication(applicationId, name) {
                        this.ApplicationId = applicationId;
                        this.Name = name;
                    }
                    UpdateApplication.TYPE_NAME = 'UpdateApplication';
                    return UpdateApplication;
                }());
                Commands.UpdateApplication = UpdateApplication;
                var CreateApplication = (function () {
                    function CreateApplication(name, typeOfApplication) {
                        this.Name = name;
                        this.TypeOfApplication = typeOfApplication;
                    }
                    CreateApplication.TYPE_NAME = 'CreateApplication';
                    return CreateApplication;
                }());
                Commands.CreateApplication = CreateApplication;
                var DeleteApplication = (function () {
                    function DeleteApplication(id) {
                        this.Id = id;
                    }
                    DeleteApplication.TYPE_NAME = 'DeleteApplication';
                    return DeleteApplication;
                }());
                Commands.DeleteApplication = DeleteApplication;
            })(Commands = Applications.Commands || (Applications.Commands = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var RegisterSimple = (function () {
                function RegisterSimple(emailAddress) {
                    this.EmailAddress = emailAddress;
                }
                RegisterSimple.TYPE_NAME = 'RegisterSimple';
                return RegisterSimple;
            }());
            Accounts.RegisterSimple = RegisterSimple;
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Requests;
            (function (Requests) {
                var AcceptInvitation = (function () {
                    function AcceptInvitation(userName, password, invitationKey) {
                        this.UserName = userName;
                        this.Password = password;
                        this.InvitationKey = invitationKey;
                    }
                    AcceptInvitation.TYPE_NAME = 'AcceptInvitation';
                    return AcceptInvitation;
                }());
                Requests.AcceptInvitation = AcceptInvitation;
                var AcceptInvitationReply = (function () {
                    function AcceptInvitationReply(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    AcceptInvitationReply.TYPE_NAME = 'AcceptInvitationReply';
                    return AcceptInvitationReply;
                }());
                Requests.AcceptInvitationReply = AcceptInvitationReply;
                var ActivateAccount = (function () {
                    function ActivateAccount(activationKey) {
                        this.ActivationKey = activationKey;
                    }
                    ActivateAccount.TYPE_NAME = 'ActivateAccount';
                    return ActivateAccount;
                }());
                Requests.ActivateAccount = ActivateAccount;
                var ActivateAccountReply = (function () {
                    function ActivateAccountReply(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    ActivateAccountReply.TYPE_NAME = 'ActivateAccountReply';
                    return ActivateAccountReply;
                }());
                Requests.ActivateAccountReply = ActivateAccountReply;
                var ChangePassword = (function () {
                    function ChangePassword(currentPassword, newPassword) {
                        this.CurrentPassword = currentPassword;
                        this.NewPassword = newPassword;
                    }
                    ChangePassword.TYPE_NAME = 'ChangePassword';
                    return ChangePassword;
                }());
                Requests.ChangePassword = ChangePassword;
                var ChangePasswordReply = (function () {
                    function ChangePasswordReply() {
                    }
                    ChangePasswordReply.TYPE_NAME = 'ChangePasswordReply';
                    return ChangePasswordReply;
                }());
                Requests.ChangePasswordReply = ChangePasswordReply;
                var IgnoreFieldAttribute = (function () {
                    function IgnoreFieldAttribute() {
                    }
                    IgnoreFieldAttribute.TYPE_NAME = 'IgnoreFieldAttribute';
                    return IgnoreFieldAttribute;
                }());
                Requests.IgnoreFieldAttribute = IgnoreFieldAttribute;
                var Login = (function () {
                    function Login(userName, password) {
                        this.UserName = userName;
                        this.Password = password;
                    }
                    Login.TYPE_NAME = 'Login';
                    return Login;
                }());
                Requests.Login = Login;
                var LoginReply = (function () {
                    function LoginReply() {
                    }
                    LoginReply.TYPE_NAME = 'LoginReply';
                    return LoginReply;
                }());
                Requests.LoginReply = LoginReply;
                (function (LoginResult) {
                    LoginResult[LoginResult["Locked"] = 0] = "Locked";
                    LoginResult[LoginResult["IncorrectLogin"] = 1] = "IncorrectLogin";
                    LoginResult[LoginResult["Successful"] = 2] = "Successful";
                })(Requests.LoginResult || (Requests.LoginResult = {}));
                var LoginResult = Requests.LoginResult;
                var ResetPassword = (function () {
                    function ResetPassword(activationKey, newPassword) {
                        this.ActivationKey = activationKey;
                        this.NewPassword = newPassword;
                    }
                    ResetPassword.TYPE_NAME = 'ResetPassword';
                    return ResetPassword;
                }());
                Requests.ResetPassword = ResetPassword;
                var ResetPasswordReply = (function () {
                    function ResetPasswordReply() {
                    }
                    ResetPasswordReply.TYPE_NAME = 'ResetPasswordReply';
                    return ResetPasswordReply;
                }());
                Requests.ResetPasswordReply = ResetPasswordReply;
                var ValidateNewLogin = (function () {
                    function ValidateNewLogin() {
                    }
                    ValidateNewLogin.TYPE_NAME = 'ValidateNewLogin';
                    return ValidateNewLogin;
                }());
                Requests.ValidateNewLogin = ValidateNewLogin;
                var ValidateNewLoginReply = (function () {
                    function ValidateNewLoginReply() {
                    }
                    ValidateNewLoginReply.TYPE_NAME = 'ValidateNewLoginReply';
                    return ValidateNewLoginReply;
                }());
                Requests.ValidateNewLoginReply = ValidateNewLoginReply;
            })(Requests = Accounts.Requests || (Accounts.Requests = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Queries;
            (function (Queries) {
                var AccountDTO = (function () {
                    function AccountDTO() {
                    }
                    AccountDTO.TYPE_NAME = 'AccountDTO';
                    return AccountDTO;
                }());
                Queries.AccountDTO = AccountDTO;
                (function (AccountState) {
                    AccountState[AccountState["VerificationRequired"] = 0] = "VerificationRequired";
                    AccountState[AccountState["Active"] = 1] = "Active";
                    AccountState[AccountState["Locked"] = 2] = "Locked";
                    AccountState[AccountState["ResetPassword"] = 3] = "ResetPassword";
                })(Queries.AccountState || (Queries.AccountState = {}));
                var AccountState = Queries.AccountState;
                var FindAccountByUserName = (function () {
                    function FindAccountByUserName(userName) {
                        this.UserName = userName;
                    }
                    FindAccountByUserName.TYPE_NAME = 'FindAccountByUserName';
                    return FindAccountByUserName;
                }());
                Queries.FindAccountByUserName = FindAccountByUserName;
                var GetAccountById = (function () {
                    function GetAccountById(accountId) {
                        this.AccountId = accountId;
                    }
                    GetAccountById.TYPE_NAME = 'GetAccountById';
                    return GetAccountById;
                }());
                Queries.GetAccountById = GetAccountById;
                var GetAccountEmailById = (function () {
                    function GetAccountEmailById(accountId) {
                        this.AccountId = accountId;
                    }
                    GetAccountEmailById.TYPE_NAME = 'GetAccountEmailById';
                    return GetAccountEmailById;
                }());
                Queries.GetAccountEmailById = GetAccountEmailById;
                var FindAccountByUserNameResult = (function () {
                    function FindAccountByUserNameResult(accountId, displayName) {
                        this.AccountId = accountId;
                        this.DisplayName = displayName;
                    }
                    FindAccountByUserNameResult.TYPE_NAME = 'FindAccountByUserNameResult';
                    return FindAccountByUserNameResult;
                }());
                Queries.FindAccountByUserNameResult = FindAccountByUserNameResult;
            })(Queries = Accounts.Queries || (Accounts.Queries = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Events;
            (function (Events) {
                var AccountActivated = (function () {
                    function AccountActivated(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    AccountActivated.TYPE_NAME = 'AccountActivated';
                    return AccountActivated;
                }());
                Events.AccountActivated = AccountActivated;
                var AccountRegistered = (function () {
                    function AccountRegistered(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    AccountRegistered.TYPE_NAME = 'AccountRegistered';
                    return AccountRegistered;
                }());
                Events.AccountRegistered = AccountRegistered;
                var InvitationAccepted = (function () {
                    function InvitationAccepted(accountId, invitedByUserName, userName) {
                        this.AccountId = accountId;
                        this.InvitedByUserName = invitedByUserName;
                        this.UserName = userName;
                    }
                    InvitationAccepted.TYPE_NAME = 'InvitationAccepted';
                    return InvitationAccepted;
                }());
                Events.InvitationAccepted = InvitationAccepted;
                var LoginFailed = (function () {
                    function LoginFailed(userName) {
                        this.UserName = userName;
                    }
                    LoginFailed.TYPE_NAME = 'LoginFailed';
                    return LoginFailed;
                }());
                Events.LoginFailed = LoginFailed;
            })(Events = Accounts.Events || (Accounts.Events = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
var OneTrueError;
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var Commands;
            (function (Commands) {
                var DeclineInvitation = (function () {
                    function DeclineInvitation(invitationId) {
                        this.InvitationId = invitationId;
                    }
                    DeclineInvitation.TYPE_NAME = 'DeclineInvitation';
                    return DeclineInvitation;
                }());
                Commands.DeclineInvitation = DeclineInvitation;
                var RegisterAccount = (function () {
                    function RegisterAccount(userName, password, email) {
                        this.UserName = userName;
                        this.Password = password;
                        this.Email = email;
                    }
                    RegisterAccount.TYPE_NAME = 'RegisterAccount';
                    return RegisterAccount;
                }());
                Commands.RegisterAccount = RegisterAccount;
                var RequestPasswordReset = (function () {
                    function RequestPasswordReset(emailAddress) {
                        this.EmailAddress = emailAddress;
                    }
                    RequestPasswordReset.TYPE_NAME = 'RequestPasswordReset';
                    return RequestPasswordReset;
                }());
                Commands.RequestPasswordReset = RequestPasswordReset;
            })(Commands = Accounts.Commands || (Accounts.Commands = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=AllModels.js.map