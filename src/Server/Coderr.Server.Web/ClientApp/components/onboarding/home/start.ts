import { AppRoot } from '../../../services/AppRoot';
import * as Demo from "@/dto/Common/Demo"
import * as Onboarding from "@/dto/Modules/Onboarding";
import Vue from "vue";
import { Component } from "vue-property-decorator";

export interface IDemoItem {
    description: string;
    title: string;
    id: string;
    selected: boolean;
}

interface IDemoCategory {
    name: string;
    items: IDemoItem[];
}

@Component
export default class OnboardingHomeComponent extends Vue {
    weAreInTrouble = false;
    showChat = !window.IsPremise;

    framework: string = '';

    demoIncidents: IDemoCategory[] = [];
    private oldCategory: string = '';

    noConnection = false;
    gotNoIncidents = false;

    canShowNext = true;
    canShowPrev = true;

    feedback = '';

    page = 1;

    private callbacks: (() => void)[] = [];

    created() {
        this.callbacks.push(null);
        this.callbacks.push(null);
        this.callbacks.push(this.showDemoOptions);
        this.callbacks.push(this.showComplete);
    }

    mounted() {
    }

    showComplete() {
        var cmd2 = new Onboarding.SetOnboardingChoices();
        cmd2.Libraries = [];
        cmd2.MainLanguage = this.framework;
        cmd2.Feedback = this.feedback;
        AppRoot.Instance.apiClient.command(cmd2);
        this.page = 3;
    }

    async showDemoOptions() {
        this.page = 2;
        var dto = new Demo.GetDemoIncidentOptions();
        var result = await AppRoot.Instance.apiClient.query<Demo.GetDemoIncidentOptionsResult>(dto);
        this.demoIncidents = [];
        var category: IDemoCategory = null;

        result.Items.forEach(dto => {
            if (this.framework === 'nodejs') {
                if (dto.Category !== 'JavaScript' && dto.Category !== "VueJS") {
                    return;
                }
            }

            if (category == null || dto.Category !== category.name) {
                category = {
                    name: dto.Category,
                    items: []
                };

                this.demoIncidents.push(category);
            }

            var item = {
                id: dto.Id,
                description: dto.Description,
                title: dto.Title,
                selected: false
            };
            category.items.push(item);
        });

    }

    generateErrors() {
        var itemsToGenerate: string[] = [];
        var libs: string[] = [];
        this.demoIncidents.forEach(category => {
            category.items.forEach(dto => {
                if (dto.selected) {
                    itemsToGenerate.push(dto.id);
                    if (libs.findIndex(x => x === category.name) === -1) {
                        libs.push(category.name);
                    }
                }
            });
        });
        var cmd = new Demo.GenerateDemoIncidents();
        cmd.DemoOptionIds = itemsToGenerate;
        AppRoot.Instance.apiClient.command(cmd);

        var cmd2 = new Onboarding.SetOnboardingChoices();
        cmd2.Libraries = libs;
        cmd2.MainLanguage = this.framework;
        cmd2.Feedback = this.feedback;
        AppRoot.Instance.apiClient.command(cmd2);

        this.page++;
    }

    next() {
        if (this.page < 5) {
            this.page++;
            this.canShowPrev = true;
            this.runMethod(this.page);
        }


        if (this.page === 5) {
            this.canShowNext = false;
        }
    }

    prev() {
        if (this.page > 1) {
            this.page--;
            this.canShowNext = true;
            this.runMethod(this.page);
        }

        if (this.page === 5) {
            this.canShowPrev = false;
        }
    }

    selectFramework(value: string) {
        this.framework = value;
        this.next();
    }

    toggleMe(item: IDemoItem) {
        console.log(item);
        item.selected = !item.selected;
    }

    isNewCategory(newName: string): boolean {
        if (newName === this.oldCategory)
            return false;

        this.oldCategory = newName;
        return true;
    }

    exitGuide() {
        this.$router.push({ "name": "discover" });
    }

    private runMethod(index: number) {
        var method = this.callbacks[index];
        if (method !== null)
            method();
    }

}
