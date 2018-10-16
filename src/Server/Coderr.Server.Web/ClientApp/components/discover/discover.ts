import Vue from 'vue';
import { Component } from 'vue-property-decorator';

@Component({
    components: {
        DiscoverMenu: require('./menu.vue.html')
    }
})
export default class DiscoverComponent extends Vue {
}
