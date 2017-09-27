module codeRR.Account {
    import ViewModel = Griffin.Yo.Spa.ViewModels.IViewModel;

    export class AcceptedViewModel implements ViewModel {
        getTitle(): string { return "Invitation accepted"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            context.resolve();
        }

        deactivate() {}
    }
}