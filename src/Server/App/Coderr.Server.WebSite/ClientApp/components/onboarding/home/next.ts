import { AppRoot } from '@/services/AppRoot';
import * as Onboarding from "@/dto/Modules/Onboarding";
import * as App from "@/dto/Core/Applications"
import Vue from "vue";
import { Component } from "vue-property-decorator";
import Prism from "prismjs";

declare global {
    interface Window { IsPremise: boolean; }
}

@Component
export default class OnboardingNextComponent extends Vue {
    libs: string[] = [];
    mainFramework = '';
    appKey = '';
    sharedSecret = '';
    showChat = !window.IsPremise;

    created() {
        var query = new Onboarding.GetOnboardingState();
        AppRoot.Instance.apiClient.query<Onboarding.GetOnboardingStateResult>(query)
            .then(result => {
                this.libs = result.Libraries;
                this.mainFramework = result.MainLanguage;
            });

        AppRoot.Instance.applicationService.get(-1).then(x => {
            console.log(x);
            this.appKey = x.appKey;
            this.sharedSecret = x.sharedSecret;
            Vue.nextTick(() => {
                Prism.highlightAll();
            });
        });


    }

    mounted() {
    }


}
