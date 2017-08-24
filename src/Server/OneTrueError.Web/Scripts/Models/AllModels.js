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
                    return GetOverview;
                }());
                GetOverview.TYPE_NAME = 'GetOverview';
                Queries.GetOverview = GetOverview;
                var GetOverviewApplicationResult = (function () {
                    function GetOverviewApplicationResult(label, startDate, days) {
                        this.Label = label;
                    }
                    return GetOverviewApplicationResult;
                }());
                GetOverviewApplicationResult.TYPE_NAME = 'GetOverviewApplicationResult';
                Queries.GetOverviewApplicationResult = GetOverviewApplicationResult;
                var GetOverviewResult = (function () {
                    function GetOverviewResult() {
                    }
                    return GetOverviewResult;
                }());
                GetOverviewResult.TYPE_NAME = 'GetOverviewResult';
                Queries.GetOverviewResult = GetOverviewResult;
                var OverviewStatSummary = (function () {
                    function OverviewStatSummary() {
                    }
                    return OverviewStatSummary;
                }());
                OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                Queries.OverviewStatSummary = OverviewStatSummary;
            })(Queries = Overview.Queries || (Overview.Queries = {}));
        })(Overview = Web.Overview || (Web.Overview = {}));
    })(Web = OneTrueError.Web || (OneTrueError.Web = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetFeedbackForApplicationPage;
                }());
                GetFeedbackForApplicationPage.TYPE_NAME = 'GetFeedbackForApplicationPage';
                Queries.GetFeedbackForApplicationPage = GetFeedbackForApplicationPage;
                var GetFeedbackForApplicationPageResult = (function () {
                    function GetFeedbackForApplicationPageResult() {
                    }
                    return GetFeedbackForApplicationPageResult;
                }());
                GetFeedbackForApplicationPageResult.TYPE_NAME = 'GetFeedbackForApplicationPageResult';
                Queries.GetFeedbackForApplicationPageResult = GetFeedbackForApplicationPageResult;
                var GetFeedbackForApplicationPageResultItem = (function () {
                    function GetFeedbackForApplicationPageResultItem() {
                    }
                    return GetFeedbackForApplicationPageResultItem;
                }());
                GetFeedbackForApplicationPageResultItem.TYPE_NAME = 'GetFeedbackForApplicationPageResultItem';
                Queries.GetFeedbackForApplicationPageResultItem = GetFeedbackForApplicationPageResultItem;
                var GetIncidentFeedback = (function () {
                    function GetIncidentFeedback(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    return GetIncidentFeedback;
                }());
                GetIncidentFeedback.TYPE_NAME = 'GetIncidentFeedback';
                Queries.GetIncidentFeedback = GetIncidentFeedback;
                var GetIncidentFeedbackResult = (function () {
                    function GetIncidentFeedbackResult(items, emails) {
                        this.Items = items;
                        this.Emails = emails;
                    }
                    return GetIncidentFeedbackResult;
                }());
                GetIncidentFeedbackResult.TYPE_NAME = 'GetIncidentFeedbackResult';
                Queries.GetIncidentFeedbackResult = GetIncidentFeedbackResult;
                var GetIncidentFeedbackResultItem = (function () {
                    function GetIncidentFeedbackResultItem() {
                    }
                    return GetIncidentFeedbackResultItem;
                }());
                GetIncidentFeedbackResultItem.TYPE_NAME = 'GetIncidentFeedbackResultItem';
                Queries.GetIncidentFeedbackResultItem = GetIncidentFeedbackResultItem;
                var GetFeedbackForDashboardPage = (function () {
                    function GetFeedbackForDashboardPage() {
                    }
                    return GetFeedbackForDashboardPage;
                }());
                GetFeedbackForDashboardPage.TYPE_NAME = 'GetFeedbackForDashboardPage';
                Queries.GetFeedbackForDashboardPage = GetFeedbackForDashboardPage;
                var GetFeedbackForDashboardPageResult = (function () {
                    function GetFeedbackForDashboardPageResult() {
                    }
                    return GetFeedbackForDashboardPageResult;
                }());
                GetFeedbackForDashboardPageResult.TYPE_NAME = 'GetFeedbackForDashboardPageResult';
                Queries.GetFeedbackForDashboardPageResult = GetFeedbackForDashboardPageResult;
                var GetFeedbackForDashboardPageResultItem = (function () {
                    function GetFeedbackForDashboardPageResultItem() {
                    }
                    return GetFeedbackForDashboardPageResultItem;
                }());
                GetFeedbackForDashboardPageResultItem.TYPE_NAME = 'GetFeedbackForDashboardPageResultItem';
                Queries.GetFeedbackForDashboardPageResultItem = GetFeedbackForDashboardPageResultItem;
            })(Queries = Feedback.Queries || (Feedback.Queries = {}));
        })(Feedback = Web.Feedback || (Web.Feedback = {}));
    })(Web = OneTrueError.Web || (OneTrueError.Web = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Triggers;
        (function (Triggers) {
            var LastTriggerActionDTO;
            (function (LastTriggerActionDTO) {
                LastTriggerActionDTO[LastTriggerActionDTO["ExecuteActions"] = 0] = "ExecuteActions";
                LastTriggerActionDTO[LastTriggerActionDTO["AbortTrigger"] = 1] = "AbortTrigger";
            })(LastTriggerActionDTO = Triggers.LastTriggerActionDTO || (Triggers.LastTriggerActionDTO = {}));
            var TriggerActionDataDTO = (function () {
                function TriggerActionDataDTO() {
                }
                return TriggerActionDataDTO;
            }());
            TriggerActionDataDTO.TYPE_NAME = 'TriggerActionDataDTO';
            Triggers.TriggerActionDataDTO = TriggerActionDataDTO;
            var TriggerContextRule = (function () {
                function TriggerContextRule() {
                }
                return TriggerContextRule;
            }());
            TriggerContextRule.TYPE_NAME = 'TriggerContextRule';
            Triggers.TriggerContextRule = TriggerContextRule;
            var TriggerExceptionRule = (function () {
                function TriggerExceptionRule() {
                }
                return TriggerExceptionRule;
            }());
            TriggerExceptionRule.TYPE_NAME = 'TriggerExceptionRule';
            Triggers.TriggerExceptionRule = TriggerExceptionRule;
            var TriggerFilterCondition;
            (function (TriggerFilterCondition) {
                TriggerFilterCondition[TriggerFilterCondition["StartsWith"] = 0] = "StartsWith";
                TriggerFilterCondition[TriggerFilterCondition["EndsWith"] = 1] = "EndsWith";
                TriggerFilterCondition[TriggerFilterCondition["Contains"] = 2] = "Contains";
                TriggerFilterCondition[TriggerFilterCondition["DoNotContain"] = 3] = "DoNotContain";
                TriggerFilterCondition[TriggerFilterCondition["Equals"] = 4] = "Equals";
            })(TriggerFilterCondition = Triggers.TriggerFilterCondition || (Triggers.TriggerFilterCondition = {}));
            var TriggerDTO = (function () {
                function TriggerDTO() {
                }
                return TriggerDTO;
            }());
            TriggerDTO.TYPE_NAME = 'TriggerDTO';
            Triggers.TriggerDTO = TriggerDTO;
            var TriggerRuleAction;
            (function (TriggerRuleAction) {
                TriggerRuleAction[TriggerRuleAction["AbortTrigger"] = 0] = "AbortTrigger";
                TriggerRuleAction[TriggerRuleAction["ContinueWithNextRule"] = 1] = "ContinueWithNextRule";
                TriggerRuleAction[TriggerRuleAction["ExecuteActions"] = 2] = "ExecuteActions";
            })(TriggerRuleAction = Triggers.TriggerRuleAction || (Triggers.TriggerRuleAction = {}));
            var TriggerRuleBase = (function () {
                function TriggerRuleBase() {
                }
                return TriggerRuleBase;
            }());
            TriggerRuleBase.TYPE_NAME = 'TriggerRuleBase';
            Triggers.TriggerRuleBase = TriggerRuleBase;
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetContextCollectionMetadata;
                }());
                GetContextCollectionMetadata.TYPE_NAME = 'GetContextCollectionMetadata';
                Queries.GetContextCollectionMetadata = GetContextCollectionMetadata;
                var GetContextCollectionMetadataItem = (function () {
                    function GetContextCollectionMetadataItem() {
                    }
                    return GetContextCollectionMetadataItem;
                }());
                GetContextCollectionMetadataItem.TYPE_NAME = 'GetContextCollectionMetadataItem';
                Queries.GetContextCollectionMetadataItem = GetContextCollectionMetadataItem;
                var GetTrigger = (function () {
                    function GetTrigger(id) {
                        this.Id = id;
                    }
                    return GetTrigger;
                }());
                GetTrigger.TYPE_NAME = 'GetTrigger';
                Queries.GetTrigger = GetTrigger;
                var GetTriggerDTO = (function () {
                    function GetTriggerDTO() {
                    }
                    return GetTriggerDTO;
                }());
                GetTriggerDTO.TYPE_NAME = 'GetTriggerDTO';
                Queries.GetTriggerDTO = GetTriggerDTO;
                var GetTriggersForApplication = (function () {
                    function GetTriggersForApplication(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    return GetTriggersForApplication;
                }());
                GetTriggersForApplication.TYPE_NAME = 'GetTriggersForApplication';
                Queries.GetTriggersForApplication = GetTriggersForApplication;
            })(Queries = Triggers.Queries || (Triggers.Queries = {}));
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return CreateTrigger;
                }());
                CreateTrigger.TYPE_NAME = 'CreateTrigger';
                Commands.CreateTrigger = CreateTrigger;
                var DeleteTrigger = (function () {
                    function DeleteTrigger(id) {
                        this.Id = id;
                    }
                    return DeleteTrigger;
                }());
                DeleteTrigger.TYPE_NAME = 'DeleteTrigger';
                Commands.DeleteTrigger = DeleteTrigger;
                var UpdateTrigger = (function () {
                    function UpdateTrigger(id, name) {
                        this.Id = id;
                        this.Name = name;
                    }
                    return UpdateTrigger;
                }());
                UpdateTrigger.TYPE_NAME = 'UpdateTrigger';
                Commands.UpdateTrigger = UpdateTrigger;
            })(Commands = Triggers.Commands || (Triggers.Commands = {}));
        })(Triggers = Modules.Triggers || (Modules.Triggers = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Modules;
    (function (Modules) {
        var Tagging;
        (function (Tagging) {
            var TagDTO = (function () {
                function TagDTO() {
                }
                return TagDTO;
            }());
            TagDTO.TYPE_NAME = 'TagDTO';
            Tagging.TagDTO = TagDTO;
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetTagsForIncident;
                }());
                GetTagsForIncident.TYPE_NAME = 'GetTagsForIncident';
                Queries.GetTagsForIncident = GetTagsForIncident;
            })(Queries = Tagging.Queries || (Tagging.Queries = {}));
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return TagAttachedToIncident;
                }());
                TagAttachedToIncident.TYPE_NAME = 'TagAttachedToIncident';
                Events.TagAttachedToIncident = TagAttachedToIncident;
            })(Events = Tagging.Events || (Tagging.Events = {}));
        })(Tagging = Modules.Tagging || (Modules.Tagging = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetSimilarities;
                }());
                GetSimilarities.TYPE_NAME = 'GetSimilarities';
                Queries.GetSimilarities = GetSimilarities;
                var GetSimilaritiesCollection = (function () {
                    function GetSimilaritiesCollection() {
                    }
                    return GetSimilaritiesCollection;
                }());
                GetSimilaritiesCollection.TYPE_NAME = 'GetSimilaritiesCollection';
                Queries.GetSimilaritiesCollection = GetSimilaritiesCollection;
                var GetSimilaritiesResult = (function () {
                    function GetSimilaritiesResult() {
                    }
                    return GetSimilaritiesResult;
                }());
                GetSimilaritiesResult.TYPE_NAME = 'GetSimilaritiesResult';
                Queries.GetSimilaritiesResult = GetSimilaritiesResult;
                var GetSimilaritiesSimilarity = (function () {
                    function GetSimilaritiesSimilarity(name) {
                        this.Name = name;
                    }
                    return GetSimilaritiesSimilarity;
                }());
                GetSimilaritiesSimilarity.TYPE_NAME = 'GetSimilaritiesSimilarity';
                Queries.GetSimilaritiesSimilarity = GetSimilaritiesSimilarity;
                var GetSimilaritiesValue = (function () {
                    function GetSimilaritiesValue(value, percentage, count) {
                        this.Value = value;
                        this.Percentage = percentage;
                        this.Count = count;
                    }
                    return GetSimilaritiesValue;
                }());
                GetSimilaritiesValue.TYPE_NAME = 'GetSimilaritiesValue';
                Queries.GetSimilaritiesValue = GetSimilaritiesValue;
            })(Queries = ContextData.Queries || (ContextData.Queries = {}));
        })(ContextData = Modules.ContextData || (Modules.ContextData = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetOriginsForIncident;
                }());
                GetOriginsForIncident.TYPE_NAME = 'GetOriginsForIncident';
                Queries.GetOriginsForIncident = GetOriginsForIncident;
                var GetOriginsForIncidentResult = (function () {
                    function GetOriginsForIncidentResult() {
                    }
                    return GetOriginsForIncidentResult;
                }());
                GetOriginsForIncidentResult.TYPE_NAME = 'GetOriginsForIncidentResult';
                Queries.GetOriginsForIncidentResult = GetOriginsForIncidentResult;
                var GetOriginsForIncidentResultItem = (function () {
                    function GetOriginsForIncidentResultItem() {
                    }
                    return GetOriginsForIncidentResultItem;
                }());
                GetOriginsForIncidentResultItem.TYPE_NAME = 'GetOriginsForIncidentResultItem';
                Queries.GetOriginsForIncidentResultItem = GetOriginsForIncidentResultItem;
            })(Queries = ErrorOrigins.Queries || (ErrorOrigins.Queries = {}));
        })(ErrorOrigins = Modules.ErrorOrigins || (Modules.ErrorOrigins = {}));
    })(Modules = OneTrueError.Modules || (OneTrueError.Modules = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var IgnoreFieldAttribute = (function () {
            function IgnoreFieldAttribute() {
            }
            return IgnoreFieldAttribute;
        }());
        IgnoreFieldAttribute.TYPE_NAME = 'IgnoreFieldAttribute';
        Core.IgnoreFieldAttribute = IgnoreFieldAttribute;
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Users;
        (function (Users) {
            var NotificationSettings = (function () {
                function NotificationSettings() {
                }
                return NotificationSettings;
            }());
            NotificationSettings.TYPE_NAME = 'NotificationSettings';
            Users.NotificationSettings = NotificationSettings;
            var NotificationState;
            (function (NotificationState) {
                NotificationState[NotificationState["UseGlobalSetting"] = 0] = "UseGlobalSetting";
                NotificationState[NotificationState["Disabled"] = 1] = "Disabled";
                NotificationState[NotificationState["Cellphone"] = 2] = "Cellphone";
                NotificationState[NotificationState["Email"] = 3] = "Email";
            })(NotificationState = Users.NotificationState || (Users.NotificationState = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetUserSettings;
                }());
                GetUserSettings.TYPE_NAME = 'GetUserSettings';
                Queries.GetUserSettings = GetUserSettings;
                var GetUserSettingsResult = (function () {
                    function GetUserSettingsResult() {
                    }
                    return GetUserSettingsResult;
                }());
                GetUserSettingsResult.TYPE_NAME = 'GetUserSettingsResult';
                Queries.GetUserSettingsResult = GetUserSettingsResult;
            })(Queries = Users.Queries || (Users.Queries = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return UpdateNotifications;
                }());
                UpdateNotifications.TYPE_NAME = 'UpdateNotifications';
                Commands.UpdateNotifications = UpdateNotifications;
                var UpdatePersonalSettings = (function () {
                    function UpdatePersonalSettings() {
                    }
                    return UpdatePersonalSettings;
                }());
                UpdatePersonalSettings.TYPE_NAME = 'UpdatePersonalSettings';
                Commands.UpdatePersonalSettings = UpdatePersonalSettings;
            })(Commands = Users.Commands || (Users.Commands = {}));
        })(Users = Core.Users || (Core.Users = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Support;
        (function (Support) {
            var SendSupportRequest = (function () {
                function SendSupportRequest() {
                }
                return SendSupportRequest;
            }());
            SendSupportRequest.TYPE_NAME = 'SendSupportRequest';
            Support.SendSupportRequest = SendSupportRequest;
        })(Support = Core.Support || (Core.Support = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Reports;
        (function (Reports) {
            var ContextCollectionDTO = (function () {
                function ContextCollectionDTO(name, items) {
                    this.Name = name;
                }
                return ContextCollectionDTO;
            }());
            ContextCollectionDTO.TYPE_NAME = 'ContextCollectionDTO';
            Reports.ContextCollectionDTO = ContextCollectionDTO;
            var ReportDTO = (function () {
                function ReportDTO() {
                }
                return ReportDTO;
            }());
            ReportDTO.TYPE_NAME = 'ReportDTO';
            Reports.ReportDTO = ReportDTO;
            var ReportExeptionDTO = (function () {
                function ReportExeptionDTO() {
                }
                return ReportExeptionDTO;
            }());
            ReportExeptionDTO.TYPE_NAME = 'ReportExeptionDTO';
            Reports.ReportExeptionDTO = ReportExeptionDTO;
        })(Reports = Core.Reports || (Core.Reports = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetReport;
                }());
                GetReport.TYPE_NAME = 'GetReport';
                Queries.GetReport = GetReport;
                var GetReportException = (function () {
                    function GetReportException() {
                    }
                    return GetReportException;
                }());
                GetReportException.TYPE_NAME = 'GetReportException';
                Queries.GetReportException = GetReportException;
                var GetReportList = (function () {
                    function GetReportList(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    return GetReportList;
                }());
                GetReportList.TYPE_NAME = 'GetReportList';
                Queries.GetReportList = GetReportList;
                var GetReportListResult = (function () {
                    function GetReportListResult(items) {
                        this.Items = items;
                    }
                    return GetReportListResult;
                }());
                GetReportListResult.TYPE_NAME = 'GetReportListResult';
                Queries.GetReportListResult = GetReportListResult;
                var GetReportListResultItem = (function () {
                    function GetReportListResultItem() {
                    }
                    return GetReportListResultItem;
                }());
                GetReportListResultItem.TYPE_NAME = 'GetReportListResultItem';
                Queries.GetReportListResultItem = GetReportListResultItem;
                var GetReportResult = (function () {
                    function GetReportResult() {
                    }
                    return GetReportResult;
                }());
                GetReportResult.TYPE_NAME = 'GetReportResult';
                Queries.GetReportResult = GetReportResult;
                var GetReportResultContextCollection = (function () {
                    function GetReportResultContextCollection(name, properties) {
                        this.Name = name;
                        this.Properties = properties;
                    }
                    return GetReportResultContextCollection;
                }());
                GetReportResultContextCollection.TYPE_NAME = 'GetReportResultContextCollection';
                Queries.GetReportResultContextCollection = GetReportResultContextCollection;
                var KeyValuePair = (function () {
                    function KeyValuePair(key, value) {
                        this.Key = key;
                        this.Value = value;
                    }
                    return KeyValuePair;
                }());
                KeyValuePair.TYPE_NAME = 'KeyValuePair';
                Queries.KeyValuePair = KeyValuePair;
            })(Queries = Reports.Queries || (Reports.Queries = {}));
        })(Reports = Core.Reports || (Core.Reports = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Messaging;
        (function (Messaging) {
            var EmailAddress = (function () {
                function EmailAddress(address) {
                    this.Address = address;
                }
                return EmailAddress;
            }());
            EmailAddress.TYPE_NAME = 'EmailAddress';
            Messaging.EmailAddress = EmailAddress;
            var EmailMessage = (function () {
                function EmailMessage() {
                }
                return EmailMessage;
            }());
            EmailMessage.TYPE_NAME = 'EmailMessage';
            Messaging.EmailMessage = EmailMessage;
            var EmailResource = (function () {
                function EmailResource(name, content) {
                    this.Name = name;
                    this.Content = content;
                }
                return EmailResource;
            }());
            EmailResource.TYPE_NAME = 'EmailResource';
            Messaging.EmailResource = EmailResource;
        })(Messaging = Core.Messaging || (Core.Messaging = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return SendSms;
                }());
                SendSms.TYPE_NAME = 'SendSms';
                Commands.SendSms = SendSms;
                var SendEmail = (function () {
                    function SendEmail() {
                    }
                    return SendEmail;
                }());
                SendEmail.TYPE_NAME = 'SendEmail';
                Commands.SendEmail = SendEmail;
                var SendTemplateEmail = (function () {
                    function SendTemplateEmail(mailTitle, templateName) {
                        this.MailTitle = mailTitle;
                        this.TemplateName = templateName;
                    }
                    return SendTemplateEmail;
                }());
                SendTemplateEmail.TYPE_NAME = 'SendTemplateEmail';
                Commands.SendTemplateEmail = SendTemplateEmail;
            })(Commands = Messaging.Commands || (Messaging.Commands = {}));
        })(Messaging = Core.Messaging || (Core.Messaging = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetInvitationByKey;
                }());
                GetInvitationByKey.TYPE_NAME = 'GetInvitationByKey';
                Queries.GetInvitationByKey = GetInvitationByKey;
                var GetInvitationByKeyResult = (function () {
                    function GetInvitationByKeyResult() {
                    }
                    return GetInvitationByKeyResult;
                }());
                GetInvitationByKeyResult.TYPE_NAME = 'GetInvitationByKeyResult';
                Queries.GetInvitationByKeyResult = GetInvitationByKeyResult;
            })(Queries = Invitations.Queries || (Invitations.Queries = {}));
        })(Invitations = Core.Invitations || (Core.Invitations = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return InviteUser;
                }());
                InviteUser.TYPE_NAME = 'InviteUser';
                Commands.InviteUser = InviteUser;
            })(Commands = Invitations.Commands || (Invitations.Commands = {}));
        })(Invitations = Core.Invitations || (Core.Invitations = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
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
            var IncidentSummaryDTO = (function () {
                function IncidentSummaryDTO(id, name) {
                    this.Id = id;
                    this.Name = name;
                }
                return IncidentSummaryDTO;
            }());
            IncidentSummaryDTO.TYPE_NAME = 'IncidentSummaryDTO';
            Incidents.IncidentSummaryDTO = IncidentSummaryDTO;
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return FindIncidentResult;
                }());
                FindIncidentResult.TYPE_NAME = 'FindIncidentResult';
                Queries.FindIncidentResult = FindIncidentResult;
                var FindIncidentResultItem = (function () {
                    function FindIncidentResultItem(id, name) {
                        this.Id = id;
                        this.Name = name;
                    }
                    return FindIncidentResultItem;
                }());
                FindIncidentResultItem.TYPE_NAME = 'FindIncidentResultItem';
                Queries.FindIncidentResultItem = FindIncidentResultItem;
                var FindIncidents = (function () {
                    function FindIncidents() {
                    }
                    return FindIncidents;
                }());
                FindIncidents.TYPE_NAME = 'FindIncidents';
                Queries.FindIncidents = FindIncidents;
                var GetIncident = (function () {
                    function GetIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    return GetIncident;
                }());
                GetIncident.TYPE_NAME = 'GetIncident';
                Queries.GetIncident = GetIncident;
                var GetIncidentForClosePage = (function () {
                    function GetIncidentForClosePage(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    return GetIncidentForClosePage;
                }());
                GetIncidentForClosePage.TYPE_NAME = 'GetIncidentForClosePage';
                Queries.GetIncidentForClosePage = GetIncidentForClosePage;
                var GetIncidentForClosePageResult = (function () {
                    function GetIncidentForClosePageResult() {
                    }
                    return GetIncidentForClosePageResult;
                }());
                GetIncidentForClosePageResult.TYPE_NAME = 'GetIncidentForClosePageResult';
                Queries.GetIncidentForClosePageResult = GetIncidentForClosePageResult;
                var GetIncidentResult = (function () {
                    function GetIncidentResult() {
                    }
                    return GetIncidentResult;
                }());
                GetIncidentResult.TYPE_NAME = 'GetIncidentResult';
                Queries.GetIncidentResult = GetIncidentResult;
                var GetIncidentStatistics = (function () {
                    function GetIncidentStatistics() {
                    }
                    return GetIncidentStatistics;
                }());
                GetIncidentStatistics.TYPE_NAME = 'GetIncidentStatistics';
                Queries.GetIncidentStatistics = GetIncidentStatistics;
                var GetIncidentStatisticsResult = (function () {
                    function GetIncidentStatisticsResult() {
                    }
                    return GetIncidentStatisticsResult;
                }());
                GetIncidentStatisticsResult.TYPE_NAME = 'GetIncidentStatisticsResult';
                Queries.GetIncidentStatisticsResult = GetIncidentStatisticsResult;
                var ReportDay = (function () {
                    function ReportDay() {
                    }
                    return ReportDay;
                }());
                ReportDay.TYPE_NAME = 'ReportDay';
                Queries.ReportDay = ReportDay;
            })(Queries = Incidents.Queries || (Incidents.Queries = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return IncidentIgnored;
                }());
                IncidentIgnored.TYPE_NAME = 'IncidentIgnored';
                Events.IncidentIgnored = IncidentIgnored;
                var IncidentReOpened = (function () {
                    function IncidentReOpened(applicationId, incidentId, createdAtUtc) {
                        this.ApplicationId = applicationId;
                        this.IncidentId = incidentId;
                        this.CreatedAtUtc = createdAtUtc;
                    }
                    return IncidentReOpened;
                }());
                IncidentReOpened.TYPE_NAME = 'IncidentReOpened';
                Events.IncidentReOpened = IncidentReOpened;
                var ReportAddedToIncident = (function () {
                    function ReportAddedToIncident(incident, report, isReOpened) {
                        this.Incident = incident;
                        this.Report = report;
                        this.IsReOpened = isReOpened;
                    }
                    return ReportAddedToIncident;
                }());
                ReportAddedToIncident.TYPE_NAME = 'ReportAddedToIncident';
                Events.ReportAddedToIncident = ReportAddedToIncident;
            })(Events = Incidents.Events || (Incidents.Events = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return CloseIncident;
                }());
                CloseIncident.TYPE_NAME = 'CloseIncident';
                Commands.CloseIncident = CloseIncident;
                var IgnoreIncident = (function () {
                    function IgnoreIncident(incidentId) {
                        this.IncidentId = incidentId;
                    }
                    return IgnoreIncident;
                }());
                IgnoreIncident.TYPE_NAME = 'IgnoreIncident';
                Commands.IgnoreIncident = IgnoreIncident;
            })(Commands = Incidents.Commands || (Incidents.Commands = {}));
        })(Incidents = Core.Incidents || (Core.Incidents = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return SubmitFeedback;
                }());
                SubmitFeedback.TYPE_NAME = 'SubmitFeedback';
                Commands.SubmitFeedback = SubmitFeedback;
            })(Commands = Feedback.Commands || (Feedback.Commands = {}));
        })(Feedback = Core.Feedback || (Core.Feedback = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return FeedbackAttachedToIncident;
                }());
                FeedbackAttachedToIncident.TYPE_NAME = 'FeedbackAttachedToIncident';
                Events.FeedbackAttachedToIncident = FeedbackAttachedToIncident;
            })(Events = Feedback.Events || (Feedback.Events = {}));
        })(Feedback = Core.Feedback || (Core.Feedback = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetApiKey;
                }());
                GetApiKey.TYPE_NAME = 'GetApiKey';
                Queries.GetApiKey = GetApiKey;
                var GetApiKeyResult = (function () {
                    function GetApiKeyResult() {
                    }
                    return GetApiKeyResult;
                }());
                GetApiKeyResult.TYPE_NAME = 'GetApiKeyResult';
                Queries.GetApiKeyResult = GetApiKeyResult;
                var GetApiKeyResultApplication = (function () {
                    function GetApiKeyResultApplication() {
                    }
                    return GetApiKeyResultApplication;
                }());
                GetApiKeyResultApplication.TYPE_NAME = 'GetApiKeyResultApplication';
                Queries.GetApiKeyResultApplication = GetApiKeyResultApplication;
                var ListApiKeys = (function () {
                    function ListApiKeys() {
                    }
                    return ListApiKeys;
                }());
                ListApiKeys.TYPE_NAME = 'ListApiKeys';
                Queries.ListApiKeys = ListApiKeys;
                var ListApiKeysResult = (function () {
                    function ListApiKeysResult() {
                    }
                    return ListApiKeysResult;
                }());
                ListApiKeysResult.TYPE_NAME = 'ListApiKeysResult';
                Queries.ListApiKeysResult = ListApiKeysResult;
                var ListApiKeysResultItem = (function () {
                    function ListApiKeysResultItem() {
                    }
                    return ListApiKeysResultItem;
                }());
                ListApiKeysResultItem.TYPE_NAME = 'ListApiKeysResultItem';
                Queries.ListApiKeysResultItem = ListApiKeysResultItem;
            })(Queries = ApiKeys.Queries || (ApiKeys.Queries = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return ApiKeyCreated;
                }());
                ApiKeyCreated.TYPE_NAME = 'ApiKeyCreated';
                Events.ApiKeyCreated = ApiKeyCreated;
            })(Events = ApiKeys.Events || (ApiKeys.Events = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return CreateApiKey;
                }());
                CreateApiKey.TYPE_NAME = 'CreateApiKey';
                Commands.CreateApiKey = CreateApiKey;
                var DeleteApiKey = (function () {
                    function DeleteApiKey(id) {
                        this.Id = id;
                    }
                    return DeleteApiKey;
                }());
                DeleteApiKey.TYPE_NAME = 'DeleteApiKey';
                Commands.DeleteApiKey = DeleteApiKey;
            })(Commands = ApiKeys.Commands || (ApiKeys.Commands = {}));
        })(ApiKeys = Core.ApiKeys || (Core.ApiKeys = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                return ApplicationListItem;
            }());
            ApplicationListItem.TYPE_NAME = 'ApplicationListItem';
            Applications.ApplicationListItem = ApplicationListItem;
            var TypeOfApplication;
            (function (TypeOfApplication) {
                TypeOfApplication[TypeOfApplication["Mobile"] = 0] = "Mobile";
                TypeOfApplication[TypeOfApplication["DesktopApplication"] = 1] = "DesktopApplication";
                TypeOfApplication[TypeOfApplication["Server"] = 2] = "Server";
            })(TypeOfApplication = Applications.TypeOfApplication || (Applications.TypeOfApplication = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return GetApplicationTeamResult;
                }());
                GetApplicationTeamResult.TYPE_NAME = 'GetApplicationTeamResult';
                Queries.GetApplicationTeamResult = GetApplicationTeamResult;
                var OverviewStatSummary = (function () {
                    function OverviewStatSummary() {
                    }
                    return OverviewStatSummary;
                }());
                OverviewStatSummary.TYPE_NAME = 'OverviewStatSummary';
                Queries.OverviewStatSummary = OverviewStatSummary;
                var GetApplicationIdByKey = (function () {
                    function GetApplicationIdByKey(applicationKey) {
                        this.ApplicationKey = applicationKey;
                    }
                    return GetApplicationIdByKey;
                }());
                GetApplicationIdByKey.TYPE_NAME = 'GetApplicationIdByKey';
                Queries.GetApplicationIdByKey = GetApplicationIdByKey;
                var GetApplicationIdByKeyResult = (function () {
                    function GetApplicationIdByKeyResult() {
                    }
                    return GetApplicationIdByKeyResult;
                }());
                GetApplicationIdByKeyResult.TYPE_NAME = 'GetApplicationIdByKeyResult';
                Queries.GetApplicationIdByKeyResult = GetApplicationIdByKeyResult;
                var GetApplicationInfo = (function () {
                    function GetApplicationInfo() {
                    }
                    return GetApplicationInfo;
                }());
                GetApplicationInfo.TYPE_NAME = 'GetApplicationInfo';
                Queries.GetApplicationInfo = GetApplicationInfo;
                var GetApplicationInfoResult = (function () {
                    function GetApplicationInfoResult() {
                    }
                    return GetApplicationInfoResult;
                }());
                GetApplicationInfoResult.TYPE_NAME = 'GetApplicationInfoResult';
                Queries.GetApplicationInfoResult = GetApplicationInfoResult;
                var GetApplicationList = (function () {
                    function GetApplicationList() {
                    }
                    return GetApplicationList;
                }());
                GetApplicationList.TYPE_NAME = 'GetApplicationList';
                Queries.GetApplicationList = GetApplicationList;
                var GetApplicationOverviewResult = (function () {
                    function GetApplicationOverviewResult() {
                    }
                    return GetApplicationOverviewResult;
                }());
                GetApplicationOverviewResult.TYPE_NAME = 'GetApplicationOverviewResult';
                Queries.GetApplicationOverviewResult = GetApplicationOverviewResult;
                var GetApplicationTeam = (function () {
                    function GetApplicationTeam(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    return GetApplicationTeam;
                }());
                GetApplicationTeam.TYPE_NAME = 'GetApplicationTeam';
                Queries.GetApplicationTeam = GetApplicationTeam;
                var GetApplicationTeamMember = (function () {
                    function GetApplicationTeamMember() {
                    }
                    return GetApplicationTeamMember;
                }());
                GetApplicationTeamMember.TYPE_NAME = 'GetApplicationTeamMember';
                Queries.GetApplicationTeamMember = GetApplicationTeamMember;
                var GetApplicationTeamResultInvitation = (function () {
                    function GetApplicationTeamResultInvitation() {
                    }
                    return GetApplicationTeamResultInvitation;
                }());
                GetApplicationTeamResultInvitation.TYPE_NAME = 'GetApplicationTeamResultInvitation';
                Queries.GetApplicationTeamResultInvitation = GetApplicationTeamResultInvitation;
                var GetApplicationOverview = (function () {
                    function GetApplicationOverview(applicationId) {
                        this.ApplicationId = applicationId;
                    }
                    return GetApplicationOverview;
                }());
                GetApplicationOverview.TYPE_NAME = 'GetApplicationOverview';
                Queries.GetApplicationOverview = GetApplicationOverview;
            })(Queries = Applications.Queries || (Applications.Queries = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return ApplicationDeleted;
                }());
                ApplicationDeleted.TYPE_NAME = 'ApplicationDeleted';
                Events.ApplicationDeleted = ApplicationDeleted;
                var ApplicationCreated = (function () {
                    function ApplicationCreated(id, name, createdById, appKey, sharedSecret) {
                        this.CreatedById = createdById;
                        this.AppKey = appKey;
                        this.SharedSecret = sharedSecret;
                    }
                    return ApplicationCreated;
                }());
                ApplicationCreated.TYPE_NAME = 'ApplicationCreated';
                Events.ApplicationCreated = ApplicationCreated;
                var UserInvitedToApplication = (function () {
                    function UserInvitedToApplication(invitationKey, applicationId, applicationName, emailAddress, invitedBy) {
                        this.InvitationKey = invitationKey;
                        this.ApplicationId = applicationId;
                        this.ApplicationName = applicationName;
                        this.EmailAddress = emailAddress;
                        this.InvitedBy = invitedBy;
                    }
                    return UserInvitedToApplication;
                }());
                UserInvitedToApplication.TYPE_NAME = 'UserInvitedToApplication';
                Events.UserInvitedToApplication = UserInvitedToApplication;
            })(Events = Applications.Events || (Applications.Events = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                                        return UserAddedToApplication;
                                    }());
                                    UserAddedToApplication.TYPE_NAME = 'UserAddedToApplication';
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
                    return RemoveTeamMember;
                }());
                RemoveTeamMember.TYPE_NAME = 'RemoveTeamMember';
                Commands.RemoveTeamMember = RemoveTeamMember;
                var UpdateApplication = (function () {
                    function UpdateApplication(applicationId, name) {
                        this.ApplicationId = applicationId;
                        this.Name = name;
                    }
                    return UpdateApplication;
                }());
                UpdateApplication.TYPE_NAME = 'UpdateApplication';
                Commands.UpdateApplication = UpdateApplication;
                var CreateApplication = (function () {
                    function CreateApplication(name, typeOfApplication) {
                        this.Name = name;
                        this.TypeOfApplication = typeOfApplication;
                    }
                    return CreateApplication;
                }());
                CreateApplication.TYPE_NAME = 'CreateApplication';
                Commands.CreateApplication = CreateApplication;
                var DeleteApplication = (function () {
                    function DeleteApplication(id) {
                        this.Id = id;
                    }
                    return DeleteApplication;
                }());
                DeleteApplication.TYPE_NAME = 'DeleteApplication';
                Commands.DeleteApplication = DeleteApplication;
            })(Commands = Applications.Commands || (Applications.Commands = {}));
        })(Applications = Core.Applications || (Core.Applications = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
(function (OneTrueError) {
    var Core;
    (function (Core) {
        var Accounts;
        (function (Accounts) {
            var RegisterSimple = (function () {
                function RegisterSimple(emailAddress) {
                    this.EmailAddress = emailAddress;
                }
                return RegisterSimple;
            }());
            RegisterSimple.TYPE_NAME = 'RegisterSimple';
            Accounts.RegisterSimple = RegisterSimple;
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return AcceptInvitation;
                }());
                AcceptInvitation.TYPE_NAME = 'AcceptInvitation';
                Requests.AcceptInvitation = AcceptInvitation;
                var AcceptInvitationReply = (function () {
                    function AcceptInvitationReply(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    return AcceptInvitationReply;
                }());
                AcceptInvitationReply.TYPE_NAME = 'AcceptInvitationReply';
                Requests.AcceptInvitationReply = AcceptInvitationReply;
                var ActivateAccount = (function () {
                    function ActivateAccount(activationKey) {
                        this.ActivationKey = activationKey;
                    }
                    return ActivateAccount;
                }());
                ActivateAccount.TYPE_NAME = 'ActivateAccount';
                Requests.ActivateAccount = ActivateAccount;
                var ActivateAccountReply = (function () {
                    function ActivateAccountReply(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    return ActivateAccountReply;
                }());
                ActivateAccountReply.TYPE_NAME = 'ActivateAccountReply';
                Requests.ActivateAccountReply = ActivateAccountReply;
                var ChangePassword = (function () {
                    function ChangePassword(currentPassword, newPassword) {
                        this.CurrentPassword = currentPassword;
                        this.NewPassword = newPassword;
                    }
                    return ChangePassword;
                }());
                ChangePassword.TYPE_NAME = 'ChangePassword';
                Requests.ChangePassword = ChangePassword;
                var ChangePasswordReply = (function () {
                    function ChangePasswordReply() {
                    }
                    return ChangePasswordReply;
                }());
                ChangePasswordReply.TYPE_NAME = 'ChangePasswordReply';
                Requests.ChangePasswordReply = ChangePasswordReply;
                var IgnoreFieldAttribute = (function () {
                    function IgnoreFieldAttribute() {
                    }
                    return IgnoreFieldAttribute;
                }());
                IgnoreFieldAttribute.TYPE_NAME = 'IgnoreFieldAttribute';
                Requests.IgnoreFieldAttribute = IgnoreFieldAttribute;
                var Login = (function () {
                    function Login(userName, password) {
                        this.UserName = userName;
                        this.Password = password;
                    }
                    return Login;
                }());
                Login.TYPE_NAME = 'Login';
                Requests.Login = Login;
                var LoginReply = (function () {
                    function LoginReply() {
                    }
                    return LoginReply;
                }());
                LoginReply.TYPE_NAME = 'LoginReply';
                Requests.LoginReply = LoginReply;
                var LoginResult;
                (function (LoginResult) {
                    LoginResult[LoginResult["Locked"] = 0] = "Locked";
                    LoginResult[LoginResult["IncorrectLogin"] = 1] = "IncorrectLogin";
                    LoginResult[LoginResult["Successful"] = 2] = "Successful";
                })(LoginResult = Requests.LoginResult || (Requests.LoginResult = {}));
                var ResetPassword = (function () {
                    function ResetPassword(activationKey, newPassword) {
                        this.ActivationKey = activationKey;
                        this.NewPassword = newPassword;
                    }
                    return ResetPassword;
                }());
                ResetPassword.TYPE_NAME = 'ResetPassword';
                Requests.ResetPassword = ResetPassword;
                var ResetPasswordReply = (function () {
                    function ResetPasswordReply() {
                    }
                    return ResetPasswordReply;
                }());
                ResetPasswordReply.TYPE_NAME = 'ResetPasswordReply';
                Requests.ResetPasswordReply = ResetPasswordReply;
                var ValidateNewLogin = (function () {
                    function ValidateNewLogin() {
                    }
                    return ValidateNewLogin;
                }());
                ValidateNewLogin.TYPE_NAME = 'ValidateNewLogin';
                Requests.ValidateNewLogin = ValidateNewLogin;
                var ValidateNewLoginReply = (function () {
                    function ValidateNewLoginReply() {
                    }
                    return ValidateNewLoginReply;
                }());
                ValidateNewLoginReply.TYPE_NAME = 'ValidateNewLoginReply';
                Requests.ValidateNewLoginReply = ValidateNewLoginReply;
            })(Requests = Accounts.Requests || (Accounts.Requests = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return AccountDTO;
                }());
                AccountDTO.TYPE_NAME = 'AccountDTO';
                Queries.AccountDTO = AccountDTO;
                var AccountState;
                (function (AccountState) {
                    AccountState[AccountState["VerificationRequired"] = 0] = "VerificationRequired";
                    AccountState[AccountState["Active"] = 1] = "Active";
                    AccountState[AccountState["Locked"] = 2] = "Locked";
                    AccountState[AccountState["ResetPassword"] = 3] = "ResetPassword";
                })(AccountState = Queries.AccountState || (Queries.AccountState = {}));
                var FindAccountByUserName = (function () {
                    function FindAccountByUserName(userName) {
                        this.UserName = userName;
                    }
                    return FindAccountByUserName;
                }());
                FindAccountByUserName.TYPE_NAME = 'FindAccountByUserName';
                Queries.FindAccountByUserName = FindAccountByUserName;
                var GetAccountById = (function () {
                    function GetAccountById(accountId) {
                        this.AccountId = accountId;
                    }
                    return GetAccountById;
                }());
                GetAccountById.TYPE_NAME = 'GetAccountById';
                Queries.GetAccountById = GetAccountById;
                var GetAccountEmailById = (function () {
                    function GetAccountEmailById(accountId) {
                        this.AccountId = accountId;
                    }
                    return GetAccountEmailById;
                }());
                GetAccountEmailById.TYPE_NAME = 'GetAccountEmailById';
                Queries.GetAccountEmailById = GetAccountEmailById;
                var FindAccountByUserNameResult = (function () {
                    function FindAccountByUserNameResult(accountId, displayName) {
                        this.AccountId = accountId;
                        this.DisplayName = displayName;
                    }
                    return FindAccountByUserNameResult;
                }());
                FindAccountByUserNameResult.TYPE_NAME = 'FindAccountByUserNameResult';
                Queries.FindAccountByUserNameResult = FindAccountByUserNameResult;
            })(Queries = Accounts.Queries || (Accounts.Queries = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return AccountActivated;
                }());
                AccountActivated.TYPE_NAME = 'AccountActivated';
                Events.AccountActivated = AccountActivated;
                var AccountRegistered = (function () {
                    function AccountRegistered(accountId, userName) {
                        this.AccountId = accountId;
                        this.UserName = userName;
                    }
                    return AccountRegistered;
                }());
                AccountRegistered.TYPE_NAME = 'AccountRegistered';
                Events.AccountRegistered = AccountRegistered;
                var InvitationAccepted = (function () {
                    function InvitationAccepted(accountId, invitedByUserName, userName) {
                        this.AccountId = accountId;
                        this.InvitedByUserName = invitedByUserName;
                        this.UserName = userName;
                    }
                    return InvitationAccepted;
                }());
                InvitationAccepted.TYPE_NAME = 'InvitationAccepted';
                Events.InvitationAccepted = InvitationAccepted;
                var LoginFailed = (function () {
                    function LoginFailed(userName) {
                        this.UserName = userName;
                    }
                    return LoginFailed;
                }());
                LoginFailed.TYPE_NAME = 'LoginFailed';
                Events.LoginFailed = LoginFailed;
            })(Events = Accounts.Events || (Accounts.Events = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
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
                    return DeclineInvitation;
                }());
                DeclineInvitation.TYPE_NAME = 'DeclineInvitation';
                Commands.DeclineInvitation = DeclineInvitation;
                var RegisterAccount = (function () {
                    function RegisterAccount(userName, password, email) {
                        this.UserName = userName;
                        this.Password = password;
                        this.Email = email;
                    }
                    return RegisterAccount;
                }());
                RegisterAccount.TYPE_NAME = 'RegisterAccount';
                Commands.RegisterAccount = RegisterAccount;
                var RequestPasswordReset = (function () {
                    function RequestPasswordReset(emailAddress) {
                        this.EmailAddress = emailAddress;
                    }
                    return RequestPasswordReset;
                }());
                RequestPasswordReset.TYPE_NAME = 'RequestPasswordReset';
                Commands.RequestPasswordReset = RequestPasswordReset;
            })(Commands = Accounts.Commands || (Accounts.Commands = {}));
        })(Accounts = Core.Accounts || (Core.Accounts = {}));
    })(Core = OneTrueError.Core || (OneTrueError.Core = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=AllModels.js.map