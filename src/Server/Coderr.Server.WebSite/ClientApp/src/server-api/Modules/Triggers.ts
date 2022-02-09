export enum LastTriggerActionDTO
{
    ExecuteActions = 0,
    AbortTrigger = 1,
}
export class TriggerActionDataDTO
{
    public static TYPE_NAME: string = 'TriggerActionDataDTO';
    public ActionContext: string;
    public ActionName: string;
}
export class TriggerContextRule
{
    public static TYPE_NAME: string = 'TriggerContextRule';
    public ContextName: string;
    public PropertyName: string;
    public PropertyValue: string;
    public Filter: TriggerFilterCondition;
    public ResultToUse: TriggerRuleAction;
}
export class TriggerDTO
{
    public static TYPE_NAME: string = 'TriggerDTO';
    public Description: string;
    public Id: string;
    public Name: string;
    public Summary: string;
}
export class TriggerExceptionRule
{
    public static TYPE_NAME: string = 'TriggerExceptionRule';
    public FieldName: string;
    public Value: string;
    public Filter: TriggerFilterCondition;
    public ResultToUse: TriggerRuleAction;
}
export enum TriggerFilterCondition
{
    StartsWith = 0,
    EndsWith = 1,
    Contains = 2,
    DoNotContain = 3,
    Equals = 4,
}
export enum TriggerRuleAction
{
    AbortTrigger = 0,
    ContinueWithNextRule = 1,
    ExecuteActions = 2,
}
export class TriggerRuleBase
{
    public static TYPE_NAME: string = 'TriggerRuleBase';
    public Filter: TriggerFilterCondition;
    public ResultToUse: TriggerRuleAction;
}
export class GetContextCollectionMetadata
{
    public static TYPE_NAME: string = 'GetContextCollectionMetadata';
    public ApplicationId: number;
}
export class GetContextCollectionMetadataItem
{
    public static TYPE_NAME: string = 'GetContextCollectionMetadataItem';
    public Name: string;
    public Properties: string[];
}
export class GetTrigger
{
    public static TYPE_NAME: string = 'GetTrigger';
    public Id: number;
}
export class GetTriggerDTO
{
    public static TYPE_NAME: string = 'GetTriggerDTO';
    public Actions: TriggerActionDataDTO[];
    public ApplicationId: number;
    public Description: string;
    public Id: number;
    public LastTriggerAction: LastTriggerActionDTO;
    public Name: string;
    public Rules: TriggerRuleBase[];
    public RunForExistingIncidents: boolean;
    public RunForNewIncidents: boolean;
    public RunForReOpenedIncidents: boolean;
}
export class GetTriggersForApplication
{
    public static TYPE_NAME: string = 'GetTriggersForApplication';
    public ApplicationId: number;
}
export class CreateTrigger
{
    public static TYPE_NAME: string = 'CreateTrigger';
    public Actions: TriggerActionDataDTO[];
    public ApplicationId: number;
    public Description: string;
    public Id: number;
    public LastTriggerAction: LastTriggerActionDTO;
    public Name: string;
    public Rules: TriggerRuleBase[];
    public RunForExistingIncidents: boolean;
    public RunForNewIncidents: boolean;
    public RunForReOpenedIncidents: boolean;
}
export class DeleteTrigger
{
    public static TYPE_NAME: string = 'DeleteTrigger';
    public Id: number;
}
export class UpdateTrigger
{
    public static TYPE_NAME: string = 'UpdateTrigger';
    public Actions: TriggerActionDataDTO[];
    public Description: string;
    public Id: number;
    public LastTriggerAction: LastTriggerActionDTO;
    public Name: string;
    public Rules: TriggerRuleBase[];
    public RunForExistingIncidents: boolean;
    public RunForNewIncidents: boolean;
    public RunForReOpenedIncidents: boolean;
}
