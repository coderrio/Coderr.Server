// ReSharper disable InconsistentNaming
export class RegisterSimple {
  public static TYPE_NAME: string = 'RegisterSimple';
  public emailAddress: string;
}
export class AcceptInvitation {
  public static TYPE_NAME: string = 'AcceptInvitation';
  public acceptedEmail: string;
  public accountId: number;
  public emailUsedForTheInvitation: string;
  public firstName: string;
  public invitationKey: string;
  public lastName: string;
  public password: string;
  public userName: string;
}
export class ChangePassword {
  public static TYPE_NAME: string = 'ChangePassword';
  public currentPassword: string;
  public newPassword: string;
}
export class ValidateNewLoginReply {
  public static TYPE_NAME: string = 'ValidateNewLoginReply';
  public emailIsTaken: boolean;
  public userNameIsTaken: boolean;
}
export class AccountDTO {
  public static TYPE_NAME: string = 'AccountDTO';
  public createdAtUtc: Date;
  public email: string;
  public id: number;
  public lastLoginAtUtc: Date;
  public state: AccountState;
  public updatedAtUtc: Date;
  public userName: string;
}
export enum AccountState {
  VerificationRequired = 0,
  Active = 1,
  Locked = 2,
  ResetPassword = 3,
}
export class FindAccountByUserName {
  public static TYPE_NAME: string = 'FindAccountByUserName';
  public userName: string;
}
export class FindAccountByUserNameResult {
  public static TYPE_NAME: string = 'FindAccountByUserNameResult';
  public accountId: number;
  public displayName: string;
}
export class GetAccountById {
  public static TYPE_NAME: string = 'GetAccountById';
  public accountId: number;
}
export class GetAccountEmailById {
  public static TYPE_NAME: string = 'GetAccountEmailById';
  public accountId: number;
}
export class AccountActivated {
  public static TYPE_NAME: string = 'AccountActivated';
  public accountId: number;
  public emailAddress: string;
  public userName: string;
}
export class AccountRegistered {
  public static TYPE_NAME: string = 'AccountRegistered';
  public accountId: number;
  public isSysAdmin: boolean;
  public userName: string;
}
export class InvitationAccepted {
  public static TYPE_NAME: string = 'InvitationAccepted';
  public acceptedEmailAddress: string;
  public accountId: number;
  public applicationIds: number[];
  public invitedByUserName: string;
  public invitedEmailAddress: string;
  public userName: string;
}
export class LoginFailed {
  public static TYPE_NAME: string = 'LoginFailed';
  public invalidLogin: boolean;
  public isActivated: boolean;
  public isLocked: boolean;
  public userName: string;
}
export class DeclineInvitation {
  public static TYPE_NAME: string = 'DeclineInvitation';
  public invitationId: string;
}
export class RegisterAccount {
  public static TYPE_NAME: string = 'RegisterAccount';
  public accountId: number;
  public activateDirectly: boolean;
  public email: string;
  public password: string;
  public userName: string;
}
export class RequestPasswordReset {
  public static TYPE_NAME: string = 'RequestPasswordReset';
  public emailAddress: string;
}
export class ListAccounts {
  public static TYPE_NAME: string = 'ListAccounts';

}

export class ListAccountsResult {
  public accounts: ListAccountsResultItem[];
}

export class ListAccountsResultItem {
  public accountId: number;
  public userName: string;
  public email: string;
  public createdAtUtc: string;
}
