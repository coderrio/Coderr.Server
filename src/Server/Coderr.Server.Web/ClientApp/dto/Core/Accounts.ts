export class RegisterSimple
{
    public static TYPE_NAME: string = 'RegisterSimple';
    public EmailAddress: string;
}
export class AcceptInvitation
{
    public static TYPE_NAME: string = 'AcceptInvitation';
    public AcceptedEmail: string;
    public AccountId: number;
    public EmailUsedForTheInvitation: string;
    public FirstName: string;
    public InvitationKey: string;
    public LastName: string;
    public Password: string;
    public UserName: string;
}
export class ChangePassword
{
    public static TYPE_NAME: string = 'ChangePassword';
    public CurrentPassword: string;
    public NewPassword: string;
}
export class ValidateNewLoginReply
{
    public static TYPE_NAME: string = 'ValidateNewLoginReply';
    public EmailIsTaken: boolean;
    public UserNameIsTaken: boolean;
}
export class AccountDTO
{
    public static TYPE_NAME: string = 'AccountDTO';
    public CreatedAtUtc: Date;
    public Email: string;
    public Id: number;
    public LastLoginAtUtc: Date;
    public State: AccountState;
    public UpdatedAtUtc: Date;
    public UserName: string;
}
export enum AccountState
{
    VerificationRequired = 0,
    Active = 1,
    Locked = 2,
    ResetPassword = 3,
}
export class FindAccountByUserName
{
    public static TYPE_NAME: string = 'FindAccountByUserName';
    public UserName: string;
}
export class FindAccountByUserNameResult
{
    public static TYPE_NAME: string = 'FindAccountByUserNameResult';
    public AccountId: number;
    public DisplayName: string;
}
export class GetAccountById
{
    public static TYPE_NAME: string = 'GetAccountById';
    public AccountId: number;
}
export class GetAccountEmailById
{
    public static TYPE_NAME: string = 'GetAccountEmailById';
    public AccountId: number;
}
export class AccountActivated
{
    public static TYPE_NAME: string = 'AccountActivated';
    public AccountId: number;
    public EmailAddress: string;
    public UserName: string;
}
export class AccountRegistered
{
    public static TYPE_NAME: string = 'AccountRegistered';
    public AccountId: number;
    public IsSysAdmin: boolean;
    public UserName: string;
}
export class InvitationAccepted
{
    public static TYPE_NAME: string = 'InvitationAccepted';
    public AcceptedEmailAddress: string;
    public AccountId: number;
    public ApplicationIds: number[];
    public InvitedByUserName: string;
    public InvitedEmailAddress: string;
    public UserName: string;
}
export class LoginFailed
{
    public static TYPE_NAME: string = 'LoginFailed';
    public InvalidLogin: boolean;
    public IsActivated: boolean;
    public IsLocked: boolean;
    public UserName: string;
}
export class DeclineInvitation
{
    public static TYPE_NAME: string = 'DeclineInvitation';
    public InvitationId: string;
}
export class RegisterAccount
{
    public static TYPE_NAME: string = 'RegisterAccount';
    public AccountId: number;
    public ActivateDirectly: boolean;
    public Email: string;
    public Password: string;
    public UserName: string;
}
export class RequestPasswordReset
{
    public static TYPE_NAME: string = 'RequestPasswordReset';
    public EmailAddress: string;
}
