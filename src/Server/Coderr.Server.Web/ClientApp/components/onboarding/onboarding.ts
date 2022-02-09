import { AppRoot } from '@/services/AppRoot';
import Vue from 'vue';
import { Component } from 'vue-property-decorator';
import * as Onboarding from "@/dto/Modules/Onboarding";

@Component({
    components: {
        //AnalyzeMenu: require('./menu.vue.html')
    }
})
export default class OnboardingComponent extends Vue {
    created() {
        var query = new Onboarding.GetOnboardingState();
        AppRoot.Instance.apiClient.query<Onboarding.GetOnboardingStateResult>(query)
            .then(x => {
                console.log(x);
                if (!x.IsComplete) {
                    this.$router.push({ "name": "onboardStart" });
                } else {
                    this.$router.push({ "name": "onboardNextStep" });
                }
            });
    }

}
