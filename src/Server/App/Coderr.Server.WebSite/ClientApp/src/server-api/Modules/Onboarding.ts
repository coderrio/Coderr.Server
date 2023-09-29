export class SetOnboardingChoices
{
    public static TYPE_NAME: string = 'SetOnboardingChoices';
    public Libraries: string[];
    public MainLanguage: string;
    public Feedback: string;
}
export class GetOnboardingState {
    public static TYPE_NAME: string = 'GetOnboardingState';
    public Libraries: string[];
    public MainLanguage: string;
}
export class GetOnboardingStateResult
{
    public static TYPE_NAME: string = 'GetOnboardingStateResult';
    public IsComplete: boolean;
    public Libraries: string[];
    public MainLanguage: string;
}
