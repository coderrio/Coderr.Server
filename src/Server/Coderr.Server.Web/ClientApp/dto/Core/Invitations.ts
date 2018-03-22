export class GetInvitationByKey
{
    public static TYPE_NAME: string = 'GetInvitationByKey';
    public InvitationKey: string;
}
export class GetInvitationByKeyResult
{
    public static TYPE_NAME: string = 'GetInvitationByKeyResult';
    public EmailAddress: string;
}
export class InviteUser
{
    public static TYPE_NAME: string = 'InviteUser';
    public ApplicationId: number;
    public EmailAddress: string;
    public Text: string;
}
export class DeleteInvitation
{
    public static TYPE_NAME: string = 'DeleteInvitation';
    public ApplicationId: number;
    public InvitedEmailAddress: string;
}
