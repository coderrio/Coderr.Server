module codeRR.Home {
    import ViewModel = Griffin.Yo.Spa.ViewModels.IViewModel;

    export class WelcomeSaasViewModel implements ViewModel {
        getTitle(): string { return "Welcome"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            context.resolve();
        }

        deactivate() {}
    }
}