import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
    components: {
        DeploymentMenu: require('./menu.vue.html')
    }
})
export default class DeploymentComponent extends Vue {
    created() {
    }

}
