import { AppRoot } from '../../../../services/AppRoot';
import Vue from "vue";
import { Component } from "vue-property-decorator";


@Component
export default class ManageHomeComponent extends Vue {


    created() {
        if (this.$route.params.applicationId) {
            this.$router.push({
                name: 'manageAppSettings',
                params: { applicationId: this.$route.params.applicationId }
            });
        }
    }

    
    mounted() {
    }
    

}
