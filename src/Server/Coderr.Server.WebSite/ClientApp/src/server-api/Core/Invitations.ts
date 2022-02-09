export class GetInvitationByKey {
  public static TYPE_NAME: string = 'GetInvitationByKey';
  public invitationKey: string;
}
export class GetInvitationByKeyResult {
  public static TYPE_NAME: string = 'GetInvitationByKeyResult';
  public emailAddress: string;
}
export class InviteUser {
  public static TYPE_NAME: string = 'InviteUser';
  public applicationId: number;
  public emailAddress: string;
  public text: string;
}
export class DeleteInvitation {
  public static TYPE_NAME: string = 'DeleteInvitation';
  public applicationId: number;
  public invitedEmailAddress: string;
}
