import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
    components: {
        AnalyzeMenu: require('./menu.vue.html')
    }
})
export default class AnalyzeComponent extends Vue {
    created() {
    }


}
