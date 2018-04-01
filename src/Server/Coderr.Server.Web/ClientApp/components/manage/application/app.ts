import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
    components: {
        ManageAppMenu: require('./menu.vue.html')
    }
})
export default class ManageComponent extends Vue {
    created() {
    }

}
