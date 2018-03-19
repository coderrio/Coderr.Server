import { AppRoot } from '../../../services/AppRoot';
import { SendSupportRequest } from "../../../dto/Core/Support";
import Vue from "vue";
import { Component } from "vue-property-decorator";
import * as SimpleMDE from "SimpleMDE";
import 'simplemde/dist/simplemde.min.css';

@Component
export default class SupportComponent extends Vue {
    private simpleMde$: SimpleMDE;
    message = "";
    referer = "";
    sent = false;

    created() {
        this.referer = document.referrer;

    }


    mounted() {
        this.simpleMde$ = new SimpleMDE({ element: document.getElementById("SupportMessage") });
    }

    sendMessage() {
        console.log('here');
        var cmd = new SendSupportRequest();

        cmd.Message = this.simpleMde$.value();
        cmd.Subject = "Support request";
        cmd.Url = this.referer;
        AppRoot.Instance.apiClient.command(cmd)
            .then(x => {
                AppRoot.notify('Suport request have been sent. We will contact you shortly.');
                this.sent = true;
            });
    }

}
